using System;

namespace NuPattern.Runtime.Guidance.LaunchPoints
{
    /// <summary>
    /// Represents a visual studio known launch point location
    /// </summary>
    internal class VsLaunchPointLocation : IVsLaunchPointLocation
    {
        /// <summary>
        /// Gets the unique identifier
        /// </summary>
        public Guid Guid { get; set; }

        /// <summary>
        /// Gets the id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets the display name
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets the parent group alias
        /// </summary>
        public string Alias { get; set; }
    }
}