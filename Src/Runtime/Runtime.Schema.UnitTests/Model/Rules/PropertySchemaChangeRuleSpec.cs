using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuPattern.Extensibility;
using NuPattern.Extensibility.Binding;

namespace NuPattern.Runtime.Schema.UnitTests
{
    [TestClass]
    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
    public class PropertySchemaChangeRuleSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "TestCode"), TestClass]
        public class GivenAPropertyWithAnyType
        {
            private DslTestStore<PatternModelDomainModel> store = new DslTestStore<PatternModelDomainModel>();
            private PropertySchema propertySchema;

            [TestInitialize]
            public void InitializeContext()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    var patternModel = this.store.ElementFactory.CreateElement<PatternModelSchema>();
                    this.propertySchema = (PropertySchema)patternModel.CreatePatternSchema().CreatePropertySchema();
                    this.propertySchema.Type = typeof(string).FullName;
                    this.propertySchema.DefaultValue.Value = "Foo";
                    this.propertySchema.DefaultValue.ValueProvider = new ValueProviderBindingSettings { TypeId = "Bar" };
                });
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenSettingTypeToInt_ThenResetsDefaultValue()
            {
                this.propertySchema.Type = typeof(int).FullName;

                Assert.Equal(this.propertySchema.DefaultValue.Value, string.Empty);
                Assert.Equal(this.propertySchema.DefaultValue.ValueProvider, null);
            }
        }
    }
}