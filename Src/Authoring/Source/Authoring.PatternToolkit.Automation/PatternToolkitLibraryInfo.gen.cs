using System;

namespace NuPattern.Authoring.PatternToolkit.Automation
{
    /// <summary>
    /// Definitions for the PatternToolkitLibrary toolkit project
    /// </summary>
    internal class AutomationLibraryToolkitInfo
    {
        /// <summary>
        /// Gets the pattern definition identifier.
        /// </summary>
        public static Guid ProductId = new Guid("d6139b37-9971-4834-a9dc-2b3daef962cf");

        /// <summary>
        /// Gets the VSIX identifier of this toolkit.
        /// </summary>
        public static string ToolkitId = "97bd7ab2-964b-43f1-8a08-be6db68b018b";

        /// <summary>
        /// Gets the VSIX name of this toolkit.
        /// </summary>
        public static string RegistrationName = "NuPattern Toolkit Library";
    }
}

