
using System;
using System.Collections.Generic;
using System.Xml;

namespace NuPattern.VisualStudio.Extensions
{
    /// <summary>
    /// Defines a VSIX extension
    /// </summary>
    public interface IExtension
    {
        /// <summary>
        /// Gets the content of the VSIX
        /// </summary>
        IEnumerable<IExtensionContent> Content { get; }

        /// <summary>
        /// Gets the header of the VSIX
        /// </summary>
        IExtensionHeader Header { get; }

        /// <summary>
        /// Gets the dependencies
        /// </summary>
        IEnumerable<IExtensionReference> References { get; }

        /// <summary>
        /// Gets the version of the VSIX manifest schema
        /// </summary>
        Version SchemaVersion { get; }

        /// <summary>
        /// Gets the type of the VSIX
        /// </summary>
        string Type { get; }

#if VSVER11 || VSVER12
        /// <summary>
        /// Gets the addtional unsupported elements defined in the VSIX manifest
        /// </summary>
        IList<XmlElement> AdditionalElements { get; }

        /// <summary>
        /// Gets a value to indicate whether the current product is supported
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        bool IsProductSupported(string productId, Version version);

        /// <summary>
        /// Gets the localized version of the unsupported element defined in the VSIX manifest
        /// </summary>
        IList<XmlElement> LocalizedAdditionalElements { get; }

        /// <summary>
        /// Gets the targets for the VSIX
        /// </summary>
        IEnumerable<IExtensionRequirement> Targets { get; }
#endif
    }
}
