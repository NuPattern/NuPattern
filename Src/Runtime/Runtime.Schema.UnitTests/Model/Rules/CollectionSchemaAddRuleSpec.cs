using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuPattern.Extensibility;
using NuPattern.Reflection;

namespace NuPattern.Runtime.Schema.UnitTests
{
    public class CollectionSchemaAddRuleSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "Unfortunate, but unavoidable naming.")]
        public class GivenANewCollection
        {
            private DslTestStore<PatternModelDomainModel> store = new DslTestStore<PatternModelDomainModel>();
            private CollectionSchema collection;

            [TestInitialize]
            public void InitializeContext()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.collection = this.store.ElementFactory.CreateElement<CollectionSchema>();
                });
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenAddRule_ThenCustomizationPolicyIsNotNull()
            {
                Assert.NotNull(this.collection.Policy);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenAddRule_ThenCustomizationPolicyHasRequiredSettings()
            {
                IEnumerable<CustomizableSettingSchema> settings = this.collection.Policy.Settings;

                Assert.Equal(5, settings.Count());
                Assert.NotNull(settings.FirstOrDefault(setting => setting.PropertyId == this.collection.GetPropertyName(element => element.IsVisible)));
                Assert.NotNull(settings.FirstOrDefault(setting => setting.PropertyId == this.collection.GetPropertyName(element => element.Description)));
                Assert.NotNull(settings.FirstOrDefault(setting => setting.PropertyId == this.collection.GetPropertyName(element => element.DisplayName)));
                Assert.NotNull(settings.FirstOrDefault(setting => setting.PropertyId == this.collection.GetPropertyName(element => element.ValidationRules)));
                Assert.NotNull(settings.FirstOrDefault(setting => setting.PropertyId == this.collection.GetPropertyName(element => element.Icon)));
            }
        }
    }
}