using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuPattern.Extensibility.Bindings;
using NuPattern.Runtime.Bindings;

namespace NuPattern.Extensibility.UnitTests.Binding
{
    public class PropertyBindingSettingsSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenSettings
        {
            private IPropertyBindingSettings settings;

            [TestInitialize]
            public void InitializeContext()
            {
                this.settings = new PropertyBindingSettings
                {
                    Value = "Foo",
                    ValueProvider = new ValueProviderBindingSettings
                    {
                        TypeId = "Bar",
                    },
                };
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenResetValue_ThenValueIsEmpty()
            {
                this.settings.Reset();

                Assert.Equal(string.Empty, settings.Value);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenResetValue_ThenValueProviderIsNull()
            {
                this.settings.Reset();

                Assert.Equal(null, settings.ValueProvider);
            }
        }
    }
}
