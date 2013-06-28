using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuPattern.Runtime.Bindings;

namespace NuPattern.Runtime.UnitTests.Binding
{
    public class BoundPropertySpec
    {
        private static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenNoContext
        {
            [TestMethod, TestCategory("Unit")]
            public void WhenInitializingSettings_ThenHasPropertyName()
            {
                var property = new BoundProperty("Name", () => "", s => { });

                Assert.Equal("Name", property.Settings.Name);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenValueIsEmpty_ThenSettingsInitializedEmpty()
            {
                var property = new BoundProperty("Name", () => "", s => { });

                Assert.Equal("", property.Settings.Value);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenChangingSettingsValue_ThenAutomaticallySerializesValue()
            {
                var value = "";
                var property = new BoundProperty("Name", () => value, s => value = s);

                property.Settings.Value = "Hello";

                Assert.NotEqual("", value);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenReassigningSettings_ThenSavesNewValue()
            {
                var value = "";
                var property = new BoundProperty("Name", () => value, s => value = s);
                var settings = property.Settings;

                property.Settings = new PropertyBindingSettings { Name = "Name", Value = "Hello" };

                Assert.NotEqual("", value);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenReassigningSettings_ThenDetachesChangesFromOlder()
            {
                var value = "";
                var property = new BoundProperty("Name", () => value, s => value = s);
                var settings = property.Settings;

                property.Settings = new PropertyBindingSettings { Name = "Name", Value = "Hello" };

                settings.Value = "Foo";

                Assert.False(value.Contains("Foo"));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenExistingLegacyRawValueExists_ThenAutomaticallyUpgradesToBindingValue()
            {
                var value = "Foo";
                var property = new BoundProperty("Name", () => value, s => value = s);

                Assert.Equal("Foo", property.Settings.Value);
                Assert.True(value.Trim().StartsWith("{"));
            }
        }

        [TestClass]
        public class GivenBoundProperty
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
