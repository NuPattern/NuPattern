using System;
using Microsoft.VisualStudio.Shell;
using NuPattern.Runtime.UI.ViewModels;

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
        /// <remarks>
        /// TODO: This method is a workaround for toolkits that want to extend the Solution Builder views, and add buttons to teh toolbar.
        /// Currently, the toolkits are getting to the XAML content from the toolwindow, and adding their WPF buttons dynamically.
        /// </remarks>
        ToolWindowPane ShowSolutionBuilder(IServiceProvider serviceProvider);

        /// <summary>
        /// Gets the view model for the Solution Builder tool window
        /// </summary>
        /// <remarks>
        /// TODO: This method is a workaround for toolkits that want to extend the Solution Builder views.
        /// Currently, the toolkits are accessing the ViewModel and changing the presentation of its elements.
        /// </remarks>
        /// <returns></returns>
        ISolutionBuilderViewModel GetSolutionBuilderViewModel(IServiceProvider serviceProvider);

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
