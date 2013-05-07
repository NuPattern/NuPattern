using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Shell;
using NuPattern.Runtime.Shell.ToolWindows;

namespace NuPattern.Runtime.Shell
{
    [Export(typeof(IPatternWindows))]
    internal class PatternWindowsService : IPatternWindows
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

        /// <summary>
        /// TODO: this method needs replacing with a better way to place buttons on the toolbar of solution builder.
        /// </summary>
        public ToolWindowPane ShowSolutionBuilder(IServiceProvider serviceProvider)
        {
            Guard.NotNull(() => serviceProvider, serviceProvider);

            return SolutionBuilderToolWindow.OpenWindow(serviceProvider);
        }

        /// <summary>
        /// TODO: this method needs replacing with better way to get accecss to the ViewModel so that toolkits can customize how nodes are displayed.
        /// </summary>
        public Presentation.ViewModel GetSolutionBuilderViewModel(IServiceProvider serviceProvider)
        {
            Guard.NotNull(() => serviceProvider, serviceProvider);
            var packageToolWindow = serviceProvider.GetService<IPackageToolWindow>();

            var window = packageToolWindow.ShowWindow<SolutionBuilderToolWindow>(true);
            return window.ViewModel;
        }
    }
}
