using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuPattern.Extensibility.Bindings;

namespace NuPattern.Extensibility.UnitTests.Binding
{
    public class BoundPropertySpec
    {
        private static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class Given
        {
            private BoundProperty property;

            private string Property
            {
                get;
                set;
            }

            [TestInitialize]
            public void InitializeContext()
            {
                this.property = new BoundProperty("Property",
                        () => this.Property,
                        s => this.Property = s);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenSettingsHaveAValue_ThenSettingsPersisted()
            {
                this.property.Settings = new PropertyBindingSettings { Name = "Property", Value = "Foo", ValueProvider = null };

                Assert.Equal("{\r\n  \"Name\": \"Property\",\r\n  \"Value\": \"Foo\",\r\n  \"ValueProvider\": null\r\n}", this.Property);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenSettingsHaveAValueProvider_ThenSettingsPersisted()
            {
                this.property.Settings = new PropertyBindingSettings { Name = "Property", Value = "", ValueProvider = new ValueProviderBindingSettings { TypeId = "Foo" } };

                Assert.Equal("{\r\n  \"Name\": \"Property\",\r\n  \"Value\": \"\",\r\n  \"ValueProvider\": {\r\n    \"TypeId\": \"Foo\",\r\n    \"Properties\": []\r\n  }\r\n}", this.Property);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenSettingsHasNoValueOrValueProvider_ThenNothingPersisted()
            {
                this.property.Settings = new PropertyBindingSettings { Name = "Property", Value = "", ValueProvider = null };

                Assert.Equal(string.Empty, this.Property);

                this.property.Settings = new PropertyBindingSettings { Name = "Property", Value = "", ValueProvider = new ValueProviderBindingSettings() };

                Assert.Equal(string.Empty, this.Property);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenPropertyIsEmpty_ThenSettingsCreatedBlank()
            {
                this.Property = string.Empty;

                Assert.Equal("Property", this.property.Settings.Name);
                Assert.Equal(string.Empty, this.property.Settings.Value);
                Assert.Equal(null, this.property.Settings.ValueProvider);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenPropertyIsNull_ThenSettingsCreatedBlank()
            {
                this.Property = null;

                Assert.Equal("Property", this.property.Settings.Name);
                Assert.Equal(string.Empty, this.property.Settings.Value);
                Assert.Equal(null, this.property.Settings.ValueProvider);
            }
        }
    }
}
