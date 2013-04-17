using System.ComponentModel.Composition.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.Runtime.Bindings;
using NuPattern.Runtime.Composition;

namespace NuPattern.Runtime.UnitTests.Binding
{
    [TestClass]
    public class CompositionServiceBindingContextSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestMethod, TestCategory("Unit")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Test code.")]
        public void WhenAddingExport_ThenCanRetrieveItFromContextContainer()
        {
            var catalog = new TypeCatalog(typeof(Foo));
            var container = new CompositionContainer(catalog);
            var compositionService = new Mock<IFeatureCompositionService>();
            compositionService.Setup(x => x.GetExportedValue<ExportProvider>()).Returns(container);

            var service = new CompositionServiceBindingContext(compositionService.Object);

            var foo = new Foo();

            service.AddExport<IFoo>(foo);

            Assert.Same(foo, service.Container.GetExportedValue<IFoo>());
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Test code.")]
        [TestMethod, TestCategory("Unit")]
        public void WhenAddingExportWithContractName_ThenCanRetrieveItFromContextContainer()
        {
            var catalog = new TypeCatalog(typeof(Foo));
            var container = new CompositionContainer(catalog);
            var compositionService = new Mock<IFeatureCompositionService>();
            compositionService.Setup(x => x.GetExportedValue<ExportProvider>()).Returns(container);

            var service = new CompositionServiceBindingContext(compositionService.Object);

            var foo = new Foo();

            service.AddExport<IFoo>(foo, "Bar");

            Assert.Same(foo, service.Container.GetExportedValue<IFoo>("Bar"));
        }

        public class Foo : IFoo
        {
        }
        public interface IFoo
        {
        }
    }
}
