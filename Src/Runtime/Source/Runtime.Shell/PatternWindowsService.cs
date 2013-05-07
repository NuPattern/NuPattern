using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Shell;
using NuPattern.Runtime.Shell.ToolWindows;
using NuPattern.Runtime.UI.ViewModels;

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
        /// TODO: this method needs replacing with one that return void, and a better way to place buttons on the toolbar of solution builder.
        /// </summary>
        public ToolWindowPane ShowSolutionBuilder(IServiceProvider serviceProvider)
        {
            Guard.NotNull(() => serviceProvider, serviceProvider);

            return SolutionBuilderToolWindow.OpenWindow(serviceProvider);
        }

        /// <summary>
        /// TODO: this method needs deleting with better way to get accecss to the ViewModel so that toolkits can customize how nodes are displayed.
        /// </summary>
        public ISolutionBuilderViewModel GetSolutionBuilderViewModel(IServiceProvider serviceProvider)
        {
            Guard.NotNull(() => serviceProvider, serviceProvider);
            var packageToolWindow = serviceProvider.GetService<IPackageToolWindow>();

            var window = packageToolWindow.ShowWindow<SolutionBuilderToolWindow>(true);
            return window.ViewModel;
        }
    }
}
