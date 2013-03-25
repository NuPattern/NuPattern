using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.Extensibility.Bindings;
using NuPattern.Runtime.Bindings;

namespace NuPattern.Extensibility.UnitTests.Binding
{
    [TestClass]
    public class DynamicBindingContextExtensionsSpec
    {
        [TestMethod, TestCategory("Unit")]
        public void WhenAddingAllInterfaces_ThenAddsExportsToContext()
        {
            var context = new Mock<IDynamicBindingContext>();
            var foo = new Foo();

            context.Object.AddExportsFromInterfaces(foo);

            context.Verify(x => x.AddExport<IFoo>(foo));
            context.Verify(x => x.AddExport<IBar>(foo));
        }

        public class Foo : IFoo { }
        public interface IFoo : IBar { }
        public interface IBar { }
    }
}
