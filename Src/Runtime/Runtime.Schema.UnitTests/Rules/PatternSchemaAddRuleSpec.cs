using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuPattern.Modeling;
using NuPattern.Reflection;

namespace NuPattern.Runtime.Schema.UnitTests
{
    public class PatternSchemaAddRuleSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public class GivenANewPattern
        {
            private DslTestStore<PatternModelDomainModel> store = new DslTestStore<PatternModelDomainModel>();
            private PatternSchema pattern;

            [TestInitialize]
            public void InitializeContext()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.pattern = this.store.ElementFactory.CreateElement<PatternSchema>();
                });
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenIsCustomizatableDefaultsToTrue()
            {
                Assert.Equal(CustomizationState.True, this.pattern.IsCustomizable);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenLoadingAnExistingPatternWithIsCustomizableInherited_ThenIsCustomizableIsTrue()
            {
                using (this.store.Store.TransactionManager.BeginTransaction("Loading", true))
                {
                    Assert.Equal<CustomizationState>(this.pattern.IsCustomizable, CustomizationState.True);
                }
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenAddRule_ThenCustomizationPolicyIsNotNull()
            {
                Assert.NotNull(this.pattern.Policy);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenAddRule_ThenCustomizationPolicyHasRequiredSettings()
            {
                IEnumerable<CustomizableSettingSchema> settings = this.pattern.Policy.Settings;

                Assert.Equal(4, settings.Count());
                Assert.NotNull(settings.FirstOrDefault(setting => setting.PropertyId == this.pattern.GetPropertyName(element => element.Description)));
                Assert.NotNull(settings.FirstOrDefault(setting => setting.PropertyId == this.pattern.GetPropertyName(element => element.DisplayName)));
                Assert.NotNull(settings.FirstOrDefault(setting => setting.PropertyId == this.pattern.GetPropertyName(element => element.ValidationRules)));
                Assert.NotNull(settings.FirstOrDefault(setting => setting.PropertyId == this.pattern.GetPropertyName(element => element.Icon)));
            }
        }
    }
}