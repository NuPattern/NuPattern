using System.Collections.Generic;
using System.Xml;

namespace NuPattern.VisualStudio.Extensions
{
    /// <summary>
    /// Defines the content of a VSIX
    /// </summary>
    public interface IExtensionContent
    {
        /// <summary>
        /// Gets the attributes of the content.
        /// </summary>
        IDictionary<string, string> Attributes { get; }

        /// <summary>
        /// Gets the content type of the content
        /// </summary>
        string ContentTypeName { get; }

        /// <summary>
        /// Gets the relative path of the content
        /// </summary>
        string RelativePath { get; }

#if VSVER11 || VSVER12
        /// <summary>
        /// Gets the addtional unsupported elements of the content
        /// </summary>
        IList<XmlElement> AdditionalElements { get; }
#endif
    }
}
