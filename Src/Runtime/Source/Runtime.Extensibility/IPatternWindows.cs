using System;
using Microsoft.VisualStudio.Shell;
using NuPattern.Presentation;

namespace NuPattern.Runtime
{
    /// <summary>
    /// Defines the tool windows for NuPattern.
    /// </summary>
    [CLSCompliant(false)]
    public interface IPatternWindows
    {
        /// <summary>
        /// Displays the Solution Builder tool window.
        /// </summary>
        ToolWindowPane ShowSolutionBuilder(IServiceProvider serviceProvider);

        /// <summary>
        /// Gets the view model for the Solution Builder tool window
        /// </summary>
        /// <remarks>
        /// TODO: This method is a workaround for toolkits that want to extend the Solution Builder views.
        /// </remarks>
        /// <returns></returns>
        ViewModel GetSolutionBuilderViewModel(IServiceProvider serviceProvider);

        /// <summary>
        /// Displays the Guidance Explorer tool window.
        /// </summary>
        void ShowGuidanceExplorer(IServiceProvider serviceProvider);

        /// <summary>
        /// Displays the Guidance Browser tool window.
        /// </summary>
        void ShowGuidanceBrowser(IServiceProvider serviceProvider);

        /// <summary>
        /// Hides the guidance windows
        /// </summary>
        void HideGuidanceWindows(IServiceProvider serviceProvider);
    }
}
