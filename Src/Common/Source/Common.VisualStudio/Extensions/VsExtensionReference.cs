using System;
using ExtMan = Microsoft.VisualStudio.ExtensionManager;

namespace NuPattern.VisualStudio.Extensions
{
    /// <summary>
    /// A wrapper for a VS VSIX registration reference
    /// </summary>
    internal class VsExtensionReference : IExtensionReference
    {
        private ExtMan.IExtensionReference vsReference;

        public VsExtensionReference(ExtMan.IExtensionReference vsReference)
        {
            this.vsReference = vsReference;
        }

        public bool CanAutoDownload
        {
            get { return this.vsReference.CanAutoDownload; }
        }

        public bool IsRequired
        {
            get { return this.vsReference.IsRequired; }
        }

        public Uri MoreInfoUrl
        {
            get { return this.vsReference.MoreInfoUrl; }
        }

        public string Name
        {
            get { return this.vsReference.Name; }
        }

        public string NestedExtensionPath
        {
            get { return this.vsReference.NestedExtensionPath; }
        }
    }
}