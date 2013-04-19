using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.ReflectionModel;
using System.Linq;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.ExtensibilityHosting;
using Microsoft.VisualStudio.Shell;
using NuPattern.ComponentModel.Composition;
using NuPattern.Diagnostics;

namespace NuPattern.Runtime.Composition
{
    /// <summary>
    /// Provides access to the composition services.
    /// </summary>
    public class NuPatternGlobalContainer : CompositionContainer
    // inheriting CompositionContainer is a workaround for a MEF issue in Beta2
    // that prevents us from using CompositionContainer as a contract type 
    // but with a different contract name.
    {
        static readonly ITraceSource tracer = Tracer.GetSourceFor<NuPatternGlobalContainer>();

        /// <summary>
        /// Contract name of the exported global container.
        /// </summary>
        public static readonly string ExportedContractName = typeof(NuPatternGlobalContainer).FullName;

        /// <summary>
        /// Gets the container
        /// </summary>
        [Export(typeof(NuPatternGlobalContainer))]
        public static CompositionContainer Instance { get; internal set; }

        static NuPatternGlobalContainer()
        {
            var globalComponentModel = (IComponentModel)Package.GetGlobalService(typeof(SComponentModel));
            if (globalComponentModel == null)
            {
                tracer.TraceWarning("Could not retrieve the VS global IComponentModel service.");
            }
            else
            {
                var catalog = globalComponentModel.GetCatalog(NuPattern.ComponentModel.Composition.Catalog.CatalogName);
                if (catalog == null)
                {
                    tracer.TraceWarning("Could not retrieve the catalog '{0}' from the global IComponentModel service.", NuPattern.ComponentModel.Composition.Catalog.CatalogName);
                }
                else
                {
                    try
                    {
                        //
                        // Create our DecoratingReflectionCatalog
                        //
                        var componentsCatalog = new ComponentCatalog(catalog);

                        //
                        // Now create a catalog provider
                        //
                        CatalogExportProvider catalogProvider = new CatalogExportProvider(componentsCatalog);

                        //
                        // have it inherit the global provider
                        //
                        catalogProvider.SourceProvider = globalComponentModel.DefaultExportProvider;

                        //
                        // Create provider settings to look here first and then include others when not found
                        //
                        var providerSettings = new VsExportProviderSettings(
                            VsExportProvidingPreference.BeforeExportsFromOtherContainers,
                            VsExportSharingPolicy.IncludeExportsFromOthers);

                        Instance = VsCompositionContainer.Create(componentsCatalog, providerSettings);
                    }
                    catch (Exception ex)
                    {
                        tracer.TraceError(ex, "Failed to initialize global container");
                        throw;
                    }
                }
            }
        }

        // Implements the "innovation" interface that VsCompositionContainer requires but does not 
        // make explicit in the API in any way.
        class VsContractNameProviderCatalogExportProvider : CatalogExportProvider //, IVsContractNameProvider
        {
            public VsContractNameProviderCatalogExportProvider(ComposablePartCatalog catalog)
                : base(catalog)
            {
            }

            public IDictionary<string, IList<string>> Exports
            {
                get
                {
                    return base.Catalog.Parts.ToDictionary(
                        part => ReflectionModelServices.GetPartType(part).Value.Name,
                        part => (IList<string>)part.ExportDefinitions.Select(export => export.ContractName).ToList());
                }
            }
        }
    }
}