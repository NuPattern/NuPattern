using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;

namespace NuPattern.Extensibility.Bindings
{
    /// <summary>
    /// Implements a <see cref="IFeatureCompositionService"/> that delegates 
    /// all its members to an inner <see cref="IFeatureCompositionService"/>.
    /// </summary>
    internal class DelegatingCompositionService : IFeatureCompositionService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DelegatingCompositionService"/> class.
        /// </summary>
        /// <param name="compositionService">The composition service.</param>
        public DelegatingCompositionService(IFeatureCompositionService compositionService)
        {
            this.CompositionService = compositionService;
        }

        /// <summary>
        /// Gets or sets the composition service used for delegation.
        /// </summary>
        public IFeatureCompositionService CompositionService { get; set; }

        /// <summary>
        /// Composes the parts.
        /// </summary>
        public void ComposeParts(params object[] attributedParts)
        {
            this.CompositionService.ComposeParts(attributedParts);
        }

        /// <summary>
        /// Gets the export.
        /// </summary>
        public Lazy<T, TMetadataView> GetExport<T, TMetadataView>()
        {
            return this.CompositionService.GetExport<T, TMetadataView>();
        }

        /// <summary>
        /// Gets the export.
        /// </summary>
        public Lazy<T> GetExport<T>()
        {
            return this.CompositionService.GetExport<T>();
        }

        /// <summary>
        /// Gets the exported value.
        /// </summary>
        public T GetExportedValue<T>()
        {
            return this.CompositionService.GetExportedValue<T>();
        }

        /// <summary>
        /// Gets the exported value or default.
        /// </summary>
        public T GetExportedValueOrDefault<T>()
        {
            return this.CompositionService.GetExportedValueOrDefault<T>();
        }

        /// <summary>
        /// Gets the exported values.
        /// </summary>
        public IEnumerable<T> GetExportedValues<T>()
        {
            return this.CompositionService.GetExportedValues<T>();
        }

        /// <summary>
        /// Gets the exports.
        /// </summary>
        public IEnumerable<Lazy<T, TMetadataView>> GetExports<T, TMetadataView>()
        {
            return this.CompositionService.GetExports<T, TMetadataView>();
        }

        /// <summary>
        /// Gets the exports.
        /// </summary>
        public IEnumerable<Lazy<T>> GetExports<T>()
        {
            return this.CompositionService.GetExports<T>();
        }

        /// <summary>
        /// Satisfies the imports once.
        /// </summary>
        public void SatisfyImportsOnce(ComposablePart part)
        {
            this.CompositionService.SatisfyImportsOnce(part);
        }

        /// <summary>
        /// Does not dispose the inner <see cref="CompositionService"/>.
        /// </summary>
        public void Dispose()
        {
        }
    }
}
