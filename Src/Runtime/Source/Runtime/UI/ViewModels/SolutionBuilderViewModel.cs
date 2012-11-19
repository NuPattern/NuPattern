using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Data;
using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.Patterning.Runtime.Properties;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;

namespace Microsoft.VisualStudio.Patterning.Runtime.UI
{
    /// <summary>
    /// Provides a view model for the solution builder
    /// </summary>
    [CLSCompliant(false)]
    public class SolutionBuilderViewModel : ViewModel
    {
        private const string NewSolutionNamePrefix = "Solution";

        internal const string UsingGuidanceFeatureId = RuntimeShellInfo.VsixIdentifier;

        private static readonly ITraceSource tracer = Tracer.GetSourceFor<SolutionBuilderViewModel>();

        private ProductElementViewModel currentNode;
        private IServiceProvider serviceProvider;
        private ISolutionEvents solutionListener;
        private SolutionBuilderContext context;
        private bool isStateOpened;
        private bool isSolutionOpened;

        /// <summary>
        /// Initializes a new instance of the <see cref="SolutionBuilderViewModel"/> class.
        /// </summary>
        /// <param name="context">The pattern explorer context.</param>
        /// <param name="serviceProvider">The service provider.</param>
        public SolutionBuilderViewModel(SolutionBuilderContext context, IServiceProvider serviceProvider)
        {
            Guard.NotNull(() => context, context);
            Guard.NotNull(() => serviceProvider, serviceProvider);

            this.context = context;
            this.context.SolutionBuilder = this;
            this.serviceProvider = serviceProvider;

            this.context.SetSelected = p => this.CurrentNode = p;

            this.solutionListener = this.GetService<ISolutionEvents>();

            this.isSolutionOpened = this.solutionListener.IsSolutionOpened;
            this.isStateOpened = this.context.PatternManager.IsOpen || this.isSolutionOpened;

            this.Nodes = new ObservableCollection<ProductElementViewModel>();
            if (this.IsStateOpened)
            {
                this.Refresh();
            }

            this.solutionListener.SolutionOpened += this.OnSolutionOpened;
            this.solutionListener.SolutionClosed += this.OnSolutionClosed;

            context.PatternManager.IsOpenChanged += this.OnIsOpenChanged;
            context.PatternManager.ElementDeleted += this.OnElementDeleted;
            context.PatternManager.ElementCreated += this.OnElementCreated;

            this.InitializeCommands();
        }

        /// <summary>
        /// Event raised when the current node changes.
        /// </summary>
        public event EventHandler CurrentNodeChanged;

        /// <summary>
        /// Gets the activate command.
        /// </summary>
        public System.Windows.Input.ICommand ActivateCommand { get; private set; }

        /// <summary>
        /// Gets the add new pattern command.
        /// </summary>
        public System.Windows.Input.ICommand AddNewProductCommand { get; private set; }

        /// <summary>
        /// Gets the begin edit command.
        /// </summary>
        public System.Windows.Input.ICommand BeginEditCommand { get; private set; }

        /// <summary>
        /// Gets the cancel edit command.
        /// </summary>
        public System.Windows.Input.ICommand CancelEditCommand { get; private set; }

        /// <summary>
        /// Gets the collapse all command.
        /// </summary>
        public System.Windows.Input.ICommand CollapseAllCommand { get; private set; }

        /// <summary>
        /// Gets the end edit command.
        /// </summary>
        public System.Windows.Input.ICommand EndEditCommand { get; private set; }

        /// <summary>
        /// Gets the expand all command.
        /// </summary>
        public System.Windows.Input.ICommand ExpandAllCommand { get; private set; }

        /// <summary>
        /// Gets the guidance command.
        /// </summary>
        public System.Windows.Input.ICommand GuidanceCommand { get; private set; }

        /// <summary>
        /// Gets the create new solution command.
        /// </summary>
        public System.Windows.Input.ICommand CreateNewSolutionCommand { get; private set; }

        /// <summary>
        /// Gets the delete command.
        /// </summary>
        public System.Windows.Input.ICommand DeleteCommand { get; private set; }

        /// <summary>
        /// Gets the save command.
        /// </summary>
        public System.Windows.Input.ICommand SaveCommand { get; private set; }

