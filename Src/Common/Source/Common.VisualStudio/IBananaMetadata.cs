
namespace NuPattern.VisualStudio
{
    /// <summary>
    /// Metadata provided by a feature
    /// </summary>
    public interface IBananaMetadata
    {
        /// <summary>
        /// The feature identifier, which should equal the containing VSIX identifier.
        /// </summary>
        string FeatureId { get; }

        /// <summary>
        /// Default name for instances of this feature.
        /// </summary>
        string DefaultName { get; }
    }
}
