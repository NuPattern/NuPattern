using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using NuPattern.Runtime.Guidance;
using NuPattern.Runtime.Guidance.UI;
using NuPattern.Runtime.Guidance.UI.ViewModels;
using NuPattern.Runtime.Guidance.UI.Views;
using NuPattern.Runtime.Guidance.Workflow;
using NuPattern.VisualStudio;

namespace NuPattern.Runtime.Shell.ToolWindows
{
    /// <summary>
    /// Provides the Guidance Explorer tool window.
    /// </summary>
    internal partial class GuidanceExplorerToolWindow
    {
        private SelectionContainer selectionContainer;
        private GuidanceExplorerViewModel viewModel;
        private IVsTrackSelectionEx trackSelection;

        [Import]
        private IUserMessageService UserMessageService { get; set; }

        [Import]
        private IGuidanceManager GuidanceManager { get; set; }

        [Import(typeof(SVsServiceProvider))]
        private IServiceProvider ServiceProvider { get; set; }

        public override void OnToolWindowCreated()
        {
            base.OnToolWindowCreated();

            this.selectionContainer = new SelectionContainer();
            this.viewModel.CurrentNodeChanged += this.OnCurrentNodeChanged;
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
                GuidanceManager = this.GuidanceManager,
                Extension = this.GuidanceManager.ActiveGuidanceExtension,
                UserMessageService = this.UserMessageService,
            };

            this.viewModel = new GuidanceExplorerViewModel(context, this.ServiceProvider);
            this.Content = new GuidanceExplorerView { DataContext = this.viewModel };
        }


        /// <summary>
        /// Opens the window, if not already opened.
        /// </summary>
        internal static void OpenWindow(IServiceProvider serviceProvider)
        {
            Guard.NotNull(() => serviceProvider, serviceProvider);

            var packageToolWindow = serviceProvider.GetService<IPackageToolWindow>();
            packageToolWindow.ShowWindow<GuidanceExplorerToolWindow>(true);
        }

        /// <summary>
        /// Hides the window, if it was automatically opened.
        /// </summary>
        internal static void HideWindow(IServiceProvider serviceProvider)
        {
            Guard.NotNull(() => serviceProvider, serviceProvider);

            var packageToolWindow = serviceProvider.GetService<IPackageToolWindow>();

            if (packageToolWindow.IsWindowVisible<GuidanceExplorerToolWindow>())
            {
                packageToolWindow.HideWindow<GuidanceExplorerToolWindow>();
            }
        }

        private void OnCurrentNodeChanged(object sender, NodeChangedEventArgs e)
        {
            var currentAction = this.viewModel.CurrentWorkflow == null ? null : this.viewModel.CurrentWorkflow.Model.FocusedAction;
            if (currentAction != null)
            {
                SelectItem(new[] { currentAction });
                BrowseNode(currentAction);
            }

            //Ensure GuidanceBrowser tool window is open
            GuidanceBrowserToolWindow.OpenWindow(this);
        }

        private void SelectItem(object[] selectedObjects)
        {
            this.selectionContainer.SelectableObjects = selectedObjects;
            this.selectionContainer.SelectedObjects = selectedObjects;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(this.trackSelection.OnSelectChange(this.selectionContainer));
        }

        private void BrowseNode(INode node)
        {
            var packageToolWindow = this.ServiceProvider.GetService<IPackageToolWindow>();
            var toolWindow = packageToolWindow.GetWindow<GuidanceBrowserToolWindow>();
            if (toolWindow != null)
            {
                var browser = toolWindow as GuidanceBrowserToolWindow;
                if (browser != null)
                {
                    browser.SelectCurrentNode(node);
                }
            }
        }
    }
}
