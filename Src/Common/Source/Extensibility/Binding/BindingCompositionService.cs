using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.ReflectionModel;
using System.Linq;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Patterning.Extensibility.Properties;
using Microsoft.VisualStudio.Patterning.Runtime;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;

namespace Microsoft.VisualStudio.Patterning.Extensibility.Binding
{
	[PartCreationPolicy(CreationPolicy.Shared)]
	[Export(typeof(IBindingCompositionService))]
	internal class BindingCompositionService : IBindingCompositionService
	{
		private static readonly ITraceSource tracer = Tracer.GetSourceFor<BindingCompositionService>();

		private IServiceProvider serviceProvider;
		private IComponentModel componentModel;
		private CompositionContainer container;
		private ComposablePartCatalog bindingCatalog;
		private CompositionContainer defaultCompositionProvider;

		/// <summary>
		/// Initializes a new instance of the <see cref="FeatureCompositionService"/> class 
		/// with the given underlying container.
		/// </summary>
		[ImportingConstructor]
		public BindingCompositionService([Import(typeof(SVsServiceProvider))] IServiceProvider serviceProvider)
		{
			Guard.NotNull(() => serviceProvider, serviceProvider);

			this.serviceProvider = serviceProvider;
			this.componentModel = this.serviceProvider.GetService<SComponentModel, IComponentModel>();

			CreateBindingContainer();
		}

