
namespace NuPattern.Runtime.References
{
    /// <summary>
    /// Constants for reference kinds.
    /// </summary>
    public static partial class ReferenceKindConstants
    {
        /// <summary>
        /// Kind used on guidance references.
        /// </summary>
        public static readonly string GuidanceTopic = typeof(GuidanceReference).FullName;

        /// <summary>
        /// Kind used on artifact link references.
        /// </summary>
        public static readonly string SolutionItem = typeof(SolutionArtifactLinkReference).FullName;
    }
}
