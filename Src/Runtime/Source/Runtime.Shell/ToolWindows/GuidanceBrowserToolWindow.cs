using System;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.UI.ViewModels;
using NuPattern.Runtime.Shell.Properties;
using NuPattern.VisualStudio;

namespace NuPattern.Runtime.Shell.ToolWindows
{
    /// <summary>
    /// Provides the Guidance Browser tool window.
    /// </summary>
    [Guid("167f0a44-f79b-4007-a084-8ecc1457776e")]
    internal class GuidanceBrowserToolWindow : ToolWindowPane
    {
        private GuidanceBrowserViewModel viewModel;

        /// <summary>
        /// Creates a new instance of the <see cref="GuidanceExplorerToolWindow"/> class.
        /// </summary>
        public GuidanceBrowserToolWindow() :
            base(null)
        {
            this.Caption = Resources.GuidanceBrowserToolWindow_WindowTitle;

            this.BitmapResourceID = 303;
            this.BitmapIndex = 0;
        }

        [Import]
        private IUserMessageService UserMessageService { get; set; }

        [Import]
        private IUriReferenceService UriReferenceService { get; set; }

        [Import]
        private IFeatureManager FeatureManager { get; set; }

        [Import(typeof(SVsServiceProvider))]
        private IServiceProvider ServiceProvider { get; set; }

        protected override void Initialize()
        {
            base.Initialize();

            var componentModel = this.GetService<SComponentModel, IComponentModel>();
            componentModel.DefaultCompositionService.SatisfyImportsOnce(this);

            var context = new GuidanceBrowserContext
            {
                FeatureManager = this.FeatureManager,
                UriReferenceService = this.UriReferenceService,
                UserMessageService = this.UserMessageService,
            };

            this.viewModel = new GuidanceBrowserViewModel(context, this.ServiceProvider);
            this.Content = new GuidanceBrowserView { DataContext = this.viewModel };
        }
    }
}
