using System;
using System.Linq;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace NuPattern.VisualStudio.Windows
{
    /// <summary>
    /// Extensions for managing document windows.
    /// </summary>
    public static class WindowExtensions
    {
        /// <summary>
        /// Gets the designer from the specified file if it is open in its default editor.
        /// </summary>
        /// <param name="fileName">The physical path to the file to open.</param>
        public static T GetDesignerData<T>(string fileName) where T : class
        {
            var dte = ServiceProvider.GlobalProvider.GetService<SDTE, DTE>();
            var rdt = new RunningDocumentTable(ServiceProvider.GlobalProvider);

            var documentInfo = rdt.FirstOrDefault(info => info.Moniker.Equals(fileName, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrEmpty(documentInfo.Moniker))
            {
                ActivateDocument(documentInfo.Moniker);

                var docData = documentInfo.DocData as T;
                if (docData == null)
                {
                    // Close file is not  opened in 'designer' view
                    var projectItem = dte.Solution.FindProjectItem(documentInfo.Moniker);
                    projectItem.Document.Close(vsSaveChanges.vsSaveChangesYes);

                    // Open in designer mode (invisibly)
                    docData = GetOpenedDocData<T>(OpenDesigner(fileName, false));
                }

                return docData;
            }

            return null;
        }

        /// <summary>
        /// Performs the action on the specified file while open in its default editor.
        /// </summary>
        /// <param name="fileName">The physical path to the file to open.</param>
        /// <param name="action">The action to execute</param>
        /// <param name="leaveOpen">Whether to leave the file open in editor or close it after complete</param>
        public static void DoActionOnWindow<T>(string fileName, Action<T> action, bool leaveOpen = false) where T : class
        {
            // Activate if already open (then leave open)
            var docData = GetDesignerData<T>(fileName);
            if (docData != null)
            {
                action(docData);
            }
            else
            {
                // Open in designer mode (visible)
                var document = OpenDesigner(fileName, !leaveOpen);
                action(GetOpenedDocData<T>(document));

                // Save (and leave open) or close document
                if (leaveOpen)
                {
                    document.Save();
                }
                else
                {
                    document.Close(vsSaveChanges.vsSaveChangesYes);
                }
            }
        }

        private static T GetOpenedDocData<T>(Document document) where T : class
        {
            var rdt = new RunningDocumentTable(ServiceProvider.GlobalProvider);
            var documentInfo = rdt.FirstOrDefault(info => info.Moniker.Equals(document.FullName, StringComparison.OrdinalIgnoreCase));

            return documentInfo.DocData as T;
        }

        /// <summary>
        /// Activates the specified open file. 
        /// </summary>
        /// <param name="fileName">The full path to the item to open.</param>
        public static void ActivateDocument(string fileName)
        {
            var dte = ServiceProvider.GlobalProvider.GetService<SDTE, DTE>();
            var document = dte.Documents.OfType<Document>().FirstOrDefault(
                doc => doc.FullName.Equals(fileName, StringComparison.OrdinalIgnoreCase));

            if (document != null)
            {
                document.Activate();
            }
        }

        /// <summary>
        /// Opens the file in its default designer.
        /// </summary>
        /// <param name="fileName">The full path to the item to open.</param>
        /// <param name="invisibleMode"></param>
        /// <returns></returns>
        [CLSCompliant(false)]
        public static Document OpenDesigner(string fileName, bool invisibleMode = true)
        {
            var dte = ServiceProvider.GlobalProvider.GetService<SDTE, DTE>();
            var projectItem = dte.Solution.FindProjectItem(fileName);

            //TODO: Open always in designer view 
            var window = projectItem.Open(EnvDTE.Constants.vsViewKindDesigner);

            if (!invisibleMode)
            {
                window.Visible = true;
                window.Activate();
            }

            return window.Document;
        }
    }
}