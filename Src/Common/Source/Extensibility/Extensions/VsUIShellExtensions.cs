using System;
using System.Windows;
using System.Windows.Interop;
using Microsoft.VisualStudio.Patterning.Runtime;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Internal.VisualStudio.PlatformUI;

namespace Microsoft.VisualStudio.Patterning.Extensibility
{
	/// <summary>
	/// Defines extension methods related to <see cref="IVsUIShell"/>.
	/// </summary>
	internal static class VsUIShellExtensions
	{
		/// <summary>
		/// Gets the Visual Studio main window.
		/// </summary>
		public static Window GetMainWindow(this IVsUIShell shell)
		{
			IntPtr hwnd;
			ErrorHandler.ThrowOnFailure(shell.GetDialogOwnerHwnd(out hwnd));
			return HwndSource.FromHwnd(hwnd).RootVisual as Window;
		}

		/// <summary>
		/// Creates a <see cref="Window"/> dialog as child of the main Visual Studio window.
		/// </summary>
		/// <typeparam name="T">The type of the window to create.</typeparam>
		/// <param name="shell">The UI shell service.</param>
		/// <param name="dataContext">The data context.</param>
		/// <returns>
		/// The created <see cref="Window"/> dialog.
		/// </returns>
		public static T CreateDialog<T>(this IVsUIShell shell, object dataContext) where T : IDialogWindow, new()
		{
			var dialog = new T { DataContext = dataContext };

			//// Note that we are not using Window as a constraint in order to not add all WPF references in all the assemblies that reference this assembly
			var dialogWindow = dialog as Window;
			if (dialogWindow != null)
			{
				dialogWindow.Owner = shell.GetMainWindow();
			}

			return dialog;
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
			ErrorHandler.ThrowOnFailure(shell.ShowMessageBox(0, new Guid(), title, message, null, 0,
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
			ErrorHandler.ThrowOnFailure(shell.ShowMessageBox(0, new Guid(), title, message, null, 0,
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
            ErrorHandler.ThrowOnFailure(shell.ShowMessageBox(0, new Guid(), title, message, null, 0,
                OLEMSGBUTTON.OLEMSGBUTTON_YESNO, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST, OLEMSGICON.OLEMSGICON_WARNING, 1, out result));
            if (result == DialogResult.Yes)
            {
                return true;
            }

            return false;
        }
	}
}