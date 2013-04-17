using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.ReflectionModel;
using System.Linq;

namespace NuPattern.ComponentModel.Composition
{
    /// <summary>
    /// Custom catalog that filters an inner catalog.
    /// </summary>
    internal class FilteringReflectionCatalog : ComposablePartCatalog
    {
        /// <summary>
        /// Argument passed to the export filter, containing 
        /// detailed information about the export.
        /// </summary>
        internal class FilteredExport
        {
            /// <summary>
            /// Initializes the context from a part and the export.
            /// </summary>
            internal FilteredExport(ComposablePartDefinition part, ExportDefinition export)
            {
                this.ExportDefinition = export;
                this.ExportingMember = ReflectionModelServices.GetExportingMember(export);
                this.ExportingType = ReflectionModelServices.GetPartType(part).Value;
            }

            /// <summary>
            /// Gets the original export definition.
            /// </summary>
            public ExportDefinition ExportDefinition { get; private set; }
            /// <summary>
            /// Gets the type that provides the export.
            /// </summary>
            public Type ExportingType { get; private set; }
            /// <summary>
            /// Optional member where the export is provided.
            /// </summary>
            public LazyMemberInfo ExportingMember { get; private set; }
        }

        /// <summary>
        /// Argument passed to the part filter, containing
        /// detailed information about the part definition.
        /// </summary>
        internal class FilteredPart
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="FilteredPart"/> class with the 
            /// given part definition.
            /// </summary>
            public FilteredPart(ComposablePartDefinition part)
            {
                this.PartDefinition = part;
                this.PartType = ReflectionModelServices.GetPartType(part).Value;
            }

            /// <summary>
            /// Gets the part definition.
            /// </summary>
            public ComposablePartDefinition PartDefinition { get; private set; }

            /// <summary>
            /// Gets the concrete type of the part.
            /// </summary>
            public Type PartType { get; private set; }
        }

        private readonly ComposablePartCatalog innerCatalog;
        private IEnumerable<ComposablePartDefinition> parts;

        /// <summary>
        /// Initializes the catalog.
        /// </summary>
        public FilteringReflectionCatalog(ComposablePartCatalog innerCatalog)
        {
            this.innerCatalog = innerCatalog;
            this.PartFilter = context => true;
            this.ExportFilter = context => true;
        }

        /// <summary>
        /// Gets or sets the filter for part definitions.
        /// </summary>
        public Func<FilteredPart, bool> PartFilter { get; set; }

        /// <summary>
        /// Gets or sets the filter for exports.
        /// </summary>
        public Func<FilteredExport, bool> ExportFilter { get; set; }

        /// <summary>
        /// Gets the filtered exports from the inner catalog.
        /// </summary>
        public override IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> GetExports(ImportDefinition definition)
        {
            return base.GetExports(definition).Where(tuple => ExportFilter(new FilteredExport(tuple.Item1, tuple.Item2)));
        }

        /// <summary>
        /// Gets the filtered parts.
        /// </summary>
        public override IQueryable<ComposablePartDefinition> Parts
        {
            get
            {
                if (parts == null)
                    parts = innerCatalog.Parts.Where(definition => PartFilter(new FilteredPart(definition))).ToList();

                return parts.AsQueryable();
            }
        }

        /// <summary>
        /// Disposes the inner container.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                innerCatalog.Dispose();
        }
    }
}
