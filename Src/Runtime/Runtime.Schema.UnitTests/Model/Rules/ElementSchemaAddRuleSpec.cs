using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuPattern.Extensibility;

namespace NuPattern.Runtime.Schema.UnitTests
{
    [TestClass]
    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
    public class ElementSchemaAddRuleSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public class GivenANewElement
        {
            private DslTestStore<PatternModelDomainModel> store = new DslTestStore<PatternModelDomainModel>();
            private ElementSchema element;

            [TestInitialize]
            public void InitializeContext()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.element = this.store.ElementFactory.CreateElement<ElementSchema>();
                });
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenAddRule_ThenCustomizationPolicyIsNotNull()
            {
                Assert.NotNull(this.element.Policy);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenAddRule_ThenCustomizationPolicyHasRequiredSettings()
            {
                IEnumerable<CustomizableSettingSchema> settings = this.element.Policy.Settings;

                Assert.Equal(5, settings.Count());
                Assert.NotNull(settings.FirstOrDefault(setting => setting.PropertyId == this.element.GetPropertyName(element => element.IsVisible)));
                Assert.NotNull(settings.FirstOrDefault(setting => setting.PropertyId == this.element.GetPropertyName(element => element.Description)));
                Assert.NotNull(settings.FirstOrDefault(setting => setting.PropertyId == this.element.GetPropertyName(element => element.DisplayName)));
                Assert.NotNull(settings.FirstOrDefault(setting => setting.PropertyId == this.element.GetPropertyName(element => element.ValidationRules)));
                Assert.NotNull(settings.FirstOrDefault(setting => setting.PropertyId == this.element.GetPropertyName(element => element.Icon)));
            }
        }
    }
}