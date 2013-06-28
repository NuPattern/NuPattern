using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.ComponentModel;
using NuPattern.Runtime.Design;
using NuPattern.Runtime.Guidance;

namespace NuPattern.Runtime.UnitTests.Design
{
    [TestClass]
    public class GuidanceExtensionsTypeConverterSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public class GivenNoContext
        {
            [TestMethod, TestCategory("Unit")]
            public void WhenGettingStandardValues_ThenReturnsGuidanceExtensions()
            {
                var manager = new Mock<IGuidanceManager>();
                manager.SetupGet(m => m.InstalledGuidanceExtensions)
                    .Returns(Mocks.Of<IGuidanceExtensionRegistration>().Where(fx => fx.ExtensionId == "FooId" && fx.ExtensionManifest.Header.Description == "Bar").Take(1).ToArray());
                manager.SetupGet(m => m.InstantiatedGuidanceExtensions)
                    .Returns(Mocks.Of<IGuidanceExtension>().Where(fx => fx.ExtensionId == "FooId" && fx.InstanceName == "Foo").Take(1).ToArray());

                var context = Mocks.Of<ITypeDescriptorContext>().First(c => c.GetService(typeof(IGuidanceManager)) == manager.Object);

                var converter = new GuidanceExtensionsTypeConverter();
                var values = converter.GetStandardValues(context);

                Assert.Equal(2, values.Count); //// "(none)" is being injected

                var standardValue = values.Cast<StandardValue>().FirstOrDefault(s => s.DisplayText != "(None)");
                Assert.Equal("Foo", standardValue.DisplayText);
                Assert.Equal("Bar", standardValue.Description);
                Assert.Equal("Foo", standardValue.Value);
            }
        }
    }
}