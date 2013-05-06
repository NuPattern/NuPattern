using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using NuPattern.Diagnostics;
using NuPattern.Presentation;
using NuPattern.Reflection;
using NuPattern.Runtime.Automation;
using NuPattern.Runtime.Bindings;
using NuPattern.Runtime.Properties;
using NuPattern.Runtime.Store;
using NuPattern.VisualStudio;

namespace NuPattern.Runtime.UI.ViewModels
{
    /// <summary>
    /// Base view model for <see cref="ProductElement"/> elements.
    /// </summary>
    internal abstract class ProductElementViewModel : ViewModel<IProductElement>, IEditableObject, NuPattern.Runtime.UI.ViewModels.IProductElementViewModel
    {
        private static readonly string assemblyName = typeof(ProductElementViewModel).Assembly.GetName().Name;
        private static readonly string deleteIconPath = @"pack://application:,,,/" + assemblyName + @";component/Resources/CommandRemove.png";
        private static readonly string propertiesIconPath = @"pack://application:,,,/" + assemblyName + @";component/Resources/CommandProperties.png";

        private static readonly ITracer tracer = Tracer.Get<ProductElementViewModel>();

        private bool isEditing;
        private bool isExpanded;
        private bool isSelected;
        private string iconPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductElementViewModel"/> class.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Base class validates")]
        protected ProductElementViewModel(IProductElement model, SolutionBuilderContext context)
            : base(model)
        {
            Guard.NotNull(() => context, context);

            this.Context = context;
            this.Nodes = new ObservableCollection<ProductElementViewModel>();
            this.MenuOptions = new ObservableCollection<MenuOptionViewModel>();

            this.InitializeCommands();
            this.CreateStandardMenuOptions();

            model.PropertyChanged += this.OnElementPropertyChanged;

            if (model.Info != null)
            {
                this.Nodes.CollectionChanged += this.OnNodesCollectionChanged;

                this.CreateAutomationMenuOptions();
            }
        }

        /// <summary>
        /// Gets the add element command.
        /// </summary>
        public System.Windows.Input.ICommand AddElementCommand { get; private set; }

        /// <summary>
        /// Gets the extension point to instanciate.
        /// </summary>
        public System.Windows.Input.ICommand AddExtensionCommand { get; private set; }


        /// <summary>
        /// Gets the solution builder context.
        /// </summary>
        public ISolutionBuilderContext ContextViewModel
        {
            get { return this.Context; }
        }

        /// <summary>
        /// Gets the solution builder context.
        /// </summary>
        public SolutionBuilderContext Context { get; private set; }

        /// <summary>
        /// Gets the delete command.
        /// </summary>
        public System.Windows.Input.ICommand DeleteCommand { get; private set; }

        /// <summary>
        /// Gets the rename command.
        /// </summary>
        public System.Windows.Input.ICommand RenameCommand { get; private set; }

        /// <summary>
        /// Gets the icon path.
        /// </summary>
        public string IconPath
        {
            get
            {
                return this.iconPath;
            }

            set
            {
                if (this.iconPath != value)
                {
                    this.iconPath = value;
                    this.OnPropertyChanged(() => this.IconPath);
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is editing.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is editing; otherwise, <c>false</c>.
        /// </value>
        public bool IsEditing
        {
            get { return this.isEditing; }
            set
            {
                this.isEditing = value;
                this.OnPropertyChanged(() => this.IsEditing);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is expanded.
        /// </summary>
        /// <value>
        /// Returns <c>true</c> if this instance is expanded; otherwise, <c>false</c>.
        /// </value>
        public bool IsExpanded
        {
            get { return this.isExpanded; }
            set
            {
                if (this.isExpanded != value)
                {
                    this.isExpanded = value;
                    this.OnPropertyChanged(() => this.IsExpanded);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is selected.
        /// </summary>
        /// <value>
        /// Returns <c>true</c> if this instance is selected; otherwise, <c>false</c>.
        /// </value>
        public bool IsSelected
        {
            get { return this.isSelected; }
            set
            {
                if (this.isSelected != value)
                {
                    this.isSelected = value;
                    this.OnPropertyChanged(() => this.IsSelected);
                    if (value)
                    {
                        this.Context.SetSelected(this);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the underlying model.
        /// </summary>
        public new IProductElement Model
        {
            get { return (IProductElement)base.Model; }
        }

        /// <summary>
        /// Gets the child nodes.
        /// </summary>
        public ObservableCollection<IProductElementViewModel> NodesViewModel 
        { 
            get { return new ObservableCollection<IProductElementViewModel> (this.Nodes); } 
        }

        /// <summary>
        /// Gets the child nodes.
        /// </summary>
        public ObservableCollection<ProductElementViewModel> Nodes { get; private set; }

        /// <summary>
        /// Gets the menu options.
        /// </summary>
        public ObservableCollection<MenuOptionViewModel> MenuOptions { get; private set; }

        /// <summary>
        /// Gets the parent node.
        /// </summary>
        public ProductElementViewModel ParentNode { get; private set; }

        /// <summary>
        /// Gets the show properties command.
        /// </summary>
        public System.Windows.Input.ICommand ShowPropertiesCommand { get; private set; }

        /// <summary>
        /// Gets the element container. It is the place where the children are added.
        /// </summary>
        internal abstract IElementContainer ElementContainer { get; }

        /// <summary>
        /// Begins an edit on an object.
        /// </summary>
        public void BeginEdit()
        {
            this.IsEditing = true;
        }

        /// <summary>
        /// Discards changes since the last <see cref="BeginEdit"/> call.
        /// </summary>
        public void CancelEdit()
        {
            this.IsEditing = false;
        }

        /// <summary>
        /// Pushes changes since the last <see cref="BeginEdit"/> call into the underlying object.
        /// </summary>
        public void EndEdit()
        {
            this.IsEditing = false;
        }

        /// <summary>
        /// Determines whether the user can delete this instance.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the user can delete this instance; otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool CanDeleteInstance()
        {
            return true;
        }

        /// <summary>
        /// Determines whether an 'Add' menu can be created for the element info
        /// </summary>
        /// <returns>
        /// <c>true</c> if the menu can be created; otherwise <c>false</c>.
        /// </returns>
        protected virtual bool CanCreateAddMenu(IPatternElementInfo info)
        {
            // Only hide for elements/collections
            var abstractElementInfo = info as IAbstractElementInfo;
            if (abstractElementInfo != null)
            {
                return abstractElementInfo.IsVisible && abstractElementInfo.AllowAddNew;
            }

            return true;
        }

        /// <summary>
        /// Determines whether the user can add a new instance of the given info.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the user can add a new instance; otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool CanAddNewInstance(IPatternElementInfo info)
        {
            // Only show menu for elements/collections
            var abstractElementInfo = info as IAbstractElementInfo;
            if (abstractElementInfo != null)
            {
                if (abstractElementInfo.IsVisible && abstractElementInfo.AllowAddNew)
                {
                    var cardinality = abstractElementInfo.Cardinality;
                    switch (cardinality)
                    {
                        case Cardinality.OneToMany:
                        case Cardinality.ZeroToMany:
                            return true;

                        default:
                            //case Cardinality.OneToOne:
                            //case Cardinality.ZeroToOne:
                            return (this.ElementContainer.Elements.Count(e => e.Info == abstractElementInfo) == 0);
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Creates the add menu options.
        /// </summary>
        /// <returns>The entry point in the menu option.</returns>
        protected MenuOptionViewModel CreateAddMenuOptions()
        {
            var options = this.GetCurrentAddOptions();
            var addOption = new MenuOptionViewModel(Resources.ProductElementViewModel_AddMenuText, options)
            {
                GroupIndex = 1
            };

            this.MenuOptions.Add(addOption);
            return addOption;
        }

        /// <summary>
        /// Deletes this instance.
        /// </summary>
        protected virtual void Delete()
        {
            if (this.Context.UserMessageService.PromptWarning(string.Format(
                CultureInfo.CurrentCulture,
                Resources.SolutionBuilder_ConfirmDelete,
                this.Model.InstanceName)))
            {
                var ex = NuPattern.VisualStudio.TraceSourceExtensions.Shield(tracer,
                    () => this.Model.Delete(),
                    Resources.ProductViewModel_FailedToDeleteProduct);

                if (ex != null)
                    this.Context.UserMessageService.ShowError(ex.Message);
                else if (this.ParentNode != null)
                    this.ParentNode.Nodes.Remove(this);
            }
        }

        /// <summary>
        /// Gets the current add options.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Not appropriate.")]
        protected IEnumerable<MenuOptionViewModel> GetCurrentAddOptions()
        {
            return this.ElementContainer.Info.Elements
                .Where(e => CanCreateAddMenu(e))
                .Select(e => new MenuOptionViewModel(e.DisplayName, this.AddElementCommand)
                {
                    Model = e
                })
                .Concat(this.GetExtensionsMenuOptions(this.ElementContainer.Info.ExtensionPoints))
                .OrderBy(m => m.Caption);
        }

        private bool CanCreateExtension(ExtensionState state)
        {
            var info = state.ExtensionPoint;
            var extensionId = info.RequiredExtensionPointId;
            var extensions = this.ElementContainer.Extensions;

            if ((info.Cardinality != Cardinality.OneToOne &&
                 info.Cardinality != Cardinality.ZeroToOne) ||
                !extensions.Any(p => p.Info.ProvidedExtensionPoints.Any(ep => ep.ExtensionPointId == extensionId)))
            {
                return state.ConditionBindings.All(b => b.Evaluate(state.BindingContext) && b.Value.Evaluate());
            }

            return false;
        }

        private void CreateAutomationMenuOptions()
        {
            this.MenuOptions.AddRange(
                this.Model.AutomationExtensions
                    .Where(a => a is IAutomationMenuCommand)
                    .Select(a => new AutomationMenuOptionViewModel(a))
                    .OrderBy(o => o.Caption));
        }

        private void CreateStandardMenuOptions()
        {
            this.MenuOptions.Add(
                new MenuOptionViewModel(Resources.ProductElementViewModel_DeleteMenuText, this.DeleteCommand)
                {
                    GroupIndex = 3,
                    ImagePath = deleteIconPath,
                    IconType = IconType.Image,
                    InputGestureText = Resources.ProductElementViewModel_DeleteInputGestureText
                });

            this.MenuOptions.Add(
                new MenuOptionViewModel(Resources.ProductElementViewModel_RenameMenuText, this.RenameCommand)
                {
                    GroupIndex = 3,
                    InputGestureText = Resources.ProductElementViewModel_RenameInputGestureText
                });

            this.MenuOptions.Add(
                new MenuOptionViewModel(Resources.ProductElementViewModel_PropertiesMenuText, this.ShowPropertiesCommand)
                {
                    GroupIndex = 4,
                    ImagePath = propertiesIconPath,
                    IconType = IconType.Image,
                    InputGestureText = Resources.ProductElementViewModel_PropertiesInputGestureText
                });
        }

        private void AddProductElement(IPatternElementInfo info)
        {
            tracer.ShieldUI(
                () =>
                {
                    var viewModel = this.CreateNewNodeViewModel(info);
                    var view = this.Context.NewNodeDialogFactory(viewModel);
                    if (view.ShowDialog().GetValueOrDefault())
                    {
                        using (new MouseCursor(System.Windows.Input.Cursors.Wait))
                        {
                            var element = Factory.CreateElement(this.ElementContainer, info, viewModel.InstanceName);
                            this.Select(element);
                        }
                    }
                },
                Resources.ProductElementViewModel_ElementCreationFailed,
                info.DisplayName);
        }

        private AddNewNodeViewModel CreateNewNodeViewModel(IPatternElementInfo info)
        {
            return new AddNewNodeViewModel(this.Nodes.Select(x => x.Model), info, this.Context.UserMessageService);
        }

        private IEnumerable<MenuOptionViewModel> GetExtensionsMenuOptions(IEnumerable<IExtensionPointInfo> extensions)
        {
            return extensions.Select(e =>
                new MenuOptionViewModel(
                    e.DisplayName,
                    this.Context.PatternManager.GetCandidateExtensionPoints(e.RequiredExtensionPointId)
                        .Select(info =>
                            new MenuOptionViewModel(info.Schema.Pattern.DisplayName, this.AddExtensionCommand)
                            {
                                Model = new ExtensionState(info.Schema.Pattern, e, this.Context.BindingFactory)
                            })));
        }

        private void InitializeCommands()
        {
            this.AddElementCommand = new RelayCommand<IPatternElementInfo>(
                e => this.AddProductElement(e),
                e => e != null && this.CanAddNewInstance(e));
            this.AddExtensionCommand = new RelayCommand<ExtensionState>(
                i => this.AddProductElement(i.ExtendingProduct),
                i => i != null && this.CanCreateExtension(i));

            this.ShowPropertiesCommand = new RelayCommand(this.Context.ShowProperties);
            this.DeleteCommand = new RelayCommand(this.Delete, this.CanDeleteInstance);
            if (this.Context.ViewModel != null)
            {
                this.RenameCommand = new RelayCommand(this.Context.ViewModel.BeginEditNode, this.Context.ViewModel.CanBeginEditNode);
            }
        }

        private void OnNodesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (ProductElementViewModel propertyContainer in e.NewItems)
                {
                    propertyContainer.ParentNode = this;
                }
            }

            if (e.OldItems != null)
            {
                foreach (ProductElementViewModel propertyContainer in e.OldItems)
                {
                    propertyContainer.ParentNode = null;
                }
            }
        }

        private void Select(IProductElement element)
        {
            var selection = this.Nodes.FirstOrDefault(x => x.Model == element);
            if (selection != null)
            {
                selection.IsSelected = true;
            }
        }

        private void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //TODO: Check if changed property is used by custom OrderingComparer (if any),
            //and if so, then refresh the parent element

            // Verify the element is using default ordering (i.e. by InstanceName)
            var element = sender as IProductElement;
            if (element.Info != null)
            {
                if (String.Equals(e.PropertyName, Reflector<IProductElement>.GetPropertyName(p => p.InstanceName), StringComparison.OrdinalIgnoreCase))
                {
                    var containedElement = element.Info as IContainedElementInfo;
                    if (containedElement != null)
                    {
                        if (containedElement.IsDefaultOrderComparer())
                        {
                            PreserveSelection(() =>
                            {
                                if (this.ParentNode != null)
                                {
                                    this.ParentNode.Reorder();
                                }
                                else //Top level node
                                {
                                    this.Context.ViewModel.Reorder();
                                }
                            });
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Restores the selection of the current element after performing an operation,
        /// if the current element was selected before the operation.
        /// </summary>
        private void PreserveSelection(Action operation)
        {
            var isSelected = this.IsSelected;

            operation();

            if (isSelected)
            {
                if (this.ParentNode != null)
                {
                    this.ParentNode.Select(this.Model);
                }
                else
                {
                    this.Context.ViewModel.Select(this.Model);
                }
            }
        }

        /// <summary>
        /// Reorders the child nodes
        /// </summary>
        internal void Reorder()
        {
            // Fetching the AllElements property reorders elements automatically
            var elements = this.ElementContainer.AllElements;
            this.OnPropertyChanged(() => this.Nodes);
        }

        /// <summary>
        /// Refreshes child elements of node
        /// </summary>
        internal void Refresh()
        {
            this.Nodes.Clear();
            this.RenderHierarchyRecursive();
        }

        /// <summary>
        /// Adds all descendant nodes
        /// </summary>
        internal void RenderHierarchyRecursive()
        {
            this.AddChildNodes(this.ElementContainer.AllElements);
            foreach (var element in this.Nodes)
            {
                element.RenderHierarchyRecursive();
            }
        }

        internal void AddChildNodes(IEnumerable<IProductElement> elements)
        {
            // Filter out invisible abstract elements (only)
            var visibleNodes = from e in elements
                               let isAbstract = (e as IAbstractElement != null)
                               let hasInfo = (e.Info != null)
                               let abstractInfo = isAbstract && hasInfo ? ((e as IAbstractElement).Info) : null
                               where (!isAbstract)
                               || (isAbstract && !hasInfo)
                               || (isAbstract && hasInfo && abstractInfo.IsVisible)
                               select e;

            tracer.ShieldUI(() =>
            {
                this.Nodes.AddRange(
                    visibleNodes.Select(x => Factory.CreateViewModel(x, this.Context)));
            },
                Resources.SolutionBuilderViewModel_FailedToRenderElements,
                this.Model.InstanceName);
        }


        private class ExtensionState
        {
            private IBindingFactory bindingFactory;
            private IDynamicBinding<ICondition>[] conditionBindings;

            internal ExtensionState(
                IPatternInfo extendingProduct,
                IExtensionPointInfo extensionPoint,
                IBindingFactory bindingFactory)
            {
                this.ExtendingProduct = extendingProduct;
                this.ExtensionPoint = extensionPoint;
                this.bindingFactory = bindingFactory;

                this.BindingContext = bindingFactory.CreateContext();
                this.BindingContext.AddExportsFromInterfaces(extendingProduct);
                this.BindingContext.AddExportsFromInterfaces(extensionPoint);
            }

            public IDynamicBindingContext BindingContext { get; private set; }

            public IPatternInfo ExtendingProduct { get; private set; }

            public IExtensionPointInfo ExtensionPoint { get; private set; }

            public IEnumerable<IDynamicBinding<ICondition>> ConditionBindings
            {
                get
                {
                    return this.conditionBindings ??
                        (this.conditionBindings = this.ExtensionPoint.ConditionSettings
                            .Select(s => this.bindingFactory.CreateBinding<ICondition>(s)).ToArray());
                }
            }
        }

        private static class Factory
        {
            #region Element factories

            private static readonly KeyValuePair<Type, Func<IElementContainer, IPatternElementInfo, string, IProductElement>>[] elementFactories = new[]
            {
                new KeyValuePair<Type, Func<IElementContainer, IPatternElementInfo, string, IProductElement>>(typeof(IElementInfo), NewElement),
                new KeyValuePair<Type, Func<IElementContainer, IPatternElementInfo, string, IProductElement>>(typeof(ICollectionInfo), NewCollection),
                new KeyValuePair<Type, Func<IElementContainer, IPatternElementInfo, string, IProductElement>>(typeof(IPatternInfo), NewExtension)
            };

            #endregion

            #region ViewModel factories

            private static readonly KeyValuePair<Type, Func<IProductElement, SolutionBuilderContext, ProductElementViewModel>>[] vmFactories = new[]
            {
                new KeyValuePair<Type, Func<IProductElement, SolutionBuilderContext, ProductElementViewModel>>(
                    typeof(IAbstractElement),
                    (e, ctx) => new ElementViewModel((IAbstractElement)e, ctx)),

                new KeyValuePair<Type, Func<IProductElement, SolutionBuilderContext, ProductElementViewModel>>(
                    typeof(IProduct),
                    (e, ctx) => new ProductViewModel((IProduct)e, ctx))
            };

            #endregion

            internal static ProductElementViewModel CreateViewModel(IProductElement child, SolutionBuilderContext ctx)
            {
                var factory = FindFactory(child);
                return factory(child, ctx).With(vm => vm.IsExpanded = true);
            }

            private static Func<IProductElement, SolutionBuilderContext, ProductElementViewModel> FindFactory(IProductElement element)
            {
                return vmFactories.Where(f => f.Key.IsAssignableFrom(element.GetType())).Select(f => f.Value).First();
            }


            internal static IProductElement CreateElement(IElementContainer parent, IPatternElementInfo info, string name)
            {
                var factory = FindFactory(info);
                return factory(parent, info, name);
            }

            private static Func<IElementContainer, IPatternElementInfo, string, IProductElement> FindFactory(IPatternElementInfo info)
            {
                return elementFactories.Where(f => f.Key.IsAssignableFrom(info.GetType())).Select(f => f.Value).First();
            }

            private static ICollection NewCollection(IElementContainer parent, IPatternElementInfo info, string name)
            {
                return parent.CreateCollection(e =>
                {
                    e.DefinitionId = info.Id;
                    e.InstanceName = name;
                });
            }

            private static IElement NewElement(IElementContainer parent, IPatternElementInfo info, string name)
            {
                return parent.CreateElement(e =>
                {
                    e.DefinitionId = info.Id;
                    e.InstanceName = name;
                });
            }

            private static IProduct NewExtension(IElementContainer parent, IPatternElementInfo info, string name)
            {
                var productInfo = (IPatternInfo)info;
                return parent.CreateExtension(e =>
                {
                    e.DefinitionId = productInfo.Id;
                    e.InstanceName = name;
                    e.ExtensionId = productInfo.ExtensionId;
                });
            }
        }
    }
}