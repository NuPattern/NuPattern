
using System.Collections.Generic;
namespace NuPattern.Authoring.PatternToolkit
{
    /// <summary>
    /// Info defined in the version independent TARGETS file
    /// </summary>
    internal class TargetsInfo
    {
        /// <summary>
        /// Gets or sets the path of the targets file.
        /// </summary>
        public string TargetsPath { get; set; }

        /// <summary>
        /// Gets or sets the version of the toolkit
        /// </summary>
        public string ToolkitVersion { get; set; }

        /// <summary>
        /// Gets or sets the installed extension path properties
        /// </summary>
        public Dictionary<string, string> InstalledExtensionProperties { get; set; }
    }
}
