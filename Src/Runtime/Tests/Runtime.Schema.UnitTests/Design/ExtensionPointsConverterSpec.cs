using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.Runtime.Schema.Design;

namespace NuPattern.Runtime.Schema.UnitTests.Design
{
    [TestClass]
    public class ExtensionPointsConverterSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        private ITypeDescriptorContext context;
        private ExtensionPointsConverter converter;

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
            info.SetupGet(i => i.Id).Returns("FooId");
            patternManager.SetupGet(m => m.InstalledToolkits).Returns(new[] { info.Object });
            mock.Setup(c => c.GetService(typeof(IPatternManager))).Returns(patternManager.Object);

            this.context = mock.Object;
            this.converter = new ExtensionPointsConverter();
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenTryingToConvertFromString_ThenReturnsTrue()
        {
            Assert.True(this.converter.CanConvertFrom(typeof(string)));
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenTryingToConvertFromIn_ThenReturnsFalse()
        {
            Assert.False(this.converter.CanConvertFrom(typeof(int)));
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenConvertingFromValue_ThenReturnsConvertedValue()
        {
            var convertedValue = this.converter.ConvertFrom(this.context, CultureInfo.InvariantCulture, "Foo");
            Assert.NotNull(convertedValue);
            Assert.NotNull(convertedValue as List<IExtensionPointSchema>);
            Assert.Equal(1, ((List<IExtensionPointSchema>)convertedValue).Count);
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenConvertingFromInValidValue_ThenReturnsNull()
        {
            Assert.Null(this.converter.ConvertFrom(this.context, CultureInfo.InvariantCulture, "sdsd"));
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenTryingToConvertToString_ThenReturnsTrue()
        {
            Assert.True(this.converter.CanConvertTo(typeof(string)));
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenConvertingValueToString_ThenReturnsConvertedValue()
        {
            var extension = new Mock<IExtensionPointSchema>();
            extension.SetupGet(e => e.RequiredExtensionPointId).Returns("Bar");
            var extension1 = new Mock<IExtensionPointSchema>();
            extension1.SetupGet(e => e.RequiredExtensionPointId).Returns("Bar1");
            var extensions = new List<IExtensionPointSchema> { extension.Object, extension1.Object };

            var convertedValue = this.converter.ConvertTo(this.context, CultureInfo.InvariantCulture, extensions, typeof(string));
            Assert.Equal("Bar|Bar1", convertedValue as string);
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenConvertingInValidValueToString_ThenReturnsInvalidValue()
        {
            Assert.Equal("sdsd", this.converter.ConvertTo(this.context, CultureInfo.InvariantCulture, "sdsd", typeof(string)));
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenTryingToConvertToInt_ThenReturnsFalse()
        {
            Assert.False(this.converter.CanConvertTo(typeof(int)));
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenGettingStandardValues_ThenReturnsValues()
        {
            var values = this.converter.GetStandardValues(this.context);

            Assert.NotNull(values);
            Assert.Equal(1, values.Count);
            Assert.Equal("Foo", values.Cast<IExtensionPointSchema>().ElementAt(0).RequiredExtensionPointId);
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenGettingFilteredStandardValues_ThenReturnsValues()
        {
            var values = new ExtensionPointsConverter(p => p.Id != "FooId").GetStandardValues(this.context);

            Assert.NotNull(values);
            Assert.Equal(0, values.Count);
        }
    }
}