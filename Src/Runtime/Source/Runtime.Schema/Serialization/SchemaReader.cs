using System;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Diagrams;
using Microsoft.VisualStudio.Shell;
using NuPattern.Modeling;
using NuPattern.VisualStudio;

namespace NuPattern.Runtime.Schema
{
    /// <summary>
    /// Default <see cref="ISchemaReader"/> implementation that 
    /// uses a modeling state.
    /// </summary>
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(ISchemaReader))]
    internal class SchemaReader : ISchemaReader
    {
        private IServiceProvider serviceProvider;
        private ISerializerLocator serializerLocator;
        private IExtensionLocator extensionLocator;

        /// <summary>
        /// Initializes a new instance of the <see cref="SchemaReader"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        [ImportingConstructor]
        public SchemaReader([Import(typeof(SVsServiceProvider))] IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            var componentModel = this.serviceProvider.GetService<SComponentModel, IComponentModel>();
            this.serializerLocator = new StandardSerializerLocator(componentModel.DefaultExportProvider);
            this.extensionLocator = new StandardExtensionLocator(componentModel.DefaultExportProvider);
        }

        /// <summary>
        /// Loads the model from the given <paramref name="stream"/>.
        /// </summary>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Can not dispose state")]
        public IPatternModelInfo Load(Stream stream)
        {
            var store = this.CreateStore();
            store.TransactionManager.DoWithinTransaction(() => PatternModelSerializationHelper.Instance.LoadModel(store, stream, null, this.serializerLocator), true);

            //// \o/ TODO: fix this 
            //// state.SetLocks(Locks.All);

            return store.GetRootElement();
        }

        private Microsoft.VisualStudio.Modeling.Store CreateStore()
        {
            var extensionTypes = this.extensionLocator.GetExtendingDomainModels(typeof(PatternModelDomainModel))
                .Concat(
                    new[] 
                    { 
                        typeof(CoreDomainModel), 
                        typeof(CoreDesignSurfaceDomainModel), 
                        typeof(PatternModelDomainModel) 
                    }).ToArray();

            return new Store(this.serviceProvider, extensionTypes);
        }
    }
}