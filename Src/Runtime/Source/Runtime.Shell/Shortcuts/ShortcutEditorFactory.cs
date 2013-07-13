using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace NuPattern.Runtime.Shell.Shortcuts
{
    /// <summary>
    /// Defines the editor to handle shortcuts
    /// </summary>
    [Guid(Constants.ShortcutEditorGuid)]
    internal class ShortcutEditorFactory : IVsEditorFactory
    {
        private IServiceProvider serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShortcutEditorFactory"/> class.
        /// </summary>
        public ShortcutEditorFactory(IServiceProvider provider)
        {
            Guard.NotNull(() => provider, provider);

            this.serviceProvider = provider;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShortcutEditorFactory"/> class.
        /// </summary>
        internal ShortcutEditorFactory(IServiceProvider provider, IShortcutPersistenceHandler fileHandler)
            : this(provider)
        {
            Guard.NotNull(() => fileHandler, fileHandler);

            this.PersistenceHandler = fileHandler;
        }

        /// <summary>
        /// Gets or sets the persistence handler.
        /// </summary>
        internal IShortcutPersistenceHandler PersistenceHandler
        {
            get;
            private set;
        }

        /// <summary>
        /// Used by the editor factory architecture to create editors that support data/view separation.  
        /// </summary>
        public int CreateEditorInstance(uint grfCreateDoc, string pszMkDocument, string pszPhysicalView, IVsHierarchy pvHier, uint itemid, IntPtr punkDocDataExisting, out IntPtr ppunkDocView, out IntPtr ppunkDocData, out string pbstrEditorCaption, out Guid pguidCmdUI, out int pgrfCDW)
        {
            ppunkDocView = IntPtr.Zero;
            ppunkDocData = IntPtr.Zero;
            pbstrEditorCaption = string.Empty;
            pguidCmdUI = this.GetType().GUID;
            pgrfCDW = 0;

            // Ensure that the file exists
            if (!File.Exists(pszMkDocument))
            {
                return VSConstants.S_FALSE;
            }

            // Ensure we have a file handler.
            if (this.PersistenceHandler == null)
            {
                this.PersistenceHandler = new ShortcutFileHandler(pszMkDocument);
            }

            // Launch the shortcut
            var result = ShortcutLaunchCoordinator.LaunchShortcut(this.serviceProvider, this.PersistenceHandler);
            return result ? VSConstants.S_OK : VSConstants.S_FALSE;
        }

        /// <summary>
        /// Closes the editor.
        /// </summary>
        public int Close()
        {
            // Nothing to save, nothing to do.
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Maps a logical view to a physical view. 
        /// </summary>
        public int MapLogicalView(ref Guid rguidLogicalView, out string pbstrPhysicalView)
        {
            // We dont have a logical view per se in this editor, nothing to do.
            pbstrPhysicalView = null;

            return VSConstants.S_OK;
        }

        /// <summary>
        /// Sets the site of the editor.
        /// </summary>
        public int SetSite(Microsoft.VisualStudio.OLE.Interop.IServiceProvider psp)
        {
            // We don't have a editor, dont need to site it, nothing to do here.
            return VSConstants.S_OK;
        }
    }
}
