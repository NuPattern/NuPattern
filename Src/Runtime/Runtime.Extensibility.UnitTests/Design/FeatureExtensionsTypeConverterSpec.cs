using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.ComponentModel;
using NuPattern.Runtime.Design;

namespace NuPattern.Runtime.UnitTests.Design
{
    [TestClass]
    public class FeatureExtensionsTypeConverterSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public class GivenNoContext
        {
            [TestMethod, TestCategory("Unit")]
            public void WhenGettingStandardValues_ThenReturnsFeatureExtensions()
            {
                var manager = new Mock<IFeatureManager>();
                manager.SetupGet(m => m.InstalledFeatures)
                    .Returns(Mocks.Of<IFeatureRegistration>().Where(fx => fx.FeatureId == "FooId" && fx.ExtensionManifest.Header.Description == "Bar").Take(1).ToArray());
                manager.SetupGet(m => m.InstantiatedFeatures)
                    .Returns(Mocks.Of<IFeatureExtension>().Where(fx => fx.FeatureId == "FooId" && fx.InstanceName == "Foo").Take(1).ToArray());

                var context = Mocks.Of<ITypeDescriptorContext>().First(c => c.GetService(typeof(IFeatureManager)) == manager.Object);

                var converter = new FeatureExtensionsTypeConverter();
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