namespace NuPattern.Runtime.Guidance
{
    /// <summary>
    /// Metadata provided by a guidance extension.
    /// </summary>
    public interface IGuidanceExtensionMetadata
    {
        /// <summary>
        /// Gets the identifier of the guidance extension, which should equal the containing VSIX identifier.
        /// </summary>
        string ExtensionId { get; }

        /// <summary>
        /// Default name for instances of this guidance extension.
        /// </summary>
        string DefaultName { get; }
    }
}