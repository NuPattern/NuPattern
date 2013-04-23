using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.ReflectionModel;
using System.Linq;
using NuPattern.Properties;

namespace NuPattern.ComponentModel.Composition
{
    /// <summary>
    /// Custom catalog that allows decorating a reflection-based catalog with 
    /// custom export and part metadata.
    /// </summary>
    public class DecoratingReflectionCatalog : ComposablePartCatalog, ICompositionElement
    {
        /// <summary>
        /// The context where a reflection-based export was found in the decorated catalog.
        /// </summary>
        public class DecoratedExport
        {
            /// <summary>
            /// Initializes the context from a part and the export.
            /// </summary>
            internal DecoratedExport(ComposablePartDefinition part, ExportDefinition export)
            {
                this.ExportDefinition = export;
                this.ExportingMember = ReflectionModelServices.GetExportingMember(export);
                this.ExportingType = ReflectionModelServices.GetPartType(part);
                this.NewMetadata = new Dictionary<string, object>(export.Metadata);
            }

            /// <summary>
            /// Gets a read/write bag of metadata containing the 
            /// original export metadata.
            /// </summary>
            public IDictionary<string, object> NewMetadata { get; private set; }
            /// <summary>
            /// Gets the original export definition.
            /// </summary>
            public ExportDefinition ExportDefinition { get; private set; }
            /// <summary>
            /// Gets the type that provides the export.
            /// </summary>
            public Lazy<Type> ExportingType { get; private set; }
            /// <summary>
            /// Optional member where the export is provided.
            /// </summary>
            public LazyMemberInfo ExportingMember { get; private set; }
        }

        /// <summary>
        /// The context where a reflection-based part was found in the decorated catalog.
        /// </summary>
        public class DecoratedPart
        {
            /// <summary>
            /// Initializes the context from a part definition.
            /// </summary>
            internal DecoratedPart(ComposablePartDefinition definition)
            {
                this.PartDefinition = definition;
                this.PartType = ReflectionModelServices.GetPartType(definition);
                this.NewMetadata = new Dictionary<string, object>(definition.Metadata);
            }

            /// <summary>
            /// Gets a read/write bag of metadata containing the 
            /// original part metadata.
            /// </summary>
            public IDictionary<string, object> NewMetadata { get; private set; }
            /// <summary>
            /// Gets the original part definition.
            /// </summary>
            public ComposablePartDefinition PartDefinition { get; private set; }
            /// <summary>
            /// Gets the part type.
            /// </summary>
            public Lazy<Type> PartType { get; private set; }
        }

        private readonly ComposablePartCatalog innerCatalog;
        private IEnumerable<ComposablePartDefinition> cachedSharedParts;

        /// <summary>
        /// Initializes the catalog.
        /// </summary>
        /// <param name="innerCatalog"></param>
        public DecoratingReflectionCatalog(ComposablePartCatalog innerCatalog)
        {
            this.ExportMetadataDecorator = context => { };
            this.PartMetadataDecorator = context => { };
            this.innerCatalog = innerCatalog;
        }

        /// <summary>
        /// Gets or sets the decorator for a parts metadata.
        /// </summary>
        public Action<DecoratedPart> PartMetadataDecorator { get; set; }

        /// <summary>
        /// Gets or sets the decorator for exports metadata.
        /// </summary>
        public Action<DecoratedExport> ExportMetadataDecorator { get; set; }

        /// <summary>
        /// Applies the decorations and gets the parts definitions.
        /// </summary>
        public override IQueryable<ComposablePartDefinition> Parts
        {
            get
            {
                if (this.cachedSharedParts == null)
                {
                    this.cachedSharedParts = this.BuildSharedParts().ToList();
                }

                return this.cachedSharedParts.Concat(BuildNonSharedParts())
                    .Distinct(new SelectorEqualityComparer<ComposablePartDefinition, Type>(def => ReflectionModelServices.GetPartType(def).Value))
                    .AsQueryable();
            }
        }

        private IEnumerable<ComposablePartDefinition> BuildSharedParts()
        {
            return BuildParts(innerCatalog.Parts.Where(def => !IsNonShared(def)));
        }

        private IEnumerable<ComposablePartDefinition> BuildNonSharedParts()
        {
            return BuildParts(innerCatalog.Parts.Where(def => IsNonShared(def)));
        }

        private IEnumerable<ComposablePartDefinition> BuildParts(IQueryable<ComposablePartDefinition> parts)
        {
            return parts.Select(def => ReflectionModelServices.CreatePartDefinition(
                ReflectionModelServices.GetPartType(def),
                true,
                new Lazy<IEnumerable<ImportDefinition>>(() => def.ImportDefinitions),
                new Lazy<IEnumerable<ExportDefinition>>(() => def.ExportDefinitions.Select(export =>
                    ReflectionModelServices.CreateExportDefinition(
                        ReflectionModelServices.GetExportingMember(export),
                        export.ContractName,
                        new Lazy<IDictionary<string, object>>(() => VisitExport(def, export)),
                        this))),
                new Lazy<IDictionary<string, object>>(() => VisitPart(def)),
                this));
        }

        private bool IsNonShared(ComposablePartDefinition def)
        {
            var metadata = VisitPart(def);
            return metadata.ContainsKey(CompositionConstants.PartCreationPolicyMetadataName) &&
                (CreationPolicy)metadata[CompositionConstants.PartCreationPolicyMetadataName] == CreationPolicy.NonShared;
        }

        private IDictionary<string, object> VisitPart(ComposablePartDefinition def)
        {
            var context = new DecoratedPart(def);
            PartMetadataDecorator(context);
            return context.NewMetadata;
        }

        private IDictionary<string, object> VisitExport(ComposablePartDefinition part, ExportDefinition export)
        {
            var context = new DecoratedExport(part, export);
            ExportMetadataDecorator(context);
            return context.NewMetadata;
        }

        /// <summary>
        /// Disposes the inner catalog.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                innerCatalog.Dispose();
        }

        string ICompositionElement.DisplayName
        {
            get
            {
                var composition = innerCatalog as ICompositionElement;
                if (composition == null)
                    return Resources.DecoratingReflectionCatalog_DisplayName;
                else
                    return Resources.DecoratingReflectionCatalog_CatalogDisplayName + composition.DisplayName;
            }
        }

        ICompositionElement ICompositionElement.Origin
        {
            get { return innerCatalog as ICompositionElement; }
        }
    }
}
