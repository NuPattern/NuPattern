using System;

namespace Microsoft.VisualStudio.Patterning.Extensibility
{
    /// <summary>
    /// Options for collapsing items in the solution.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue"), Flags]
    public enum CollapseOptions
    {
        /// <summary>
        /// Only items and folders in projects.
        /// </summary>
        Normal = 0,
        /// <summary>
        /// Includes projects.
        /// </summary>
        IncludeProjects = 1,
        /// <summary>
        /// Inlcudes all solution folders.
        /// </summary>
        IncludeSolutionFolders = 2,
        /// <summary>
        /// Includes all containers.
        /// </summary>
        All = 3
    }
}
