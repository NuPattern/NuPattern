using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.ReflectionModel;
using System.Linq;
using NuPattern.Reflection;

namespace NuPattern.ComponentModel.Composition
{
    /// <summary>
    /// Custom catalog that filters only parts that have a 
    /// <see cref="ComponentAttribute"/> applied, and
    /// provides default feature component metadata behavior.
    /// </summary>
    public class ComponentCatalog : DecoratingReflectionCatalog
    {
        internal readonly static string CatalogNameMetadataKey = Reflect<IComponentMetadata>.GetProperty(x => x.CatalogName).Name;
        internal readonly static string IdMetadataKey = Reflect<IComponentMetadata>.GetProperty(x => x.Id).Name;
        internal readonly static string CategoryMetadataKey = Reflect<IComponentMetadata>.GetProperty(x => x.Category).Name;
        internal readonly static string DescriptionMetadataKey = Reflect<IComponentMetadata>.GetProperty(x => x.Description).Name;
        internal readonly static string DisplayNameMetadataKey = Reflect<IComponentMetadata>.GetProperty(x => x.DisplayName).Name;
        internal readonly static string ExportingTypeMetadataKey = Reflect<IComponentMetadata>.GetProperty(x => x.ExportingType).Name;

        /// <summary>
        /// Creates a new instance of the <see cref="ComponentCatalog"/> class.
        /// </summary>
        /// <param name="innerCatalog"></param>
        public ComponentCatalog(ComposablePartCatalog innerCatalog)
            : base(new FilteringReflectionCatalog(innerCatalog)
            {
                PartFilter = part => part.PartType.AsComponent() != null,
                ExportFilter = export => export.ExportingType.AsComponent() != null
            })
        {
            base.PartMetadataDecorator = PartDecorator;
            base.ExportMetadataDecorator = ExportDecorator;
        }

        private static void PartDecorator(DecoratedPart context)
        {
            DecorateMetadata(context.PartType.Value, context.NewMetadata);
        }

        private static void ExportDecorator(DecoratedExport context)
        {
            DecorateMetadata(context.ExportingType.Value, context.NewMetadata);
        }

        private static void DecorateMetadata(Type partType, IDictionary<string, object> metadata)
        {
            // Mark all components as having passed through our catalog.
            metadata[CatalogNameMetadataKey] = Catalog.CatalogName;

            var attribute = partType.AsComponent();

            if (attribute != null)
            {
                // Override metadata with defaults if necessary.
                metadata[IdMetadataKey] = attribute.Id;
                metadata[CategoryMetadataKey] = attribute.Category;
                metadata[DescriptionMetadataKey] = attribute.Description;
                metadata[DisplayNameMetadataKey] = attribute.DisplayName;
                metadata[ExportingTypeMetadataKey] = partType;

                // check first for the presence of a creation policy metadata, and if none is defined, assign the NonShared one
                if (!metadata.ContainsKey(CompositionConstants.PartCreationPolicyMetadataName))
                    metadata[CompositionConstants.PartCreationPolicyMetadataName] = CreationPolicy.NonShared;
            }
        }

        /// <summary>
        /// Gets the exports.
        /// </summary>
        public IDictionary<string, IList<string>> Exports
        {
            get
            {
                return base.Parts.ToDictionary(
                    part => ReflectionModelServices.GetPartType(part).Value.Name,
                    part => (IList<string>)part.ExportDefinitions.Select(export => export.ContractName).ToList());
            }
        }
    }
}