        /// <summary>
        /// Gets the current node in the pattern explorer tree view.
        /// </summary>
        public ProductElementViewModel CurrentNode
        {
            get
            {
                return this.currentNode;
            }

            private set
            {
                if (this.currentNode != value)
                {
                    if (this.currentNode != null && this.currentNode.IsEditing)
                    {
                        this.currentNode.EndEdit();
                    }

                    this.currentNode = value;
                    this.OnCurrentNodeChanged();
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is solution opened.
        /// </summary>
        /// <value>
        /// Returns <c>true</c> if this instance is solution opened; otherwise, <c>false</c>.
        /// </value>
        public bool IsSolutionOpened
        {
            get
            {
                return this.isSolutionOpened;
            }

            private set
            {
                if (this.isSolutionOpened != value)
                {
                    this.isSolutionOpened = value;
                    this.OnPropertyChanged(() => this.IsSolutionOpened);
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether a product state opened.
        /// </summary>
        /// <value>Returns <c>true</c> if a product state is opened; otherwise, <c>false</c>.</value>
        public bool IsStateOpened
        {
            get
            {
                return this.isStateOpened;
            }

            private set
            {
                if (this.isStateOpened != value)
                {
                    this.isStateOpened = value;
                    this.OnPropertyChanged(() => this.IsStateOpened);
                }
            }
        }

        /// <summary>
        /// Gets the pattern nodes.
        /// </summary>
        public ObservableCollection<ProductElementViewModel> Nodes { get; private set; }


        private void ActivateNode()
        {
            this.context.PatternManager.ActivateElement(this.currentNode.Model);
        }

        private void AddNewProduct()
        {
            var ex = Extensibility.TracingExtensions.Shield(tracer,
                () =>
                {
                    var viewModel = new AddNewProductViewModel(this.context.PatternManager, this.context.UserMessageService);

                    var view = this.context.NewProductDialogFactory(viewModel);
                    if (view.ShowDialog().GetValueOrDefault())
                    {
                        using (new MouseCursor(System.Windows.Input.Cursors.Wait))
                        {
                            var product = this.context.PatternManager.CreateProduct(viewModel.CurrentToolkit, viewModel.ProductName);
                            this.Select(product);
                        }
                    }
                },
                Resources.SolutionBuilderViewModel_ProductInstantiationFailed);

            if (ex != null)
            {
                // If there was an explicit cancellation, show the author message, 
                // otherwise show a generic error message, compatible with existing behavior.
                if (ex is OperationCanceledException)
                    this.context.UserMessageService.ShowError(ex.Message);
                else
                    this.context.UserMessageService.ShowError(Resources.SolutionBuilderViewModel_ProductInstantiationFailed);
            }
        }

        internal void BeginEditNode()
        {
            var view = this.GetEditableCollectionView();
            if (view != null)
            {
                view.EditItem(this.currentNode);
            }
        }

        private void EndEditNode()
        {
            this.currentNode.EndEdit();
        }

        private void CancelEditNode()
        {
            var view = this.GetEditableCollectionView();
            if (view != null)
            {
                view.CancelEdit();
            }
        }

        internal bool CanBeginEditNode()
        {
            return this.currentNode != null && !this.currentNode.IsEditing;
        }

        private bool CanDeleteNode()
        {
            return this.currentNode != null && !this.currentNode.IsEditing && this.currentNode.DeleteCommand.CanExecute(null);
        }

        private bool CanFinishEditNode()
        {
            return this.currentNode != null && this.currentNode.IsEditing;
        }

        private bool CanShowGuidance()
        {
            var featureManager = this.serviceProvider.GetService<IFeatureManager>();
            if (featureManager != null)
            {
                return featureManager.IsGuidanceRegistered(UsingGuidanceFeatureId);
            }

            return false;
        }

        private bool CanShowCreateNewSolution()
        {
            return true;
        }

        private void ChangeIsExpanded(bool isExpanded)
        {
            foreach (var node in this.Nodes.Cast<ProductElementViewModel>().Traverse(n => n.Nodes))
            {
                node.IsExpanded = isExpanded;
            }
        }

        private ProductViewModel CreateViewModel(IProduct product)
        {
            var parent = new ProductViewModel(product, this.context)
            {
                IsExpanded = true
            };
            if (parent.Model.Info != null)
            {
                parent.RenderHierarchyRecursive();
            }

            return parent;
        }

        private void DeleteNode()
        {
            this.currentNode.DeleteCommand.Execute(null);
        }

        private IEditableCollectionView GetEditableCollectionView()
        {
            if (this.currentNode.ParentNode == null)
            {
                return CollectionViewSource.GetDefaultView(this.Nodes) as IEditableCollectionView;
            }

            return CollectionViewSource.GetDefaultView(this.currentNode.ParentNode.Nodes) as IEditableCollectionView;
        }

        private T GetService<T>()
        {
            var service = this.serviceProvider.GetService<T>();
            if (service == null)
            {
                throw new InvalidOperationException(string.Format(Resources.Culture, Resources.SolutionBuilderViewModel_ServiceNotFound, typeof(T)));
            }

            return service;
        }

        private void InitializeCommands()
        {
            this.AddNewProductCommand = new RelayCommand(this.AddNewProduct, () => this.IsSolutionOpened);
            this.GuidanceCommand = new RelayCommand(this.ShowGuidance, this.CanShowGuidance);
            this.SaveCommand = new RelayCommand(this.context.PatternManager.Save, () => this.context.PatternManager.IsOpen);
            this.ExpandAllCommand = new RelayCommand(() => this.ChangeIsExpanded(true), () => this.Nodes.Count > 0);
            this.CollapseAllCommand = new RelayCommand(() => this.ChangeIsExpanded(false), () => this.Nodes.Count > 0);
            this.CreateNewSolutionCommand = new RelayCommand(this.CreateNewSolution, this.CanShowCreateNewSolution);

            this.ActivateCommand = new RelayCommand(this.ActivateNode, () => this.currentNode != null);
            this.DeleteCommand = new RelayCommand(this.DeleteNode, this.CanDeleteNode);
            this.BeginEditCommand = new RelayCommand(this.BeginEditNode, this.CanBeginEditNode);
            this.CancelEditCommand = new RelayCommand(this.CancelEditNode, this.CanFinishEditNode);
            this.EndEditCommand = new RelayCommand(this.EndEditNode, this.CanFinishEditNode);
        }

        private void OnCurrentNodeChanged()
        {
            this.OnPropertyChanged(() => this.CurrentNode);
            if (this.CurrentNodeChanged != null)
            {
                this.CurrentNodeChanged(this, EventArgs.Empty);
            }
        }

        private void OnElementDeleted(object sender, ValueEventArgs<IProductElement> e)
        {
            var element = this.Nodes.Traverse(x => x.Nodes).FirstOrDefault(x => x.Model == e.Value);
            if (element != null)
            {
                if (element.ParentNode == null) // Product at the top level
                {
                    this.Nodes.Remove(element);
                }
                else
                {
                    element.ParentNode.Nodes.Remove(element);
                }
            }
        }

        private void OnElementCreated(object sender, ValueEventArgs<IProductElement> e)
        {
            if (e.Value.Parent != null)
            {
                var parent = this.Nodes
                    .Traverse(x => x.Nodes)
                    .FirstOrDefault(x => x.ElementContainer == e.Value.Parent);
                if (parent != null)
                {
                    parent.Reorder();
                    parent.AddChildNodes(new[] { e.Value });
                }
            }
            else // Product at the top level
            {
                var product = e.Value as IProduct;
                if (product != null)
                {
                    this.Reorder();
                    this.Nodes.Add(new ProductViewModel(product, this.context)
                    {
                        IsExpanded = true,
                        IsSelected = true
                    });
                }
            }
        }

        private void OnIsOpenChanged(object sender, EventArgs e)
        {
            if (this.context.PatternManager.IsOpen)
            {
                this.Refresh();
            }
            else
            {
                this.CurrentNode = null;
                this.Nodes.Clear();
            }

            this.IsStateOpened = this.IsStateOpened || this.IsSolutionOpened;
        }

        private void OnSolutionClosed(object sender, SolutionEventArgs e)
        {
            this.IsSolutionOpened = false;
            this.IsStateOpened = false;
        }

        private void OnSolutionOpened(object sender, SolutionEventArgs e)
        {
            this.IsSolutionOpened = true;
        }


        /// <summary>
        /// Reorders all top level nodes
        /// </summary>
        internal void Reorder()
        {
            // Fetching the Products property reorders products automatically
            var products = this.context.PatternManager.Products;
            this.OnPropertyChanged(() => this.Nodes);
        }

        /// <summary>
        /// Refreshes all top level nodes
        /// </summary>
        internal void Refresh()
        {
            this.CurrentNode = null;
            this.Nodes.Clear();
            this.Nodes.AddRange(this.context.PatternManager.Products.Select(p => this.CreateViewModel(p)));
        }

        internal void Select(IProductElement element)
        {
            var selection = this.Nodes.FirstOrDefault(x => x.Model == element);
            if (selection != null)
            {
                selection.IsSelected = true;
            }
        }

        private void ShowGuidance()
        {
            var featureManager = this.serviceProvider.GetService<IFeatureManager>();
            if (featureManager != null)
            {
                featureManager.ActivateSharedGuidanceWorkflow(this.serviceProvider, UsingGuidanceFeatureId);
            }
        }

        private void CreateNewSolution()
        {
            var dte = this.serviceProvider.GetService<EnvDTE.DTE>();
            if (dte != null)
            {
                //Close existing solution
                if (dte.Solution.IsOpen)
                {
                    dte.Solution.Close(true);
                }

                // Determine next available solution directory
                var defaultSaveLocation = dte.GetDefaultProjectSaveLocation();
                if (string.IsNullOrEmpty(defaultSaveLocation))
                {
                    throw new InvalidOperationException(Resources.SolutionBuilderViewModel_CreateNewSolution_FailedDirSearch);
                }

                var existingSolutionFolders = Directory.GetDirectories(defaultSaveLocation).Select(dir => new DirectoryInfo(dir).Name);
                var nextSolutionDir = UniqueNameGenerator.EnsureUnique(NewSolutionNamePrefix, existingSolutionFolders, true);

                // Create solution directory
                var solutionDir = Path.Combine(defaultSaveLocation, nextSolutionDir);
                if (!Directory.Exists(solutionDir))
                {
                    Directory.CreateDirectory(solutionDir);
                }

                // Save and Open new solution
                var solutionFullPath = Path.Combine(solutionDir, nextSolutionDir);
                try
                {
                    dte.Solution.Create(solutionDir, nextSolutionDir);
                    dte.Solution.SaveAs(solutionFullPath);
                }
                catch (COMException)
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, 
                        Resources.SolutionBuilderViewModel_CreateNewSolution_FailedCreate, solutionDir));
                }
            }
        }
    }
}