using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using NuPattern.Runtime.Guidance;
using NuPattern.Runtime.Guidance.UI.ViewModels;
using NuPattern.Runtime.Guidance.UI.Views;
using NuPattern.Runtime.Guidance.Workflow;
using NuPattern.VisualStudio;

namespace NuPattern.Runtime.Shell.ToolWindows
{
    /// <summary>
    /// Provides the Guidance Browser tool window.
    /// </summary>
    internal partial class GuidanceBrowserToolWindow
    {
        private GuidanceBrowserViewModel viewModel;

        [Import]
        private IUserMessageService UserMessageService { get; set; }

        [Import]
        private IUriReferenceService UriReferenceService { get; set; }

        [Import]
        private IGuidanceManager GuidanceManager { get; set; }

        [Import(typeof(SVsServiceProvider))]
        private IServiceProvider ServiceProvider { get; set; }

        protected override void Initialize()
        {
            base.Initialize();

            var componentModel = this.GetService<SComponentModel, IComponentModel>();
            componentModel.DefaultCompositionService.SatisfyImportsOnce(this);

            var context = new GuidanceBrowserContext
            {
                GuidanceManager = this.GuidanceManager,
                UriReferenceService = this.UriReferenceService,
                UserMessageService = this.UserMessageService,
            };

            this.viewModel = new GuidanceBrowserViewModel(context, this.ServiceProvider);
            this.Content = new GuidanceBrowserView { DataContext = this.viewModel };
        }

        /// <summary>
        /// Selects the current action
        /// </summary>
        internal void SelectCurrentNode(INode action)
        {
            this.viewModel.Node = action;
        }

        /// <summary>
        /// Opens the window, if not already opened.
        /// </summary>
        internal static void OpenWindow(IServiceProvider serviceProvider)
        {
            Guard.NotNull(() => serviceProvider, serviceProvider);

            var packageToolWindow = serviceProvider.GetService<IPackageToolWindow>();

            packageToolWindow.ShowWindow<GuidanceBrowserToolWindow>(true);
        }

        /// <summary>
        /// Hides the window, if it was automatically opened.
        /// </summary>
        internal static void HideWindow(IServiceProvider serviceProvider)
        {
            Guard.NotNull(() => serviceProvider, serviceProvider);

            var packageToolWindow = serviceProvider.GetService<IPackageToolWindow>();

            if (packageToolWindow.IsWindowVisible<GuidanceBrowserToolWindow>())
            {
                packageToolWindow.HideWindow<GuidanceBrowserToolWindow>();
            }
        }
    }
}
