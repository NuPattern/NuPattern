using System.Collections.Generic;

namespace NuPattern.VisualStudio.Extensions
{
    /// <summary>
    /// Defines hte requirements for a VSIX
    /// </summary>
    public interface IExtensionRequirement
    {
        /// <summary>
        /// Gets the attributes of the requirement
        /// </summary>
        IDictionary<string, string> Attributes { get; }

        /// <summary>
        /// Gets the identifier of the requirement
        /// </summary>
        string Identifier { get; }

        /// <summary>
        /// Gets the version range of the requirement.
        /// </summary>
        IVersionRange VersionRange { get; }
    }
}
