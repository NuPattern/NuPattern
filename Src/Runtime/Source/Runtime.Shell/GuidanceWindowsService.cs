using System;
using System.ComponentModel.Composition;
using NuPattern.Runtime.Guidance;
using NuPattern.Runtime.Shell.ToolWindows;
using NuPattern.VisualStudio;

namespace NuPattern.Runtime.Shell
{
    [Export(typeof(IGuidanceWindowsService))]
    internal class GuidanceWindowsService : IGuidanceWindowsService
    {
        public void ShowGuidanceExplorer(IServiceProvider serviceProvider)
        {
            Guard.NotNull(() => serviceProvider, serviceProvider);

            var packageToolWindow = serviceProvider.GetService<IPackageToolWindow>();

            if (!packageToolWindow.IsWindowVisible<GuidanceExplorerToolWindow>())
            {
                packageToolWindow.ShowWindow<GuidanceExplorerToolWindow>();
            }
        }

        public void ShowGuidanceBrowser(IServiceProvider serviceProvider)
        {
            Guard.NotNull(() => serviceProvider, serviceProvider);

            var packageToolWindow = serviceProvider.GetService<IPackageToolWindow>();

            if (!packageToolWindow.IsWindowVisible<GuidanceBrowserToolWindow>())
            {
                packageToolWindow.ShowWindow<GuidanceBrowserToolWindow>();
            }
        }

        public void HideGuidanceWindows(IServiceProvider serviceProvider)
        {
            Guard.NotNull(() => serviceProvider, serviceProvider);

            var packageToolWindow = serviceProvider.GetService<IPackageToolWindow>();

            packageToolWindow.HideWindow<GuidanceExplorerToolWindow>();
            packageToolWindow.HideWindow<GuidanceBrowserToolWindow>();
        }
    }
}
