using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using NuPattern.Runtime.Properties;
using NuPattern.VisualStudio;

namespace NuPattern.Runtime.UI.ViewModels
{
    /// <summary>
    /// Provides a view model for a pattern instance in the solution builder view.
    /// </summary>
    internal class ProductViewModel : ProductElementViewModel
    {
        private const string IconEnabledPath = "../../Resources/NodeProductDefault.png";
        private const string IconUninstalledPath = "../../Resources/NodeProductVersionNotFound.png";

        private static readonly ITraceSource tracer = Tracer.GetSourceFor<ProductViewModel>();

        private MenuOptionViewModel viewsOption;
        private MenuOptionViewModel addOption;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductViewModel"/> class.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public ProductViewModel(IProduct product, SolutionBuilderContext context)
            : base(product, context)
        {
            this.InitializeCommands();

            if (product.Info != null)
            {
                this.IconPath = IconEnabledPath;
                this.HasSingleView = product.Views.Count() == 1;
                this.CreateViewMenuOptions();
                this.addOption = this.CreateAddMenuOptions();
            }
            else
            {
                this.IconPath = IconUninstalledPath;
                this.HasSingleView = true;
            }
        }

        /// <summary>
        /// Gets the change view command.
        /// </summary>
        public System.Windows.Input.ICommand ChangeViewCommand { get; private set; }

        /// <summary>
        /// Gets the current view.
        /// </summary>
        public IView CurrentView
        {
            get
            {
                return this.Model.CurrentView;
            }

            private set
            {
                if (this.Model.CurrentView != value)
                {
                    this.Model.CurrentView = value;

                    this.OnPropertyChanged(() => this.CurrentView);
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has a single view.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has a single view; otherwise, <c>false</c>.
        /// </value>
        public bool HasSingleView { get; private set; }

        /// <summary>
        /// Gets the underlying model.
        /// </summary>
        public new IProduct Model
        {
            get { return (IProduct)base.Model; }
        }

        /// <summary>
        /// Gets the element container. It is the place where the children are added.
        /// </summary>
        /// <value></value>
        internal override IElementContainer ElementContainer
        {
            get { return this.CurrentView; }
        }

        /// <summary>
        /// Deletes this instance.
        /// </summary>
        protected override void Delete()
        {
            if (this.ParentNode == null)
            {
                if (this.Context.UserMessageService.PromptWarning(string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.SolutionBuilder_ConfirmDelete,
                    this.Model.InstanceName)))
                {

                    var ex = tracer.Shield(
                        () => this.Context.PatternManager.DeleteProduct(this.Model),
                        Resources.ProductViewModel_FailedToDeleteProduct);

                    if (ex != null)
                        this.Context.UserMessageService.ShowError(ex.Message);
                }
            }
            else
            {
                base.Delete();
            }
        }

        private void CreateViewMenuOptions()
        {
            if (!this.HasSingleView)
            {
                var views = this.Model.Views
                    .Where(x => x.Info != null)
                    .OrderBy(v => v.Info.DisplayName)
                    .Select(v =>
                        new MenuOptionViewModel(v.Info.DisplayName, this.ChangeViewCommand)
                        {
                            Model = v,
                            IsSelected = (v == this.Model.CurrentView),
                            IconType = IconType.SingleSelect,
                            IsVisible = v.Info.IsVisible,
                        });

                this.viewsOption = new MenuOptionViewModel(Resources.ProductViewModel_ViewsMenuText, views)
                {
                    GroupIndex = 1,
                };

                this.MenuOptions.Add(viewsOption);
            }
        }

        private void ChangeSelectedView(IView view)
        {
            var previousOption = this.viewsOption.MenuOptions.FirstOrDefault(o => o.IsSelected);
            if (previousOption != null)
            {
                previousOption.IsSelected = false;
            }

            var selectedOption = this.viewsOption.MenuOptions.FirstOrDefault(o => o.Model == view);
            if (selectedOption != null)
            {
                selectedOption.IsSelected = true;
                this.CurrentView = view;

                this.Nodes.Clear();
                this.RenderHierarchyRecursive();
                this.RefreshAddMenuOptions();
            }
        }

        private void InitializeCommands()
        {
            this.ChangeViewCommand = new Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.RelayCommand<IView>(v => this.ChangeSelectedView(v));
        }

        private void RefreshAddMenuOptions()
        {
            this.addOption.MenuOptions.Clear();
            this.addOption.MenuOptions.AddRange(this.GetCurrentAddOptions());
        }
    }
}