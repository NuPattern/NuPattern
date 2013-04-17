using Microsoft.VisualStudio.Shell;

namespace NuPattern.Runtime.Shell.ToolWindows
{
    /// <summary>
    /// Provides a service interface to acces a tool window in this package.
    /// </summary>
    internal interface IPackageToolWindow
    {
        /// <summary>
        /// Shows the window.
        /// </summary>
        /// <typeparam name="T">The type of the tool window to show.</typeparam>
        /// <returns>The <see cref="ToolWindowPane"/> to show.</returns>
        T ShowWindow<T>() where T : ToolWindowPane;

        /// <summary>
        /// Hides the window.
        /// </summary>
        /// <typeparam name="T">The type of the tool window to hide.</typeparam>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        void HideWindow<T>() where T : ToolWindowPane;

        /// <summary>
        /// Determines whether [is window visible].
        /// </summary>
        /// <typeparam name="T">The type of the tool window.</typeparam>
        /// <returns>
        /// 	<c>true</c> if [is window visible]; otherwise, <c>false</c>.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        bool IsWindowVisible<T>() where T : ToolWindowPane;
    }
}