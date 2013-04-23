using System;

namespace NuPattern.Runtime.Guidance
{
    /// <summary>
    /// Manipulation of tool windows
    /// </summary>
    public interface IGuidanceWindowsService
    {
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
