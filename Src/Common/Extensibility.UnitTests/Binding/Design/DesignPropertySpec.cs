using System;
using Microsoft.VisualStudio.Patterning.Extensibility.Binding;
using Microsoft.VisualStudio.Patterning.Runtime;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Microsoft.VisualStudio.Patterning.Extensibility.UnitTests.Binding.Design
{
    public class DesignPropertySpec
    {
        private static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenAStringProperty
        {
            private DesignProperty property;
            private Mock<IPropertyBindingSettings> settings;

            [TestInitialize]
            public void InitializeContext()
            {
                this.settings = new Mock<IPropertyBindingSettings>();
                this.property = new DesignProperty(this.settings.Object);
                this.property.Attributes = new Attribute[0];
                this.property.Type = typeof(string);
            }

            [TestMethod]
            public void WhenSettingsValueIsEmpty_ThenGetValueReturnsNull()
            {
                this.settings.Setup(s => s.Value).Returns(string.Empty);

                Assert.Null(this.property.GetValue());
            }

            [TestMethod]
            public void WhenSettingsValueIsNotEmpty_ThenGetValueReturnsValue()
            {
                this.settings.Setup(s => s.Value).Returns("Foo");

                Assert.Equal("Foo", this.property.GetValue());
            }
        }
    }
}
