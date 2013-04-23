
using System;

namespace NuPattern.VisualStudio.Extensions
{
    /// <summary>
    /// Defines the dependencies VSIXes of the VSIX
    /// </summary>
    public interface IExtensionReference
    {
        /// <summary>
        /// Gets a 'More Information' URI.
        /// </summary>
        Uri MoreInfoUrl { get; }

        /// <summary>
        /// Gets the unique name of the dependent VSIX
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the nested path to the dependency in the VSIX package
        /// </summary>
        string NestedExtensionPath { get; }

#if VSVER11
        /// <summary>
        /// Gets a value to indicate whether the dependency can be downloaded automatically.
        /// </summary>
        bool CanAutoDownload { get; }

        /// <summary>
        /// Gets a value to indicate whether the dependency is required.
        /// </summary>
        bool IsRequired { get; }
#endif
    }
}
