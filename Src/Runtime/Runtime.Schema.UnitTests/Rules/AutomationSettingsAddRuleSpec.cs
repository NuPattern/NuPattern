using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuPattern.Modeling;
using NuPattern.Reflection;

namespace NuPattern.Runtime.Schema.UnitTests
{
    [TestClass]
    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
    public class AutomationSettingsAddRuleSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public class GivenANewAutomationSettings
        {
            private DslTestStore<PatternModelDomainModel> store = new DslTestStore<PatternModelDomainModel>();
            private AutomationSettingsSchema automation;

            [TestInitialize]
            public void InitializeContext()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.automation = this.store.ElementFactory.CreateElement<AutomationSettingsSchema>();
                });
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenAddRule_ThenCustomizationPolicyIsNotNull()
            {
                Assert.NotNull(this.automation.Policy);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenAddRule_ThenCustomizationPolicyHasRequiredSettings()
            {
                IEnumerable<CustomizableSettingSchema> settings = this.automation.Policy.Settings;

                Assert.Equal(3, settings.Count());
                Assert.NotNull(settings.FirstOrDefault(setting => setting.PropertyId == this.automation.GetPropertyName(element => element.Description)));
                Assert.NotNull(settings.FirstOrDefault(setting => setting.PropertyId == this.automation.GetPropertyName(element => element.DisplayName)));
                Assert.NotNull(settings.FirstOrDefault(setting => setting.PropertyId == this.automation.GetPropertyName(element => element.Settings)));
            }
        }
    }
}