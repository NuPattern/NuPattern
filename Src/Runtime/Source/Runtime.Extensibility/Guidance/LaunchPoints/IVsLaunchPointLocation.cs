using System;

namespace NuPattern.Runtime.Guidance.LaunchPoints
{
    /// <summary>
    /// Defines a launch point location in the VS IDE
    /// </summary>
    public interface IVsLaunchPointLocation
    {
        /// <summary>
        /// Gets the unique identifier
        /// </summary>
        Guid Guid { get; set; }

        /// <summary>
        /// Gets the id
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// Gets the display name
        /// </summary>
        string DisplayName { get; set; }

        /// <summary>
        /// Gets the parent group alias
        /// </summary>
        string Alias { get; set; }
    }
}