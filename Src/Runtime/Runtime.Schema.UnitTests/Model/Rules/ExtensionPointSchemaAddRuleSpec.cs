using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.VisualStudio.Patterning.Runtime.Schema.UnitTests
{
    public class ExtensionPointSchemaAddRuleSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public class GivenANewExtensionPoiint
        {
            private DslTestStore<PatternModelDomainModel> store = new DslTestStore<PatternModelDomainModel>();
            private ExtensionPointSchema extensionPoint;

            [TestInitialize]
            public void InitializeContext()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.extensionPoint = this.store.ElementFactory.CreateElement<ExtensionPointSchema>();
                });
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenAddRule_ThenCustomizationPolicyIsNotNull()
            {
                Assert.NotNull(this.extensionPoint.Policy);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenAddRule_ThenCustomizationPolicyHasRequiredSettings()
            {
                IEnumerable<CustomizableSettingSchema> settings = this.extensionPoint.Policy.Settings;

                Assert.Equal(5, settings.Count());
                Assert.NotNull(settings.FirstOrDefault(setting => setting.PropertyId == this.extensionPoint.GetPropertyName(element => element.Conditions)));
                Assert.NotNull(settings.FirstOrDefault(setting => setting.PropertyId == this.extensionPoint.GetPropertyName(element => element.Description)));
                Assert.NotNull(settings.FirstOrDefault(setting => setting.PropertyId == this.extensionPoint.GetPropertyName(element => element.DisplayName)));
                Assert.NotNull(settings.FirstOrDefault(setting => setting.PropertyId == this.extensionPoint.GetPropertyName(element => element.ValidationRules)));
                Assert.NotNull(settings.FirstOrDefault(setting => setting.PropertyId == this.extensionPoint.GetPropertyName(element => element.Icon)));
            }
        }
    }
}