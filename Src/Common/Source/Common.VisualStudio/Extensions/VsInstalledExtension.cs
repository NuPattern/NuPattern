using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using ExtMan = Microsoft.VisualStudio.ExtensionManager;

namespace NuPattern.VisualStudio.Extensions
{
    /// <summary>
    /// A wrapper for an installed VS VSIX registration
    /// </summary>
    internal class VsInstalledExtension : IInstalledExtension
    {
        private ExtMan.IInstalledExtension vsInstalledExtension;

        /// <summary>
        /// Creates a new instance of the <see cref="VsInstalledExtension"/> class.
        /// </summary>
        public VsInstalledExtension(ExtMan.IInstalledExtension vsInstalledExtension)
        {
            this.vsInstalledExtension = vsInstalledExtension;
        }

        public DateTimeOffset? InstalledOn
        {
            get { return this.vsInstalledExtension.InstalledOn; }
        }

        public bool InstalledPerMachine
        {
            get { return this.vsInstalledExtension.InstalledPerMachine; }
        }

        public string InstallPath
        {
            get { return this.vsInstalledExtension.InstallPath; }
        }

        public int SizeInBytes
        {
            get { return Convert.ToInt32(this.vsInstalledExtension.SizeInBytes); }
        }

        public EnabledState State
        {
            get { return (EnabledState)this.vsInstalledExtension.State; }
        }

        public IEnumerable<IExtensionContent> Content
        {
            get { return this.vsInstalledExtension.Content.Select(c => new VsExtensionContent(c)); }
        }

        public IExtensionHeader Header
        {
            get { return new VsExtensionHeader(this.vsInstalledExtension.Header); }
        }

        public IEnumerable<IExtensionReference> References
        {
            get { return this.vsInstalledExtension.References.Select(r => new VsExtensionReference(r)); }
        }

        public Version SchemaVersion
        {
            get { return this.vsInstalledExtension.SchemaVersion; }
        }

        public string Type
        {
            get { return this.vsInstalledExtension.Type; }
        }

#if VSVER11
        public bool IsPackComponent
        {
            get { return this.vsInstalledExtension.IsPackComponent; }
        }

        public IList<XmlElement> AdditionalElements
        {
            get { return this.vsInstalledExtension.AdditionalElements; }
        }
        
        public IList<XmlElement> LocalizedAdditionalElements
        {
            get { return this.vsInstalledExtension.LocalizedAdditionalElements; }
        }
        
        public IEnumerable<IExtensionRequirement> Targets
        {
            get { return this.vsInstalledExtension.Targets.Select(t => new VsExtensionRequirement(t)); }
        }

        public bool IsProductSupported(string productId, Version version)
        {
            return this.vsInstalledExtension.IsProductSupported(productId, version);
        }
#endif
    }
}