		public IDynamicBindingContext CreateDynamicContext()
		{
			var bindingProvider = new CatalogExportProvider(this.bindingCatalog);
			var dynamicExports = new CompositionContainer();
			var composition = new CompositionContainer(
				dynamicExports,
				defaultCompositionProvider,
				bindingProvider,
				componentModel.DefaultExportProvider);

			bindingProvider.SourceProvider = composition;

			var context = new CompositionContainerDynamicBindingContext(dynamicExports, composition);
			context.AddExport<IDynamicBindingContext>(context);

			return context;
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		private void CreateBindingContainer()
		{
			// We leverage the Feature Runtime container, where all their components and extensions publish already.
			var defaultCatalog = this.componentModel.GetCatalog("Microsoft.VisualStudio.Default");
			var featuresCatalog = this.componentModel.GetCatalog(Microsoft.VisualStudio.TeamArchitect.PowerTools.Constants.CatalogName);
			if (defaultCatalog == null || featuresCatalog == null)
				throw new InvalidOperationException(Resources.BindingCompositionService_CatalogsNotAvailable);

			try
			{
				// Transparently change the IFeatureCompositionService implementation for all components 
				// without code changes by providing an instance earlier in the chain.
				// See http://codebetter.com/blogs/glenn.block/archive/2009/05/14/customizing-container-behavior-part-2-of-n-defaults.aspx
				this.defaultCompositionProvider = new CompositionContainer();
				defaultCompositionProvider.ComposeExportedValue<IFeatureCompositionService>(this);
				defaultCompositionProvider.ComposeExportedValue<SVsServiceProvider>((SVsServiceProvider)serviceProvider);

				// Decorated catalog of parts.
				// NOTE: caching the catalog also caches the instantiated shared parts, if any.
				this.bindingCatalog = new BindingComponentCatalog(new AggregateCatalog(featuresCatalog, defaultCatalog));
				var bindingProvider = new CatalogExportProvider(this.bindingCatalog);

				this.container = new CompositionContainer(
					defaultCompositionProvider,
					bindingProvider,
					componentModel.DefaultExportProvider);

				bindingProvider.SourceProvider = this.container;
			}
			catch (Exception ex)
			{
				tracer.TraceError(ex, Resources.BindingCompositionService_FailedToInitialize);
				throw;
			}

			// Used by the CompositionServiceBindingContext to create a new CompositionContainer over 
			// this one for dynamic context resolution.
			this.container.ComposeExportedValue<ExportProvider>(container);
		}

		/// <summary>
		/// See <see cref="ICompositionService.SatisfyImportsOnce"/>.
		/// </summary>
		public void SatisfyImportsOnce(ComposablePart part)
		{
			this.container.SatisfyImportsOnce(part);
		}

		public void ComposeParts(params object[] attributedParts)
		{
			this.container.ComposeParts(attributedParts);
		}

		public Lazy<T> GetExport<T>()
		{
			return this.container.GetExport<T>();
		}

		public Lazy<T> GetExport<T>(string contractName)
		{
			return this.container.GetExport<T>(contractName);
		}

		public Lazy<T, TMetadataView> GetExport<T, TMetadataView>()
		{
			return this.container.GetExport<T, TMetadataView>();
		}

		public Lazy<T, TMetadataView> GetExport<T, TMetadataView>(string contractName)
		{
			return this.container.GetExport<T, TMetadataView>(contractName);
		}

		public T GetExportedValue<T>()
		{
			return this.container.GetExportedValue<T>();
		}

		public T GetExportedValue<T>(string contractName)
		{
			return this.container.GetExportedValue<T>(contractName);
		}

		public T GetExportedValueOrDefault<T>()
		{
			return this.container.GetExportedValueOrDefault<T>();
		}

		public T GetExportedValueOrDefault<T>(string contractName)
		{
			return this.container.GetExportedValueOrDefault<T>(contractName);
		}

		public IEnumerable<T> GetExportedValues<T>()
		{
			return this.container.GetExportedValues<T>();
		}

		public IEnumerable<T> GetExportedValues<T>(string contractName)
		{
			return this.container.GetExportedValues<T>(contractName);
		}

		public IEnumerable<Lazy<T>> GetExports<T>()
		{
			return this.container.GetExports<T>();
		}

		public IEnumerable<Lazy<T>> GetExports<T>(string contractName)
		{
			return this.container.GetExports<T>(contractName);
		}

		public IEnumerable<Lazy<T, TMetadataView>> GetExports<T, TMetadataView>()
		{
			return this.container.GetExports<T, TMetadataView>();
		}

		public IEnumerable<Lazy<T, TMetadataView>> GetExports<T, TMetadataView>(string contractName)
		{
			return this.container.GetExports<T, TMetadataView>(contractName);
		}

		public void Dispose()
		{
			this.container.Dispose();
		}

		private class CompositionContainerDynamicBindingContext : IDynamicBindingContext
		{
			private CompositionContainer container;
			private CompositionContainer dynamicExports;

			public CompositionContainerDynamicBindingContext(CompositionContainer dynamicExports, CompositionContainer container)
			{
				this.dynamicExports = dynamicExports;
				this.container = container;
			}

			public ICompositionService CompositionService
			{
				get { return this.container; }
			}

			public void AddExport<T>(T instance) where T : class
			{
				this.dynamicExports.ComposeExportedValue(instance);
			}

			public void AddExport<T>(T instance, string contractName) where T : class
			{
				this.dynamicExports.ComposeExportedValue(contractName, instance);
			}

			public void Dispose()
			{
				this.dynamicExports.Dispose();
			}
		}

		/// <summary>
		/// This catalog only provides the exports for things that have the <see cref="FeatureComponentAttribute"/> attribute.
		/// </summary>
		private class BindingComponentCatalog : FeatureComponentCatalog
		{
			private bool initialized;
			private List<ComposablePartDefinition> sharedParts = new List<ComposablePartDefinition>();
			private List<ComposablePartDefinition> nonSharedParts = new List<ComposablePartDefinition>();

			public BindingComponentCatalog(ComposablePartCatalog innerCatalog)
				: base(innerCatalog)
			{
			}

			public override IQueryable<ComposablePartDefinition> Parts
			{
				get
				{
					if (!initialized)
						Initialize(base.Parts);

					return this.sharedParts.Concat(CloneNonSharedParts()).AsQueryable();
				}
			}

			/// <summary>
			/// Clones the non-shared to avoid object instance reuse, 
			/// which happens if you cache the part definition.
			/// </summary>
			private IEnumerable<ComposablePartDefinition> CloneNonSharedParts()
			{
				return this.nonSharedParts
					.AsParallel()
					.Where(part => part != null)
					.Select(def => ReflectionModelServices.CreatePartDefinition(
						ReflectionModelServices.GetPartType(def),
						true,
						new Lazy<IEnumerable<ImportDefinition>>(() => def.ImportDefinitions),
						new Lazy<IEnumerable<ExportDefinition>>(() => def.ExportDefinitions),
						new Lazy<IDictionary<string, object>>(() => def.Metadata),
						this));
			}

			private void Initialize(IQueryable<ComposablePartDefinition> parts)
			{
				var partsInfo = parts
					.AsParallel()
					.Where(part => part != null)
					.Select(part => new { Part = part, IsShared = IsShared(part) });

				sharedParts.AddRange(partsInfo.Where(part => part.IsShared).Select(part => part.Part));
				nonSharedParts.AddRange(partsInfo.Where(part => !part.IsShared).Select(part => part.Part));

				initialized = true;
			}


			private static bool IsShared(ComposablePartDefinition def)
			{
				return def.Metadata.ContainsKey(CompositionConstants.PartCreationPolicyMetadataName) &&
					(CreationPolicy)def.Metadata[CompositionConstants.PartCreationPolicyMetadataName] == CreationPolicy.Shared;
			}
		}
	}
}
