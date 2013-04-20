using System;
using System.ComponentModel.Composition;
using NuPattern.Runtime.Guidance;
using NuPattern.Runtime.Shell.ToolWindows;

namespace NuPattern.Runtime.Shell
{
    [Export(typeof(IGuidanceWindowsService))]
    internal class GuidanceWindowsService : IGuidanceWindowsService
    {
        public void ShowGuidanceExplorer(IServiceProvider serviceProvider)
        {
            Guard.NotNull(() => serviceProvider, serviceProvider);

            GuidanceExplorerToolWindow.OpenWindow(serviceProvider);
        }

        public void ShowGuidanceBrowser(IServiceProvider serviceProvider)
        {
            Guard.NotNull(() => serviceProvider, serviceProvider);

            GuidanceBrowserToolWindow.OpenWindow(serviceProvider);
        }

        public void HideGuidanceWindows(IServiceProvider serviceProvider)
        {
            Guard.NotNull(() => serviceProvider, serviceProvider);

            GuidanceExplorerToolWindow.HideWindow(serviceProvider);
            GuidanceBrowserToolWindow.HideWindow(serviceProvider);
        }
    }
}
