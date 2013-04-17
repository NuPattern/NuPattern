using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using ExtMan = Microsoft.VisualStudio.ExtensionManager;

namespace NuPattern.VisualStudio.Extensions
{
    /// <summary>
    /// A wrapper for a VS VSIX registration
    /// </summary>
    internal class VsExtension : IExtension
    {
        private ExtMan.IExtension vsExtension;

        /// <summary>
        /// Creates a new instance of the <see cref="VsExtension"/> class.
        /// </summary>
        /// <param name="vsExtension"></param>
        public VsExtension(ExtMan.IExtension vsExtension)
        {
            this.vsExtension = vsExtension;
        }

        public IList<XmlElement> AdditionalElements
        {
            get { return this.vsExtension.AdditionalElements; }
        }

        public IEnumerable<IExtensionContent> Content
        {
            get { return this.vsExtension.Content.Select(c => new VsExtensionContent(c)); }
        }

        public IExtensionHeader Header
        {
            get { return new VsExtensionHeader(this.vsExtension.Header); }
        }

        public bool IsProductSupported(string productId, Version version)
        {
            return this.vsExtension.IsProductSupported(productId, version);
        }

        public IList<XmlElement> LocalizedAdditionalElements
        {
            get { return this.vsExtension.LocalizedAdditionalElements; }
        }

        public IEnumerable<IExtensionReference> References
        {
            get { return this.vsExtension.References.Select(r => new VsExtensionReference(r)); }
        }

        public Version SchemaVersion
        {
            get { return this.vsExtension.SchemaVersion; }
        }

        public IEnumerable<IExtensionRequirement> Targets
        {
            get { return this.vsExtension.Targets.Select(t => new VsExtensionRequirement(t)); }
        }

        public string Type
        {
            get { return this.vsExtension.Type; }
        }
    }
}