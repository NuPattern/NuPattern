using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell.Interop;

namespace NuPattern.Runtime.Shell
{
    /// <summary>
    /// Provides opening behavior for pattern state files.
    /// </summary>
    [Guid("449EA23C-4893-4EB9-8324-364354EEE5B7")]
    internal sealed class ProductStateEditorFactory : IVsEditorFactory
    {
        private IPatternManager patternManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductStateEditorFactory"/> class.
        /// </summary>
        /// <param name="patternManager">The pattern manager.</param>
        public ProductStateEditorFactory(IPatternManager patternManager)
        {
            this.patternManager = patternManager;
        }

        /// <summary>
        /// Creates the editor instance.
        /// </summary>
        /// <param name="grfCreateDoc">The GRF create doc.</param>
        /// <param name="pszMkDocument">The PSZ mk document.</param>
        /// <param name="pszPhysicalView">The PSZ physical view.</param>
        /// <param name="hierarchy">The pv hier.</param>
        /// <param name="itemid">The itemid.</param>
        /// <param name="punkDocDataExisting">The punk doc data existing.</param>
        /// <param name="ppunkDocView">The ppunk doc view.</param>
        /// <param name="ppunkDocData">The ppunk doc data.</param>
        /// <param name="pbstrEditorCaption">The PBSTR editor caption.</param>
        /// <param name="pguidCmdUI">The pguid CMD UI.</param>
        /// <param name="pgrfCDW">The PGRF CDW.</param>
        [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "3#", Justification = "COM")]
        public int CreateEditorInstance(uint grfCreateDoc, string pszMkDocument, string pszPhysicalView, IVsHierarchy hierarchy, uint itemid, IntPtr punkDocDataExisting, out IntPtr ppunkDocView, out IntPtr ppunkDocData, out string pbstrEditorCaption, out Guid pguidCmdUI, out int pgrfCDW)
        {
            ppunkDocView = IntPtr.Zero;
            ppunkDocData = IntPtr.Zero;
            pguidCmdUI = this.GetType().GUID;
            pgrfCDW = 0;
            pbstrEditorCaption = string.Empty;

            ppunkDocView = IntPtr.Zero;
            ppunkDocData = IntPtr.Zero;

            this.patternManager.Open(pszMkDocument);

            return Microsoft.VisualStudio.VSConstants.S_OK;
        }

        #region Unused

        /// <summary>
        /// Closes this instance.
        /// </summary>
        /// <returns>The error result.</returns>
        public int Close()
        {
            return Microsoft.VisualStudio.VSConstants.S_OK;
        }

        /// <summary>
        /// Maps the logical view.
        /// </summary>
        /// <param name="rguidLogicalView">The rguid logical view.</param>
        /// <param name="pbstrPhysicalView">The PBSTR physical view.</param>
        /// <returns>The error result.</returns>
        public int MapLogicalView(ref Guid rguidLogicalView, out string pbstrPhysicalView)
        {
            pbstrPhysicalView = null;

            return Microsoft.VisualStudio.VSConstants.S_OK;
        }

        /// <summary>
        /// Sets the site.
        /// </summary>
        /// <param name="psp">The OLE service provider.</param>
        /// <returns>The error result.</returns>
        public int SetSite(Microsoft.VisualStudio.OLE.Interop.IServiceProvider psp)
        {
            return Microsoft.VisualStudio.VSConstants.S_OK;
        }

        #endregion
    }
}
