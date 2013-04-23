using System;

namespace NuPattern.VisualStudio.Extensions
{
    /// <summary>
    /// Defines version range information
    /// </summary>
    public interface IVersionRange
    {
        /// <summary>
        /// Gets whether a maximum is defined
        /// </summary>
        bool IsMaximumInclusive { get; }

        /// <summary>
        /// Gets whether a minimum is defined
        /// </summary>
        bool IsMinimumInclusive { get; }

        /// <summary>
        /// Gets the maximum supported version
        /// </summary>
        Version Maximum { get; }

        /// <summary>
        /// Gets the minimum supported version.
        /// </summary>
        Version Minimum { get; }

        /// <summary>
        /// Determines whether the given value is contianed in this version range.
        /// </summary>
        bool Contains(Version value);
    }
}
