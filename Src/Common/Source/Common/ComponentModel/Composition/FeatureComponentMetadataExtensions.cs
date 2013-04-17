using System;
using System.Collections.Generic;
using System.Linq;

namespace NuPattern.ComponentModel.Composition
{
    /// <summary>
    /// Extensions to the <see cref="IFeatureComponentMetadata"/>
    /// </summary>
    public static class FeatureComponentMetadataExtensions
    {
        /// <summary>
        /// Returns a <see cref="ExportedStandardValue"/> for the metadata.
        /// </summary>
        /// <param name="metadata"></param>
        /// <returns></returns>
        public static StandardValue AsStandardValue(this IFeatureComponentMetadata metadata)
        {
            return new ExportedStandardValue(
                string.IsNullOrEmpty(metadata.DisplayName) ? metadata.Id : metadata.DisplayName,
                metadata.Id,
                metadata.ExportingType,
                metadata.Description,
                metadata.Category);
        }
        /// <summary>
        /// Filters the exports to only those that were processed by the component catalog.
        /// </summary>
        public static IEnumerable<Lazy<T, TMetadataView>> FromFeaturesCatalog<T, TMetadataView>(this IEnumerable<Lazy<T, TMetadataView>> exports)
            where T : class
            where TMetadataView : IFeatureComponentMetadata
        {
            return exports.Where(e => e.Metadata.CatalogName == Catalog.CatalogName);
        }

        /// <summary>
        /// Filters the exports to only those that were processed by the component catalog.
        /// </summary>
        public static IEnumerable<Lazy<T, IDictionary<string, object>>> FromFeaturesCatalog<T>(this IEnumerable<Lazy<T, IDictionary<string, object>>> exports)
            where T : class
        {
            return exports.Where(e =>
                e.Metadata.ContainsKey(FeatureComponentCatalog.CatalogNameMetadataKey) &&
                (string)e.Metadata[FeatureComponentCatalog.CatalogNameMetadataKey] == Catalog.CatalogName);
        }
    }
}
