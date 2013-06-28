using System.ComponentModel;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.ComponentModel;
using NuPattern.Runtime.Schema.Design;

namespace NuPattern.Runtime.Schema.UnitTests.Design
{
    [TestClass]
    public class ExtensionPointConverterSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        private ITypeDescriptorContext context;
        private ExtensionPointConverter converter;

        [TestInitialize]
        public void Initialize()
        {
            var mock = new Mock<ITypeDescriptorContext>();
            var patternManager = new Mock<IPatternManager>();
            var info = new Mock<IInstalledToolkitInfo>();
            var schema = new Mock<IPatternModelInfo>();
            var extension = new Mock<IExtensionPointSchema>();

            extension.SetupGet(e => e.RequiredExtensionPointId).Returns("Foo");
            schema.Setup(s => s.FindAll<IExtensionPointSchema>()).Returns(new[] { extension.Object });
            info.SetupGet(i => i.Schema).Returns(schema.Object);
            patternManager.SetupGet(m => m.InstalledToolkits).Returns(new[] { info.Object });
            mock.Setup(c => c.GetService(typeof(IPatternManager))).Returns(patternManager.Object);

            this.context = mock.Object;
            this.converter = new ExtensionPointConverter();
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenGettingStandardValues_ThenReturnsValues()
        {
            var values = this.converter.GetStandardValues(this.context);

            Assert.NotNull(values);
            Assert.Equal(1, values.Count);
            Assert.Equal("Foo", ((IExtensionPointSchema)values.Cast<StandardValue>().ElementAt(0).Value).RequiredExtensionPointId);
        }


        [TestMethod, TestCategory("Unit")]
        public void WhenGettingFilteredStandardValues_ThenReturnsValues()
        {
            var values = new ExtensionPointConverter(p => p.RequiredExtensionPointId != "Foo").GetStandardValues(this.context);

            Assert.NotNull(values);
            Assert.Equal(0, values.Count);
        }
    }
}