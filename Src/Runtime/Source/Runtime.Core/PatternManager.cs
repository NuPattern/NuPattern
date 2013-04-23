using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Linq;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Modeling.Shell;
using Microsoft.VisualStudio.Shell;
using NuPattern.Diagnostics;
using NuPattern.IO;
using NuPattern.Modeling;
using NuPattern.Presentation;
using NuPattern.Reflection;
using NuPattern.Runtime.Properties;
using NuPattern.Runtime.Store;
using NuPattern.Runtime.Validation;
using NuPattern.VisualStudio;
using NuPattern.VisualStudio.Solution;
using Dsl = Microsoft.VisualStudio.Modeling;

namespace NuPattern.Runtime
{
    /// <summary>
    /// Defines a way to manage patterns in a given instance of Visual Studio.
    /// </summary>
    [Export(typeof(IPatternManager))]
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "This class deals with mapping and instantiation of DSL types, which are many.")]
    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Disposed automatically by the underlying state when closed. Cannot dispose earlier 'cause that deletes the errors from the error list.")]
    internal class PatternManager : IPatternManager
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<PatternManager>();
        private const string DslVersionAttribute = "dslVersion";

        private IServiceProvider serviceProvider;
        private ISolution solution;
        private ISolutionEvents solutionEvents;
        private IShellEvents shellEvents;
        private IItemEvents itemEvents;
        private IList<IInstalledToolkitInfo> installedToolkits;
        private IUserMessageService messageService;
        private ProductState productStore;
        private ErrorListObserver errorListObserver;
        private VsValidationController validationController;

        /// <summary>
        /// Initializes a new instance of the <see cref="PatternManager"/> class.
        /// </summary>
        /// <param name="serviceProvider">The Service Provider.</param>
        /// <param name="solutionEvents">The solution events.</param>
        /// <param name="solution">The solution</param>
        /// <param name="shellEvents">The shell events.</param>
        /// <param name="itemEvents">The item events</param>
        /// <param name="installedToolkits">The installed toolkits.</param>
        /// <param name="messageService">The messaging service</param>
        [ImportingConstructor]
        public PatternManager(
            [Import(typeof(SVsServiceProvider))] IServiceProvider serviceProvider,
            ISolution solution,
            IShellEvents shellEvents,
            ISolutionEvents solutionEvents,
            IItemEvents itemEvents,
            IEnumerable<IInstalledToolkitInfo> installedToolkits,
            IUserMessageService messageService)
        {
            Guard.NotNull(() => serviceProvider, serviceProvider);
            Guard.NotNull(() => solution, solution);
            Guard.NotNull(() => shellEvents, shellEvents);
            Guard.NotNull(() => solutionEvents, solutionEvents);
            Guard.NotNull(() => itemEvents, itemEvents);
            Guard.NotNull(() => installedToolkits, installedToolkits);
            Guard.NotNull(() => messageService, messageService);

            this.serviceProvider = serviceProvider;
            this.solution = solution;
            this.solutionEvents = solutionEvents;
            this.shellEvents = shellEvents;
            this.itemEvents = itemEvents;
            this.installedToolkits = installedToolkits.ToArray();
            this.messageService = messageService;

            this.solutionEvents.SolutionOpened += this.OnSolutionOpened;
            this.solutionEvents.SolutionClosing += this.OnSolutionClosing;
            this.itemEvents.ItemRemoved += OnItemRemoved;
        }

        /// <summary>
        /// Occurs when an pattern/element is activated.
        /// </summary>
        public virtual event EventHandler<ValueEventArgs<IProductElement>> ElementActivated = (s, e) => { };

        /// <summary>
        /// Occurs when an element was deleted.
        /// </summary>
        public virtual event EventHandler<ValueEventArgs<IProductElement>> ElementDeleted = (s, e) => { };

        /// <summary>
        /// Occurs when an element is being deleted.
        /// </summary>
        public virtual event EventHandler<ValueEventArgs<IProductElement>> ElementDeleting = (s, e) => { };

        /// <summary>
        /// Occurs when an element is instantiated.
        /// </summary>
        public virtual event EventHandler<ValueEventArgs<IProductElement>> ElementInstantiated = (s, e) => { };

        /// <summary>
        /// Occurs when an element is created.
        /// </summary>
        public virtual event EventHandler<ValueEventArgs<IProductElement>> ElementCreated = (s, e) => { };

        /// <summary>
        /// Occurs when an element is serialized.
        /// </summary>
        public virtual event EventHandler<ValueEventArgs<IProductElement>> ElementLoaded = (s, e) => { };

        /// <summary>
        /// Occurs when an pattern state is saved.
        /// </summary>
        public virtual event EventHandler<ValueEventArgs<IProductState>> StoreSaved = (s, e) => { };

        /// <summary>
        /// Occurs when the pattern manager opens or closes a state.
        /// </summary>
        public virtual event EventHandler IsOpenChanged = (s, e) => { };

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public virtual event PropertyChangedEventHandler PropertyChanged = (s, e) => { };

        /// <summary>
        /// Gets the path to the state that was last opened, or null.
        /// </summary>
        public virtual string StoreFile { get; private set; }

        /// <summary>
        /// Gets the opened state, if <see cref="IsOpen"/> is <see langword="true"/>.
        /// </summary>
        public virtual IProductState Store
        {
            get { return this.productStore; }
        }

        /// <summary>
        /// Gets a value indicating whether the pattern manager has an opened a state.
        /// </summary>
        public virtual bool IsOpen { get; private set; }

        /// <summary>
        /// Gets the installed toolkits.
        /// </summary>
        public virtual IEnumerable<IInstalledToolkitInfo> InstalledToolkits
        {
            get { return this.installedToolkits; }
        }

        /// <summary>
        /// Gets the instantiated products.
        /// </summary>
        public virtual IEnumerable<IProduct> Products
        {
            get { return this.IsOpen ? this.productStore.Products.Order() : Enumerable.Empty<IProduct>(); }
        }

        /// <summary>
        /// Gets the validation controller.
        /// </summary>
        /// <value>The validation controller.</value>
        public virtual VsValidationController ValidationController
        {
            get
            {
                if (this.validationController == null)
                {
                    this.validationController = new ProductStoreValidationController(this.productStore.Store);
                    this.errorListObserver = new ProductStoreValidationObserver(this.productStore.Store);
                    this.validationController.AddObserver(this.errorListObserver);

                    // Register the validation registrar
                    var runtimeExtensionRegistrar = new RuntimeValidationExtensionRegistrar();

                    var mef = this.serviceProvider.GetService<SComponentModel, IComponentModel>();
                    if (mef != null)
                    {
                        mef.DefaultCompositionService.SatisfyImportsOnce(runtimeExtensionRegistrar);
                    }

                    this.validationController.ValidationExtensionRegistrar = runtimeExtensionRegistrar;
                }

                return this.validationController;
            }
        }

        /// <summary>
        /// Opens the specified pattern state.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling"), SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "NotApplicable")]
        public virtual void Open(string storeFile, bool autoCreate = false)
        {
            Guard.NotNullOrEmpty(() => storeFile, storeFile);

            using (tracer.StartActivity(Resources.PatternManager_TraceOpeningStore, storeFile))
            using (new MouseCursor(System.Windows.Input.Cursors.Wait))
            {
                if (this.IsOpen)
                {
                    this.Close();
                }

                this.StoreFile = storeFile;
                tracer.TraceData(TraceEventType.Verbose, Resources.PatternManager_OpeningStoreEventId, storeFile);

                if (autoCreate && (!File.Exists(storeFile) || new FileInfo(storeFile).Length == 0))
                    storeFile = CreateEmptyStateFile(storeFile);

                //// Do NOT dispose the DSL state or it becomes null in the model elements!!
                if (VerifyDslVersion(storeFile))
                {
                    var result = new Dsl.SerializationResult();

                    var dslStore = new Dsl.Store(this.serviceProvider, typeof(Dsl.CoreDomainModel), typeof(ProductStateStoreDomainModel));

                    using (var tx = dslStore.TransactionManager.BeginTransaction(Resources.PatternManager_OpenTransactionDescription, true))
                    {
                        using (new StorePropertyBag(dslStore, ProductState.IsSerializingKey, true))
                        {
                            // Flag the state as being deserialized. This is 
                            // required by the ProductState class to ignore 
                            // element added events until deserialization is 
                            // completed.

                            var store = ProductStateStoreSerializationHelper.Instance.LoadModel(result, dslStore, storeFile, null, null, null);

                            if (!result.Failed)
                            {
                                this.productStore = store;
                                tx.Commit();
                            }
                        }
                    }

                    if (!result.Failed)
                    {
                        this.IsOpen = true;

                        this.Store.TransactionCommited += this.OnTransactionCommitedDoSave;
                        this.Store.ElementInstantiated += this.OnElementInstantiated;
                        this.Store.ElementLoaded += this.OnElementLoaded;
                        this.Store.ElementCreated += this.OnElementCreated;
                        this.Store.ElementDeleted += this.OnElementDeleted;
                        this.Store.ElementDeleting += this.OnElementDeleting;

                        this.RaiseIsOpenChanged();

                        tracer.TraceInformation(Resources.PatternManager_OpenedStore, this.StoreFile);
                        tracer.TraceData(TraceEventType.Verbose, Resources.PatternManager_OpenedStoreEventId, storeFile);
                    }
                    else
                    {
                        this.messageService.ShowError(
                                Properties.Resources.PatternManager_FailedToOpenStore);

                        tracer.TraceError(Properties.Resources.PatternManager_DeSerializationFailed, this.StoreFile);
                        result.ForEach(msg => tracer.TraceWarning(msg.ToString()));
                    }
                }
            }
        }

        /// <summary>
        /// Saves all current products to the underlying state.
        /// </summary>
        public virtual void Save()
        {
            if (!this.IsOpen)
            {
                throw new InvalidOperationException(Resources.PatternManager_SaveInvalidIfNotOpen);
            }

            tracer.TraceData(TraceEventType.Verbose, Resources.PatternManager_SavingStoreEventId, StoreFile);
            this.SaveModel(this.productStore, this.StoreFile);
            this.productStore.RaiseSaved();
            this.StoreSaved(this, new ValueEventArgs<IProductState>(this.productStore));
            tracer.TraceData(TraceEventType.Verbose, Resources.PatternManager_SavedStoreEventId, StoreFile);
        }

        /// <summary>
        /// Saves all current products to the underlying state with a new name
        /// </summary>
        /// <param name="fileName">The new file name for the state, without directory</param>
        public virtual void SaveAs(string fileName)
        {
            this.StoreFile = fileName;
            Save();
        }

        /// <summary>
        /// Saves all pending changes to the underlying state, and closes the state if it was opened, otherwise throws <see cref="InvalidOperationException"/>.
        /// </summary>
        public virtual void Close()
        {
            this.DoClose();
        }

        /// <summary>
        /// Creates the pattern from the specified <see cref="IInstalledToolkitInfo"/>.
        /// </summary>
        /// <param name="toolkitInfo">The toolkit info.</param>
        /// <param name="name">The pattern name.</param>
        /// <param name="raiseInstantiateEvents">Whether instantiation events should be raised.</param>
        public virtual IProduct CreateProduct(IInstalledToolkitInfo toolkitInfo, string name, bool raiseInstantiateEvents = true)
        {
            Guard.NotNull(() => toolkitInfo, toolkitInfo);
            Guard.NotNullOrEmpty(() => name, name);

            if (!this.IsOpen)
            {
                this.OpenDefaultState();
            }

            if (this.Find(name) != null)
            {
                throw new ArgumentException(Resources.DuplicateProductName);
            }

            IProduct product;

            using (var tx = this.productStore.Store.TransactionManager.BeginTransaction())
            using (tracer.StartActivity(Resources.PatternManager_TraceCreatingProduct, name, toolkitInfo.Extension.Header.Name))
            using (new StorePropertyBag(this.productStore.Store, ProductState.IsCreatingElementKey, true))
            {
                product = this.productStore.CreateProduct(p =>
                {
                    p.ExtensionId = toolkitInfo.Id;
                    p.DefinitionId = toolkitInfo.Schema.Pattern.Id;
                    p.InstanceName = name;

                    var prod = p as Product;
                    if (p != null)
                    {
                        prod.ExtensionName = toolkitInfo.Name;
                        prod.Author = toolkitInfo.Author;
                        prod.Version = toolkitInfo.Version.ToString();
                    }
                });

                // Flag the element in the state so that the ProductState class sees it and doesn't 
                // raise the instantiation event.
                if (!raiseInstantiateEvents)
                {
                    this.productStore.Store.PropertyBag.Add(product, null);
                }

                tx.Commit();
            }

            return product;
        }

        /// <summary>
        /// Activates the specified element.
        /// </summary>
        public virtual void ActivateElement(IProductElement element)
        {
            Guard.NotNull(() => element, element);

            // Nothing to do here.

            // Raise the event
            this.ElementActivated(this, new ValueEventArgs<IProductElement>(element));
        }

        /// <summary>
        /// Deletes the specified product.
        /// </summary>
        /// <param name="product">The product.</param>
        public bool DeleteProduct(IProduct product)
        {
            Guard.NotNull(() => product, product);

            var removed = false;
            this.productStore.WithTransaction(s => removed = s.Products.Remove((Product)product));
            return removed;
        }

        /// <summary>
        /// Validates the specified elements.
        /// </summary>
        /// <param name="elements">The elements.</param>
        public virtual bool Validate(IEnumerable<IInstanceBase> elements)
        {
            var modelElements = elements.OfType<Dsl.ModelElement>();

            return this.ValidationController.ValidateCustom(modelElements, ValidationConstants.RuntimeValidationCategory);
        }

        /// <summary>
        /// Validates the product state.
        /// </summary>
        public virtual void ValidateProductState()
        {
            ValidateProductState(this.productStore);
        }

        /// <summary>
        /// Validates the given product state.
        /// </summary>
        public virtual void ValidateProductState(IProductState store)
        {
            this.ValidationController.ValidateCustom(((ProductState)store).Store, ValidationConstants.RuntimeValidationCategory);
        }

        /// <summary>
        /// When overriden by a derived class, changes the default behavior for adding the state file 
        /// to the given parent folder.
        /// </summary>
        /// <returns>The added item.</returns>
        protected virtual IItemContainer AddStateToSolution(IItemContainer targetParent, string targetName, string sourceFile)
        {
            Guard.NotNull(() => targetParent, targetParent);
            Guard.NotNullOrEmpty(() => targetName, targetName);
            Guard.NotNullOrEmpty(() => sourceFile, sourceFile);

            tracer.TraceVerbose(Resources.PatternManager_TraceAddingFileToSolution, targetParent.PhysicalPath, targetName);

            // BUGFIX: FBRT does not checkout the file if it exists in the 'Solution Items' directory,
            //	as the path of the 'Solution Items' folder is the path of the solution.
            var targetItem = targetParent.Find<IItem>(targetName).FirstOrDefault();
            if (targetItem != null)
            {
                targetItem.Checkout();
            }

            // This should be doing the right thing, checking out the file, replacing content, etc.
            return targetParent.Add(sourceFile, targetName, true, false);
        }

        private bool VerifyDslVersion(string storeFile)
        {
            bool isValidVersion = true;

            var exception =
                tracer.Shield(
                () =>
                {
                    var document = XDocument.Load(storeFile);
                    var dslVersion = new Version(document.Root.Attribute(DslVersionAttribute).Value);

                    if (dslVersion != StoreConstants.DslVersion)
                    {
                        if (this.messageService.PromptWarning(
                                string.Format(CultureInfo.InvariantCulture,
                                    Properties.Resources.PatternManager_NewerDslVersionUpgrade, Path.GetFileName(this.StoreFile), StoreConstants.ProductName)))
                        {
                            document.Root.Attribute(DslVersionAttribute).Value = StoreConstants.DslVersion.ToString();

                            VsHelper.CheckOut(storeFile);

                            document.Save(storeFile);
                        }
                        else
                        {
                            isValidVersion = false;
                        }
                    }
                }, Properties.Resources.PatternManager_FailedToVerifyDslVersion);

            return isValidVersion && exception == null;
        }

        private void DoClose(bool saveStore = true)
        {
            if (!this.IsOpen)
            {
                throw new InvalidOperationException(Resources.PatternManager_CloseInvalidIfNotOpen);
            }

            using (tracer.StartActivity(Resources.PatternManager_TraceClosingStore, this.StoreFile))
            {
                if (saveStore)
                {
                    this.Save();
                }

                this.Store.TransactionCommited -= this.OnTransactionCommitedDoSave;
                this.Store.ElementInstantiated -= this.OnElementInstantiated;
                this.Store.ElementCreated -= this.OnElementCreated;
                this.Store.ElementLoaded -= this.OnElementLoaded;
                this.Store.ElementDeleted -= this.OnElementDeleted;
                this.Store.ElementDeleting -= this.OnElementDeleting;

                this.productStore.Store.Dispose();

                if (this.validationController != null)
                {
                    this.validationController.ClearMessages();
                    this.validationController.RemoveObserver(this.errorListObserver);
                    this.validationController = null;

                    if (this.errorListObserver != null)
                    {
                        this.errorListObserver.Dispose();
                        this.errorListObserver = null;
                    }
                }

                this.IsOpen = false;
                this.RaiseIsOpenChanged();
            }

        }

        private void OnTransactionCommitedDoSave(object sender, EventArgs e)
        {
            this.Store.TransactionCommited -= this.OnTransactionCommitedDoSave;
            try
            {
                this.Save();
            }
            finally
            {
                this.Store.TransactionCommited += this.OnTransactionCommitedDoSave;
            }
        }

        private void OnElementDeleted(object sender, ValueEventArgs<IInstanceBase> e)
        {
            var element = e.Value as IProductElement;
            if (element != null)
            {
                this.ElementDeleted(this, new ValueEventArgs<IProductElement>(element));
            }
        }

        private void OnElementDeleting(object sender, ValueEventArgs<IInstanceBase> e)
        {
            var element = e.Value as IProductElement;
            if (element != null)
            {
                this.ElementDeleting(this, new ValueEventArgs<IProductElement>(element));
            }
        }

        private void OnElementInstantiated(object sender, ValueEventArgs<IInstanceBase> e)
        {
            var element = e.Value as IProductElement;
            if (element != null)
            {
                tracer.TraceData(TraceEventType.Verbose, Resources.PatternManager_ElementInstantiatingEventId, element);
                this.ElementInstantiated(this, new ValueEventArgs<IProductElement>(element));
                tracer.TraceData(TraceEventType.Verbose, Resources.PatternManager_ElementInstantiatedEventId, element);
            }
        }

        private void OnElementCreated(object sender, ValueEventArgs<IInstanceBase> e)
        {
            var element = e.Value as IProductElement;
            if (element != null)
            {
                tracer.TraceData(TraceEventType.Verbose, Resources.PatternManager_ElementCreatingEventId, element);
                this.ElementCreated(this, new ValueEventArgs<IProductElement>(element));
                tracer.TraceData(TraceEventType.Verbose, Resources.PatternManager_ElementCreatedEventId, element);
            }
        }

        private void OnElementLoaded(object sender, ValueEventArgs<IInstanceBase> e)
        {
            var element = e.Value as IProductElement;
            if (element != null)
            {
                tracer.TraceData(TraceEventType.Verbose, Resources.PatternManager_ElementLoadingEventId, element);
                this.ElementLoaded(this, new ValueEventArgs<IProductElement>(element));
                tracer.TraceData(TraceEventType.Verbose, Resources.PatternManager_ElementLoadedEventId, element);
            }
        }

        private void OpenDefaultState()
        {
            if (!this.TryOpenSolutionState(this.solution))
            {
                var fileName = GetDefaultStateFileName(this.solution);
                var savedFile = CreateEmptyStateFile(fileName);

                this.Open(savedFile);
            }
        }

        private string CreateEmptyStateFile(string fileName)
        {
            string savedFile;
            ProductState emptyStore;
            using (var dslStore = new Dsl.Store(typeof(Dsl.CoreDomainModel), typeof(ProductStateStoreDomainModel)))
            {
                using (var tx = dslStore.TransactionManager.BeginTransaction())
                {
                    emptyStore = dslStore.ElementFactory.CreateElement<ProductState>();
                    tx.Commit();
                }

                tracer.TraceVerbose(Resources.PatternManager_TraceSavingDefaultEmptyState, fileName);
                savedFile = this.SaveModel(emptyStore, fileName);
            }

            return savedFile;
        }

        private string SaveModel(ProductState store, string fileName)
        {
            var result = new Dsl.SerializationResult();
            var itemName = Path.GetFileName(fileName);

            if (solution.IsOpen)
            {
                // First do the quick lookup in the default location.
                IItemContainer itemParent = this.solution.SolutionFolders.FirstOrDefault(folder =>
                    folder.Name.Equals(SolutionExtensions.SolutionItemsFolderName, StringComparison.OrdinalIgnoreCase) &&
                    folder.Items.Any(item => item.PhysicalPath == fileName));

                if (itemParent == null)
                {
                    // Try the slow probing anywhere in the solution.
                    itemParent = (from item in this.solution.Traverse().OfType<IItem>()
                                  where item.PhysicalPath == fileName
                                  select item.Parent)
                                  .FirstOrDefault();
                }

                if (itemParent == null)
                {
                    tracer.TraceVerbose(Resources.PatternManager_TraceSavingDefaultEmptyState, fileName);

                    // Default to SolutionItems.
                    itemParent = this.solution.Items.OfType<ISolutionFolder>().FirstOrDefault(x => x.IsSolutionItemsFolder());
                    if (itemParent == null)
                    {
                        tracer.TraceVerbose(Resources.PatternManager_SaveNoSolutionItems);
                        itemParent = this.solution.CreateSolutionFolder(SolutionExtensions.SolutionItemsFolderName);
                    }
                }

                // Serialize to temp to add or override content next.
                var tempFileName = Path.GetTempFileName();
                ProductStateStoreSerializationHelper.Instance.SaveModel(result, store, tempFileName);

                var addedItem = this.AddStateToSolution(itemParent, itemName, tempFileName);

                // Cleanup the temporary file.
                File.Delete(tempFileName);

                return addedItem.PhysicalPath;
            }
            else
            {
                ProductStateStoreSerializationHelper.Instance.SaveModel(result, store, fileName);
                return fileName;
            }
        }

        private void OnItemRemoved(object sender, FileEventArgs e)
        {
            if (e.FileName == this.StoreFile)
                this.DoClose(false);
        }

        private void OnShellInitialized(object sender, EventArgs e)
        {
            OnSolutionOpened(sender, new SolutionEventArgs(this.solution));
        }

        private void OnSolutionOpened(object sender, SolutionEventArgs e)
        {
            if (!this.shellEvents.IsInitialized)
            {
                // Schedule solution opening for later when 
                // shell is initialized.
                this.shellEvents.ShellInitialized += this.OnShellInitialized;
            }
            else
            {
                tracer.ShieldUI(
                    () => this.TryOpenSolutionState(e.Solution),
                    Resources.PatternManager_FailedToOpenStore);
            }
        }

        private void OnSolutionClosing(object sender, SolutionEventArgs e)
        {
            if (this.IsOpen)
            {
                tracer.ShieldUI(
                    () => this.Close(),
                    Resources.PatternManager_FailedToCloseOpenedStore,
                    this.StoreFile);
            }
        }

        private bool TryOpenSolutionState(ISolution solutionToOpen)
        {
            var stateFiles = FindStateFiles(solutionToOpen);
            if (stateFiles.Length == 0)
            {
                return false;
            }

            if (stateFiles.Length == 1)
            {
                this.Open(stateFiles[0].PhysicalPath);
            }
            else
            {
                var defaultFileName = GetDefaultStateFileName(solutionToOpen);
                var defaultState = stateFiles.FirstOrDefault(i => Path.GetFileName(i.PhysicalPath) == defaultFileName);

                if (defaultState != null)
                {
                    this.Open(defaultState.PhysicalPath);
                }
                else
                {
                    this.Open(stateFiles[0].PhysicalPath);
                }
            }

            this.solution = solutionToOpen;
            return true;
        }

        private static string GetDefaultStateFileName(ISolution solution)
        {
            return Path.ChangeExtension(Path.GetFileName(solution.PhysicalPath), StoreConstants.RuntimeStoreExtension);
        }

        private static IItem[] FindStateFiles(ISolution solution)
        {
            var slnItems = solution.Items.OfType<ISolutionFolder>().FirstOrDefault(i => i.IsSolutionItemsFolder());
            if (slnItems == null)
            {
                return new IItem[0];
            }

            return slnItems.Items
                .OfType<IItem>()
                .Where(i => i.PhysicalPath.EndsWith(StoreConstants.RuntimeStoreExtension, StringComparison.OrdinalIgnoreCase))
                .ToArray();
        }

        private void RaiseIsOpenChanged()
        {
            this.IsOpenChanged(this, EventArgs.Empty);
            this.RaisePropertyChanged(x => x.IsOpen);
        }

        private void RaisePropertyChanged<TResult>(Expression<Func<IPatternManager, TResult>> property)
        {
            this.PropertyChanged(this, new PropertyChangedEventArgs(Reflector<IPatternManager>.GetProperty(property).Name));
        }
    }
}
