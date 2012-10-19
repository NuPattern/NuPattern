
namespace Microsoft.VisualStudio.Patterning.Extensibility
{
    /// <summary>
    /// Extensibility Constants.
    /// </summary>
    public static class ExtensibilityConstants
    {
        /// <summary>
        /// MetadataFilter for MEF.
        /// </summary>
        public const string MetadataFilter = "PatternModelDsl";

        /// <summary>
        /// MSBuild item metadata property attached to the main .vsixmanifest of a factory project.
        /// </summary>
        public const string IsToolkitManifestItemMetadata = "IsToolkitManifest";
    }
}