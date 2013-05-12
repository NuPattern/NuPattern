using System;
using Microsoft.VisualStudio.Shell;

namespace NuPattern.Runtime.Shell.ToolWindows
{
    /// <summary>
    /// Provides a service interface to access a tool window in this package.
    /// </summary>
    internal interface IPackageToolWindow
    {
        /// <summary>
        /// Shows the window.
        /// </summary>
        /// <typeparam name="T">The type of the tool window to show.</typeparam>
        /// <param name="activate">Whether to force activation of the window</param>
        /// <returns>The <see cref="ToolWindowPane"/> to show.</returns>
        T ShowWindow<T>(bool activate) where T : ToolWindowPane;

        /// <summary>
        /// Hides the window.
        /// </summary>
        /// <typeparam name="T">The type of the tool window to hide.</typeparam>
        void HideWindow<T>() where T : ToolWindowPane;

        /// <summary>
        /// Determines whether the window is open in the IDE
        /// </summary>
        /// <typeparam name="T">The type of the tool window.</typeparam>
        /// <returns>
        /// 	<c>true</c> if [is window visible]; otherwise, <c>false</c>.
        /// </returns>
        bool IsWindowVisible<T>() where T : ToolWindowPane;

        /// <summary>
        /// Returns the window
        /// </summary>
        /// <typeparam name="T">The type of the tool window.</typeparam>
        ToolWindowPane GetWindow<T>() where T : ToolWindowPane;
    }
}