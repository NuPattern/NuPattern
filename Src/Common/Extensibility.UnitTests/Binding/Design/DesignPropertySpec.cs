using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.Extensibility.Binding;
using NuPattern.Runtime;

namespace NuPattern.Extensibility.UnitTests.Binding.Design
{
    public class DesignPropertySpec
    {
        private static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenAStringProperty
        {
            private Mock<IPropertyBindingSettings> settings;
            private DesignProperty property;

            [TestInitialize]
            public void InitializeContext()
            {
                this.settings = new Mock<IPropertyBindingSettings>();
                this.property = new DesignProperty(this.settings.Object)
                    {
                        Attributes = new Attribute[0],
                        Type = typeof(string)
                    };
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenSettingsValueIsNull_ThenGetValueReturnsDefault()
            {
                this.settings.Setup(s => s.Value).Returns((string)null);

                Assert.Equal(string.Empty, this.property.GetValue());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenSettingsValueIsEmpty_ThenGetValueReturnsDefault()
            {
                this.settings.Setup(s => s.Value).Returns(string.Empty);

                Assert.Equal(string.Empty, this.property.GetValue());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenSettingsValueIsNotEmpty_ThenGetValueReturnsValue()
            {
                this.settings.Setup(s => s.Value).Returns("Foo");

                Assert.Equal("Foo", this.property.GetValue());
            }
        }

        [TestClass]
        public class GivenAValueTypeProperty
        {
            private Mock<IPropertyBindingSettings> settings;
            private DesignProperty property;

            [TestInitialize]
            public void InitializeContext()
            {
                this.settings = new Mock<IPropertyBindingSettings>();
                this.property = new DesignProperty(this.settings.Object)
                {
                    Attributes = new Attribute[0],
                    Type = typeof(int)
                };
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenSettingsValueIsNull_ThenGetValueReturnsDefault()
            {
                this.settings.Setup(s => s.Value).Returns((string)null);

                Assert.Equal(string.Empty, this.property.GetValue());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenSettingsValueIsStringEmpty_ThenGetValueReturnsDefault()
            {
                this.settings.Setup(s => s.Value).Returns(string.Empty);

                Assert.Equal(string.Empty, this.property.GetValue());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenSettingsValueIsNotEmpty_ThenGetValueReturnsValue()
            {
                this.settings.Setup(s => s.Value).Returns(0.ToString());

                Assert.Equal(0, this.property.GetValue());
            }
        }
    }
}
