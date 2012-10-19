using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;

namespace Microsoft.VisualStudio.Patterning.Extensibility.Binding
{
    /// <summary>
    /// Exposes the <see cref="IFeatureCompositionService"/> API on top of 
    /// a <see cref="CompositionContainer"/>.
    /// </summary>
    internal class ContainerCompositionServiceAdapter : IFeatureCompositionService
    {
        private CompositionContainer container;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerCompositionServiceAdapter"/> class.
        /// </summary>
        /// <param name="container">The container to adapt.</param>
        public ContainerCompositionServiceAdapter(CompositionContainer container)
        {
            this.container = container;
        }

        /// <summary>
        /// Composes the parts.
        /// </summary>
        public void ComposeParts(params object[] attributedParts)
        {
            this.container.ComposeParts(attributedParts);
        }

        /// <summary>
        /// Gets the export.
        /// </summary>
        public Lazy<T, TMetadataView> GetExport<T, TMetadataView>()
        {
            return this.container.GetExport<T, TMetadataView>();
        }

        /// <summary>
        /// Gets the export.
        /// </summary>
        public Lazy<T> GetExport<T>()
        {
            return this.container.GetExport<T>();
        }

        /// <summary>
        /// Gets the exported value.
        /// </summary>
        public T GetExportedValue<T>()
        {
            return this.container.GetExportedValue<T>();
        }

        /// <summary>
        /// Gets the exported value or default.
        /// </summary>
        public T GetExportedValueOrDefault<T>()
        {
            return this.container.GetExportedValueOrDefault<T>();
        }

        /// <summary>
        /// Gets the exported values.
        /// </summary>
        public IEnumerable<T> GetExportedValues<T>()
        {
            return this.container.GetExportedValues<T>();
        }

        /// <summary>
        /// Gets the exports.
        /// </summary>
        public IEnumerable<Lazy<T, TMetadataView>> GetExports<T, TMetadataView>()
        {
            return this.container.GetExports<T, TMetadataView>();
        }

        /// <summary>
        /// Gets the exports.
        /// </summary>
        public IEnumerable<Lazy<T>> GetExports<T>()
        {
            return this.container.GetExports<T>();
        }

        /// <summary>
        /// Composes the specified part, with recomposition and validation disabled.
        /// </summary>
        public void SatisfyImportsOnce(ComposablePart part)
        {
            this.container.SatisfyImportsOnce(part);
        }

        /// <summary>
        /// Does not dispose the container received in the constructor.
        /// </summary>
        public void Dispose()
        {
        }

        public Lazy<T, TMetadataView> GetExport<T, TMetadataView>(string contractName)
        {
            return this.container.GetExport<T, TMetadataView>(contractName);
        }

        public Lazy<T> GetExport<T>(string contractName)
        {
            return this.container.GetExport<T>(contractName);
        }

        public T GetExportedValue<T>(string contractName)
        {
            return this.container.GetExportedValue<T>(contractName);
        }

        public T GetExportedValueOrDefault<T>(string contractName)
        {
            return this.container.GetExportedValueOrDefault<T>(contractName);
        }

        public IEnumerable<T> GetExportedValues<T>(string contractName)
        {
            return this.container.GetExportedValues<T>(contractName);
        }

        public IEnumerable<Lazy<T, TMetadataView>> GetExports<T, TMetadataView>(string contractName)
        {
            return this.container.GetExports<T, TMetadataView>(contractName);
        }

        public IEnumerable<Lazy<T>> GetExports<T>(string contractName)
        {
            return this.container.GetExports<T>(contractName);
        }
    }
}