using System;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.Patterning.Runtime.Shell.Properties;
using Microsoft.VisualStudio.Patterning.Runtime.UI;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;

namespace Microsoft.VisualStudio.Patterning.Runtime.Shell
{
    /// <summary>
    /// Provides the Pattern Explorer tool window.
    /// </summary>
    [Guid("c44b2e95-86f4-40dd-8fc8-bbc9725ea86b")]
    [CLSCompliant(false)]
    public class SolutionBuilderToolWindow : ToolWindowPane
    {
        private SelectionContainer selectionContainer;
        private SolutionBuilderViewModel viewModel;
        private IVsTrackSelectionEx trackSelection;

        /// <summary>
        /// Initializes a new instance of the <see cref="SolutionBuilderToolWindow"/> class.
        /// </summary>
        public SolutionBuilderToolWindow() :
            base(null)
        {
            this.Caption = Resources.ExplorerWindowTitle;

            this.BitmapResourceID = 301;
            this.BitmapIndex = 0;
        }

        [Import]
        private IBindingFactory BindingFactory { get; set; }

        [Import]
        private IUserMessageService UserMessageService { get; set; }

        [Import]
        private IPatternManager PatternManager { get; set; }

        [Import(typeof(SVsServiceProvider))]
        private IServiceProvider ServiceProvider { get; set; }

        /// <summary>
        /// This method can be overridden by the derived class to execute any code that must run after the creation of
        /// <see cref="IVsWindowFrame"/>.
        /// </summary>
        public override void OnToolWindowCreated()
        {
            base.OnToolWindowCreated();

            this.selectionContainer = new SelectionContainer();
            this.viewModel.CurrentNodeChanged += this.OnCurrentNodeChanged;
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            var componentModel = this.GetService<SComponentModel, IComponentModel>();
            componentModel.DefaultCompositionService.SatisfyImportsOnce(this);

            this.trackSelection = this.GetService<SVsTrackSelectionEx, IVsTrackSelectionEx>();
            ErrorHandler.ThrowOnFailure(this.trackSelection.OnSelectChange(this.selectionContainer));

            var shell = this.GetService<SVsUIShell, IVsUIShell>();

            var context = new SolutionBuilderContext
            {
                PatternManager = this.PatternManager,
                UserMessageService = this.UserMessageService,
                BindingFactory = this.BindingFactory,
                NewProductDialogFactory = ctx => shell.CreateDialog<AddNewProductView>(ctx),
                NewNodeDialogFactory = ctx => shell.CreateDialog<AddNewNodeView>(ctx),
                ShowProperties = this.ShowProperties
            };

            this.viewModel = new SolutionBuilderViewModel(context, this.ServiceProvider);
            this.Content = new SolutionBuilderView { DataContext = this.viewModel };
        }

        private void OnCurrentNodeChanged(object sender, EventArgs e)
        {
            var selectedObjects = this.viewModel.CurrentNode == null ? new object[0] : new[] { this.viewModel.CurrentNode.Model };

            this.selectionContainer.SelectableObjects = selectedObjects;
            this.selectionContainer.SelectedObjects = selectedObjects;
            ErrorHandler.ThrowOnFailure(this.trackSelection.OnSelectChange(this.selectionContainer));
        }

        private void ShowProperties()
        {
            var dte = this.GetService<EnvDTE.DTE>();

            var window = dte.Windows.Item(EnvDTE.Constants.vsWindowKindProperties);
            if (window != null)
            {
                window.Activate();
            }
        }
    }
}