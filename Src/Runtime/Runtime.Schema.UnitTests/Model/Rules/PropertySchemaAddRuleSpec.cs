using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuPattern.Extensibility;
using NuPattern.Reflection;

namespace NuPattern.Runtime.Schema.UnitTests
{
    [TestClass]
    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
    public class PropertySchemaAddRuleSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public class GivenANewProperty
        {
            private DslTestStore<PatternModelDomainModel> store = new DslTestStore<PatternModelDomainModel>();
            private PatternModelSchema patternModel;
            private PropertySchema property;

            [TestInitialize]
            public void InitializeContext()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.patternModel = this.store.ElementFactory.CreateElement<PatternModelSchema>();
                    this.property = patternModel.Create<PatternSchema>().Create<PropertySchema>();
                });
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenAddRule_ThenCustomizationPolicyIsNotNull()
            {
                Assert.NotNull(this.property.Policy);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenAddRule_ThenCustomizationPolicyHasRequiredSettings()
            {
                IEnumerable<CustomizableSettingSchema> settings = this.property.Policy.Settings;

                Assert.Equal(10, settings.Count());
                Assert.NotNull(settings.FirstOrDefault(setting => setting.PropertyId == this.property.GetPropertyName(element => element.IsVisible)));
                Assert.NotNull(settings.FirstOrDefault(setting => setting.PropertyId == this.property.GetPropertyName(element => element.Description)));
                Assert.NotNull(settings.FirstOrDefault(setting => setting.PropertyId == this.property.GetPropertyName(element => element.DisplayName)));
                Assert.NotNull(settings.FirstOrDefault(setting => setting.PropertyId == this.property.GetPropertyName(element => element.RawDefaultValue)));
                Assert.NotNull(settings.FirstOrDefault(setting => setting.PropertyId == this.property.GetPropertyName(element => element.IsReadOnly)));
                Assert.NotNull(settings.FirstOrDefault(setting => setting.PropertyId == this.property.GetPropertyName(element => element.Category)));
                Assert.NotNull(settings.FirstOrDefault(setting => setting.PropertyId == this.property.GetPropertyName(element => element.TypeConverterTypeName)));
                Assert.NotNull(settings.FirstOrDefault(setting => setting.PropertyId == this.property.GetPropertyName(element => element.EditorTypeName)));
                Assert.NotNull(settings.FirstOrDefault(setting => setting.PropertyId == this.property.GetPropertyName(element => element.RawValidationRules)));
                Assert.NotNull(settings.FirstOrDefault(setting => setting.PropertyId == this.property.GetPropertyName(element => element.RawValueProvider)));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenPropertyAddedToElement_ThenPropertyUsageIsGeneral()
            {
                PropertySchema newProperty = null;
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    newProperty = this.patternModel.Pattern.Create<PropertySchema>();
                });

                Assert.True(newProperty.PropertyUsage == Runtime.PropertyUsages.General);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenPropertyAddedToExtensionPoint_ThenPropertyUsageIsGeneral()
            {
                PropertySchema newProperty = null;
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    var extensionPoint = this.patternModel.Pattern.Create<ExtensionPointSchema>();
                    newProperty = extensionPoint.Create<PropertySchema>();
                });

                Assert.True(newProperty.PropertyUsage == Runtime.PropertyUsages.General);
            }
        }
    }
}