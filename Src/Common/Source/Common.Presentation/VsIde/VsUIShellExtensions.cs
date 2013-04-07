using System;
using System.Windows;
using Microsoft.VisualStudio.Shell.Interop;
using NuPattern.VisualStudio;

namespace NuPattern.Presentation.VsIde
{
    /// <summary>
    /// Defines extension methods related to <see cref="IVsUIShell"/>.
    /// </summary>
    [CLSCompliant(false)]
    public static class VsUIShellExtensions
    {
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
    }
}