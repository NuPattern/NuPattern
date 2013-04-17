using System;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using NuPattern.Runtime.Guidance;
using NuPattern.Runtime.Guidance.UI.ViewModels;
using NuPattern.Runtime.Guidance.UI.Views;
using NuPattern.Runtime.Shell.Properties;
using NuPattern.VisualStudio;

namespace NuPattern.Runtime.Shell.ToolWindows
{
    /// <summary>
    /// Provides the Guidance Explorer tool window.
    /// </summary>
    [Guid("6A2D1053-BEEB-4FDF-9FB0-133CE1FE6D25")]
    internal class GuidanceExplorerToolWindow : ToolWindowPane
    {
        private SelectionContainer selectionContainer;
        private GuidanceWorkflowViewModel viewModel;
        private IVsTrackSelectionEx trackSelection;

        public object WorkflowMediator { get; set; }

        /// <summary>
        /// Creates a new instance of the <see cref="GuidanceExplorerToolWindow"/> class.
        /// </summary>
        public GuidanceExplorerToolWindow() :
            base(null)
        {
            this.Caption = Resources.GuidanceExplorerToolWindow_WindowTitle;

            this.BitmapResourceID = 302;
            this.BitmapIndex = 0;
        }

        [Import]
        private IUserMessageService UserMessageService { get; set; }

        [Import]
        private IFeatureManager FeatureManager { get; set; }

        [Import(typeof(SVsServiceProvider))]
        private IServiceProvider ServiceProvider { get; set; }

        public override void OnToolWindowCreated()
        {
            base.OnToolWindowCreated();

            this.selectionContainer = new SelectionContainer();
            //this.viewModel.CurrentNodeChanged += this.OnCurrentNodeChanged;
        }

        protected override void Initialize()
        {
            base.Initialize();

            var componentModel = this.GetService<SComponentModel, IComponentModel>();
            componentModel.DefaultCompositionService.SatisfyImportsOnce(this);

            this.trackSelection = this.GetService<SVsTrackSelectionEx, IVsTrackSelectionEx>();
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(this.trackSelection.OnSelectChange(this.selectionContainer));

            var context = new GuidanceWorkflowContext
            {
                FeatureExtension = this.FeatureManager.ActiveFeature,
                UserMessageService = this.UserMessageService,
            };

            this.viewModel = new GuidanceWorkflowViewModel(context, this.ServiceProvider);
            this.Content = new GuidanceWorkflowView { DataContext = this.viewModel };
        }
    }
}
