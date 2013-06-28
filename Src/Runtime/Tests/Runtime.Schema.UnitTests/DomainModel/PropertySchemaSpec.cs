using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuPattern.Modeling;
using NuPattern.Runtime.Bindings;

namespace NuPattern.Runtime.Schema.UnitTests
{
    [TestClass]
    public class PropertySchemaSpec
    {
        private static readonly IAssertion Assert = new Assertion();

        [TestClass]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public class GivenAProperty
        {
            private PropertySchema property = null;
            private DslTestStore<PatternModelDomainModel> store = new DslTestStore<PatternModelDomainModel>();

            [TestInitialize]
            public void Initialize()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    var patternModel = this.store.ElementFactory.CreateElement<PatternModelSchema>();
                    var pattern = patternModel.Create<PatternSchema>();
                    this.property = pattern.Create<PropertySchema>();
                });
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenPropertyCreated_ThenPropertyUsageIsGeneral()
            {
                Assert.True(this.property.PropertyUsage == Runtime.PropertyUsages.General);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenChangingDefaultValueSettings_ThenPersistsChangesAutomatically()
            {
                var settings = (PropertyBindingSettings)this.property.DefaultValue;

                settings.Value = "hello";

                var saved = BindingSerializer.Deserialize<PropertyBindingSettings>(this.property.RawDefaultValue);

                Assert.Equal("hello", saved.Value);
            }


            [TestMethod, TestCategory("Unit")]
            public void WhenChangingValueProviderSettings_ThenPersistsChangesAutomatically()
            {
                var settings = this.property.ValueProvider;

                settings.TypeId = "Foo";

                var saved = BindingSerializer.Deserialize<IValueProviderBindingSettings>(this.property.RawValueProvider);

                Assert.Equal("Foo", saved.TypeId);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenSettingValidatorBindingSettings_ThenPersistsChanges()
            {
                var settings = new ValidationBindingSettings
                {
                    TypeId = "Foo",
                    Properties = 
                    {
                        new PropertyBindingSettings
                        {
                            Name = "Message", 
                            Value = "Hello",
                        },
                        new PropertyBindingSettings
                        {
                            Name = "From", 
                            ValueProvider = new ValueProviderBindingSettings
                            {
                                TypeId = "CurrentUserProvider", 
                            }
                        },
                    },
                };

                this.property.ValidationSettings = new IBindingSettings[] { settings };

                var saved = BindingSerializer.Deserialize<ValidationBindingSettings[]>(((PropertySchema)this.property).RawValidationRules);

                Assert.Equal(1, saved.Length);
                Assert.Equal("Foo", saved[0].TypeId);
                Assert.Equal(2, saved[0].Properties.Count);
                Assert.Equal("Message", saved[0].Properties[0].Name);
                Assert.Equal("Hello", saved[0].Properties[0].Value);
                Assert.Equal("From", saved[0].Properties[1].Name);
                Assert.Equal("CurrentUserProvider", saved[0].Properties[1].ValueProvider.TypeId);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenChangingSavedValidatorBindingSettings_ThenPersistsChanges()
            {
                var settings = new ValidationBindingSettings
                {
                    TypeId = "Foo",
                    Properties = 
                    {
                        new PropertyBindingSettings
                        {
                            Name = "Message", 
                            Value = "Hello",
                        },
                        new PropertyBindingSettings
                        {
                            Name = "From", 
                            ValueProvider = new ValueProviderBindingSettings
                            {
                                TypeId = "CurrentUserProvider", 
                            }
                        },
                    },
                };

                ((PropertySchema)this.property).RawValidationRules = BindingSerializer.Serialize<ValidationBindingSettings[]>(new ValidationBindingSettings[] { settings });

                ((BindingSettings)this.property.ValidationSettings.First()).Properties[0].Value = "World";
                ((BindingSettings)this.property.ValidationSettings.First()).Properties[1].ValueProvider.TypeId = "AnotherProvider";

                var saved = BindingSerializer.Deserialize<ValidationBindingSettings[]>(((PropertySchema)this.property).RawValidationRules);

                Assert.Equal(1, saved.Length);
                Assert.Equal("World", saved[0].Properties[0].Value);
                Assert.Equal("AnotherProvider", saved[0].Properties[1].ValueProvider.TypeId);
            }
        }
    }
}