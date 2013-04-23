using System.Collections.Generic;
using System.Xml;
using ExtMan = Microsoft.VisualStudio.ExtensionManager;

namespace NuPattern.VisualStudio.Extensions
{
    /// <summary>
    /// A wrapper for a VS VSIX registration content
    /// </summary>
    internal class VsExtensionContent : IExtensionContent
    {
        private ExtMan.IExtensionContent vsContent;

        /// <summary>
        /// Creates a new instance of the <see cref="VsExtensionContent"/> class.
        /// </summary>
        public VsExtensionContent(ExtMan.IExtensionContent vsContent)
        {
            this.vsContent = vsContent;
        }

        public IDictionary<string, string> Attributes
        {
            get { return this.vsContent.Attributes; }
        }

        public string ContentTypeName
        {
            get { return this.vsContent.ContentTypeName; }
        }

        public string RelativePath
        {
            get { return this.vsContent.RelativePath; }
        }

#if VSVER11
        public IList<XmlElement> AdditionalElements
        {
            get { return this.vsContent.AdditionalElements; }
        }
#endif
    }
}