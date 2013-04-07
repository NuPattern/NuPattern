using System;
using System.Windows;
using System.Windows.Interop;
using Microsoft.Internal.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell.Interop;

namespace NuPattern.VisualStudio
{
    /// <summary>
    /// Defines extension methods related to <see cref="IVsUIShell"/>.
    /// </summary>
    [CLSCompliant(false)]
    public static class VsUIShellExtensions
    {
        /// <summary>
        /// Gets the Visual Studio main window.
        /// </summary>
        public static Window GetMainWindow(this IVsUIShell shell)
        {
            IntPtr hwnd;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(shell.GetDialogOwnerHwnd(out hwnd));
            return HwndSource.FromHwnd(hwnd).RootVisual as Window;
        }

        /// <summary>
        /// Displays a warning dialog.
        /// </summary>
        /// <param name="shell">The service.</param>
        /// <param name="title">The title of the dialog.</param>
        /// <param name="message">The message to display in the dialog.</param>
        public static void ShowWarning(this IVsUIShell shell, string title, string message)
        {
            Guard.NotNull(() => title, title);
            Guard.NotNull(() => message, message);

            int result;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(shell.ShowMessageBox(0, new Guid(), title, message, null, 0,
                OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST, OLEMSGICON.OLEMSGICON_WARNING, 1, out result));
        }

        /// <summary>
        /// Displays and error dialog.
        /// </summary>
        /// <param name="shell">The service.</param>
        /// <param name="title">The title of the dialog.</param>
        /// <param name="message">The message to display in the dialog.</param>
        public static void ShowError(this IVsUIShell shell, string title, string message)
        {
            Guard.NotNull(() => title, title);
            Guard.NotNull(() => message, message);

            int result;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(shell.ShowMessageBox(0, new Guid(), title, message, null, 0,
                OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST, OLEMSGICON.OLEMSGICON_CRITICAL, 1, out result));
        }

        /// <summary>
        /// Displays a prompt dialog with warning icon.
        /// </summary>
        /// <param name="shell">The service.</param>
        /// <param name="title">The title of the dialog.</param>
        /// <param name="message">The message to display in the dialog.</param>
        /// <returns>True if the user selects 'Yes', false otherwise./>/></returns>
        public static bool ShowPrompt(this IVsUIShell shell, string title, string message)
        {
            Guard.NotNull(() => title, title);
            Guard.NotNull(() => message, message);

            int result;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(shell.ShowMessageBox(0, new Guid(), title, message, null, 0,
                OLEMSGBUTTON.OLEMSGBUTTON_YESNO, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST, OLEMSGICON.OLEMSGICON_WARNING, 1, out result));
            if (result == DialogResult.Yes)
            {
                return true;
            }

            return false;
        }
    }
}