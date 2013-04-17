namespace NuPattern.ComponentModel
{
    /// <summary>
    /// Extensions to the <see cref="IBananaComponentMetadata"/>
    /// </summary>
    public static class BananaComponentMetadataExtensions
    {
        /// <summary>
        /// Returns a <see cref="ExportedStandardValue"/> for the metadata.
        /// </summary>
        /// <param name="metadata"></param>
        /// <returns></returns>
        public static StandardValue AsStandardValue(this IBananaComponentMetadata metadata)
        {
            return new ExportedStandardValue(
                string.IsNullOrEmpty(metadata.DisplayName) ? metadata.Id : metadata.DisplayName,
                metadata.Id,
                metadata.ExportingType,
                metadata.Description,
                metadata.Category);
        }
    }
}
