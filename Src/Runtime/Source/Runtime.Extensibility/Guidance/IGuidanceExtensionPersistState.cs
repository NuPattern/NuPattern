using System;

namespace NuPattern.Runtime.Guidance
{
    /// <summary>
    /// Defines the persisted state of a feature
    /// </summary>
    public interface IGuidanceExtensionPersistState
    {
        /// <summary>
        /// Gets the identifier fo the guidance extension
        /// </summary>
        string ExtensionId { get; set; }

        /// <summary>
        /// Gets the name of the guidance extension
        /// </summary>
        string InstanceName { get; set; }

        /// <summary>
        /// Gets the version of the persisted guidance extension.
        /// </summary>
        Version Version { get; set; }
    }
}