using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.VisualStudio.Patterning.Extensibility.Binding;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.VisualStudio.Patterning.Extensibility.UnitTests
{
	[TestClass]
	[SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test Code")]
	public class ContainerCompositionServiceAdapterSpec
	{
		internal static readonly IAssertion Assert = new Assertion();

		private CompositionContainer container;

		[TestInitialize]
		[SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Test code.")]
		public void Initialize()
		{
			this.container = new CompositionContainer(new TypeCatalog(typeof(Foo)));
		}

		[TestMethod]
		[SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Test code.")]
		public void WhenComposingParts_ThenDelegates()
		{
			var delegating = new ContainerCompositionServiceAdapter(this.container);
			var consumer = new FooConsumer();

			delegating.ComposeParts(consumer);

			Assert.NotNull(consumer.Foo);
		}

		[TestMethod]
		public void WhenDisposing_ThenDoesNotDisposeInnerService()
		{
			var delegating = new ContainerCompositionServiceAdapter(this.container);

			delegating.Dispose();

			var consumer = new FooConsumer();
			delegating.ComposeParts(consumer);

			Assert.NotNull(consumer.Foo);
		}

		[TestMethod]
		[SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Test code.")]
		public void WhenGettingExport_ThenDelegates()
		{
			var delegating = new ContainerCompositionServiceAdapter(this.container);

			var foo = delegating.GetExport<IFoo>();

			Assert.NotNull(foo);
		}

		[TestMethod]
		[SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Test code.")]
		public void WhenGettingExportedValue_ThenDelegates()
		{
			var delegating = new ContainerCompositionServiceAdapter(this.container);

			var foo = delegating.GetExportedValue<IFoo>();

			Assert.NotNull(foo);
		}

		[TestMethod]
		[SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Test code.")]
		public void WhenGettingExportedValueOrDefault_ThenDelegates()
		{
			var delegating = new ContainerCompositionServiceAdapter(this.container);

			var foo = delegating.GetExportedValueOrDefault<IFoo>();

			Assert.NotNull(foo);
			Assert.Null(delegating.GetExportedValueOrDefault<IFormatProvider>());
		}

		[SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Test code.")]
		[TestMethod]
		public void WhenGettingExportedValues_ThenDelegates()
		{
			var delegating = new ContainerCompositionServiceAdapter(this.container);

			var foos = delegating.GetExportedValues<IFoo>();

			Assert.Equal(1, foos.Count());
		}

		[TestMethod]
		[SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Test code.")]
		public void WhenGettingExports_ThenDelegates()
		{
			var delegating = new ContainerCompositionServiceAdapter(this.container);

			var foos = delegating.GetExports<IFoo>();

			Assert.Equal(1, foos.Count());
		}

		[TestMethod]
		[SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Test code.")]
		public void WhenSatisfyingImportsOnce_ThenDelegates()
		{
			var delegating = new ContainerCompositionServiceAdapter(this.container);
			var consumer = new FooConsumer();

			delegating.SatisfyImportsOnce(AttributedModelServices.CreatePart(consumer));

			Assert.NotNull(consumer.Foo);
		}

		public class FooConsumer
		{
			[Import]
			public IFoo Foo { get; set; }
		}

		[Export(typeof(IFoo))]
		[NameMetadata(Name = "Foo")]
		public class Foo : IFoo
		{
		}

		public interface IFoo
		{
		}

		public interface INameMetadata
		{
			string Name { get; }
		}

		[MetadataAttribute]
		[AttributeUsage(AttributeTargets.Class)]
		public sealed class NameMetadataAttribute : Attribute, INameMetadata
		{
			public string Name { get; set; }
		}
	}
}
