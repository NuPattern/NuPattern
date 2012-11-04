using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.VisualStudio.Patterning.Extensibility.Binding;
using Microsoft.VisualStudio.Patterning.Runtime;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Microsoft.VisualStudio.Patterning.Extensibility.UnitTests
{
	public class DynamicBindingSpec
	{
		internal static readonly IAssertion Assert = new Assertion();

		[TestClass]
		public class GivenAComponentWithAnImport
		{
			private DynamicBinding<IFoo> binding;

			[TestInitialize]
			[SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
			public void Initialize()
			{
				Reflector<FeaturesGlobalContainer>.GetProperty(x => FeaturesGlobalContainer.Instance).SetValue(
					null,
					new CompositionContainer(),
					null);

				var catalog = new TypeCatalog(typeof(Foo));
				var container = new CompositionContainer(catalog);
				var compositionService = new FeatureCompositionService(container);

				this.binding = new DynamicBinding<IFoo>(new DelegatingCompositionService(compositionService), "Foo");
			}

			[TestMethod]
			public void WhenProvidingDynamicValue_ThenBindingCanResolveIt()
			{
				using (var context = this.binding.CreateDynamicContext())
				{
					// Without the dynamic context.
					var result = this.binding.Evaluate();

					Assert.True(result);
					Assert.Null(this.binding.Value.Bar);

					// With the dynamic context.
					var bar = new Bar();
					context.AddExport<IBar>(bar);
					result = this.binding.Evaluate(context);

					Assert.True(result);
					Assert.NotNull(this.binding.Value.Bar);
				}
			}

			[TestMethod]
			public void WhenDisposingDynamicContext_ThenResolvedValueRemainsValid()
			{
				var context = this.binding.CreateDynamicContext();

				// With the dynamic context.
				var bar = new Bar();
				context.AddExport<IBar>(bar);
				var result = this.binding.Evaluate(context);

				Assert.True(result);
				Assert.NotNull(this.binding.Value.Bar);

				context.Dispose();

				Assert.NotNull(this.binding.Value.Bar);
			}

			[TestMethod]
			public void WhenEvaluatingWithNullContext_ThenThrowsArgumentNullException()
			{
				Assert.Throws<ArgumentNullException>(() => this.binding.Evaluate(null));
			}

			[TestMethod]
			public void WhenEvaluatingWithNonOwnedContext_ThenThrowsArgumentException()
			{
				Assert.Throws<ArgumentException>(() => this.binding.Evaluate(new Mock<IDynamicBindingContext>().Object));
			}

			[TestMethod]
			public void WhenDisposingDynamicContextAndReevaluating_ThenResolvedValueBecomesInvalid()
			{
				using (var context = this.binding.CreateDynamicContext())
				{
					var bar = new Bar();
					context.AddExport<IBar>(bar);
					var result = this.binding.Evaluate(context);

					Assert.True(result);
					Assert.NotNull(this.binding.Value.Bar);
				}

				this.binding.Evaluate();
				Assert.Null(this.binding.Value.Bar);
			}

			[TestMethod]
			[SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Test code.")]
			public void WhenCompositionServiceProvidesExportProvider_ThenChainsDynamicContextWithIt()
			{
				var catalog = new TypeCatalog(typeof(Foo));
				var container = new CompositionContainer(catalog);
				var compositionService = new Mock<IFeatureCompositionService>();
				compositionService.Setup(x => x.GetExportedValue<ExportProvider>()).Returns(container);
				compositionService.Setup(x => x.GetExports<IFoo, IFeatureComponentMetadata>())
					.Returns(new[] 
					{ 
						new Lazy<IFoo, IFeatureComponentMetadata>(
						() => new Foo(), 
						Mocks.Of<IFeatureComponentMetadata>().First(m => 
							m.CatalogName == Microsoft.VisualStudio.TeamArchitect.PowerTools.Constants.CatalogName && 
							m.Id == "Foo"))
					});

				var binding = new DynamicBinding<IFoo>(new DelegatingCompositionService(compositionService.Object), "Foo");

				// Make sure the mocking so far is going good.
				Assert.True(binding.Evaluate());

				using (var context = binding.CreateDynamicContext())
				{
					var bar = new Bar();
					context.AddExport<IBar>(bar);
					var result = binding.Evaluate(context);

					Assert.True(result);
					Assert.NotNull(binding.Value.Bar);
				}
			}

			[SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope"), TestMethod]
			public void WhenAddedExportExistsOnBase_ThenOverridesIt()
			{
				var catalog = new TypeCatalog(typeof(Foo), typeof(Bar));
				var container = new CompositionContainer(catalog);
				var compositionService = new FeatureCompositionService(container);

				var binding = new DynamicBinding<IFoo>(new DelegatingCompositionService(compositionService), "Foo");
				Assert.True(binding.Evaluate());
				Assert.NotNull(binding.Value.Bar);

				using (var context = binding.CreateDynamicContext())
				{
					var bar = new Bar();
					context.AddExport<IBar>(bar);
					var result = binding.Evaluate(context);

					Assert.True(result);
					Assert.NotNull(binding.Value.Bar);
					Assert.Same(bar, binding.Value.Bar);
				}
			}

			[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope"), TestMethod]
			public void WhenAddedExportExistsOnBase_ThenImportManyGetsBoth()
			{
				var catalog = new TypeCatalog(typeof(Foo), typeof(Bar));
				var container = new CompositionContainer(catalog);
				var compositionService = new FeatureCompositionService(container);

				var binding = new DynamicBinding<IFoo>(new DelegatingCompositionService(compositionService), "Foo");
				Assert.True(binding.Evaluate());
				Assert.Equal(1, binding.Value.Bars.Count());

				using (var context = binding.CreateDynamicContext())
				{
					var bar = new Bar();
					context.AddExport<IBar>(bar);
					var result = binding.Evaluate(context);

					Assert.True(result);
					Assert.Equal(2, binding.Value.Bars.Count());
				}
			}
		}

		public interface IFoo
		{
			IBar Bar { get; set; }
			IEnumerable<IBar> Bars { get; set; }
		}

		[FeatureComponent(typeof(IFoo), Id = "Foo", Category = "Category", Description = "Description", DisplayName = "DisplayName")]
		[FeatureComponentCatalog(typeof(Foo))]
		public class Foo : IFoo
		{
			// TODO: when bug BlueTab-PLATU11 is fixed, remove the AllowDefault and make it fail.
			[Import(AllowDefault = true)]
			public IBar Bar { get; set; }

			[ImportMany]
			public IEnumerable<IBar> Bars { get; set; }
		}

		[Export(typeof(IBar))]
		public class Bar : IBar
		{
		}

		public interface IBar
		{
		}

		[MetadataAttribute]
		[AttributeUsage(AttributeTargets.Class)]
		public sealed class FeatureComponentCatalogAttribute : Attribute
		{
			public FeatureComponentCatalogAttribute(Type exportingType)
			{
				this.ExportingType = exportingType;
			}

			public Type ExportingType { get; private set; }

			public static string CatalogName
			{
				get { return Microsoft.VisualStudio.TeamArchitect.PowerTools.Constants.CatalogName; }
			}
		}
	}
}
