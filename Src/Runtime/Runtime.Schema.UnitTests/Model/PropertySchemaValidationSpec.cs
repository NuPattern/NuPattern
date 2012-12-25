using System;
using Microsoft.VisualStudio.Modeling.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuPattern.Extensibility;
using NuPattern.Extensibility.Binding;

namespace NuPattern.Runtime.Schema.UnitTests
{
    [TestClass]
    public class PropertySchemaValidationSpec
    {
        private static readonly IAssertion Assert = new Assertion();
        private static ValidationContext validationContext;

        [TestClass]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public class GivenAProperty
        {
            private const string TestStringOnlyAlphas = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqurstuvwxyz1234567890";
            private const string TestStringWithSpaces = TestStringOnlyAlphas + " " + TestStringOnlyAlphas;
            private const string TestStringWithAnAsterix = TestStringOnlyAlphas + "*" + TestStringOnlyAlphas;
            private static readonly string TestStringWith513Chars = new string('A', 513);
            private const string TestStringWithDotSeperatedStrings = TestStringOnlyAlphas + "." + TestStringOnlyAlphas;

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
                validationContext = new ValidationContext(ValidationCategories.Save, this.property);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenTypeIsKnown_ThenValidateTypeIsLegalSucceeds()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.property.Type = typeof(string).FullName;
                });
                this.property.ValidateTypeIsLegal(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 0);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenTypeIsEmpty_ThenValidateTypeIsLegalFails()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.property.Type = string.Empty;
                });
                this.property.ValidateTypeIsLegal(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 1);
                Assert.True(validationContext.ValidationSubjects.IndexOf(this.property) == 0);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenTypeIsUnknown_ThenValidateTypeIsLegalFails()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.property.Type = typeof(IntPtr).FullName;
                });
                this.property.ValidateTypeIsLegal(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 1);
                Assert.True(validationContext.ValidationSubjects.IndexOf(this.property) == 0);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCategoryIsAlphanumeric_ThenValidateCategoryIsLegalSucceeds()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.property.Category = TestStringOnlyAlphas;
                });
                this.property.ValidateCategoryIsLegal(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 0);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCategoryIsEmpty_ThenValidateCategoryIsLegalFails()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.property.Category = string.Empty;
                });
                this.property.ValidateCategoryIsLegal(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 1);
                Assert.True(validationContext.ValidationSubjects.IndexOf(this.property) == 0);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCategoryHasSpaces_ThenValidateCategoryIsLegalSucceeds()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.property.Category = TestStringWithSpaces;
                });
                this.property.ValidateCategoryIsLegal(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 0);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCategoryHasSpecialChars_ThenValidateCategoryIsLegalFails()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.property.Category = TestStringWithAnAsterix;
                });
                this.property.ValidateCategoryIsLegal(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 1);
                Assert.True(validationContext.ValidationSubjects.IndexOf(this.property) == 0);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCategoryHasMoreThan512Chars_ThenValidateCategoryIsLegalFails()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.property.Category = TestStringWith513Chars;
                });
                this.property.ValidateCategoryIsLegal(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 1);
                Assert.True(validationContext.ValidationSubjects.IndexOf(this.property) == 0);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenDefaultValueHasOnlyValue_ThenValidateDefaultValueHasBothValueOrValueProviderSucceeds()
            {
                PropertyBindingSettings propertyBindings = new PropertyBindingSettings();
                propertyBindings.Value = "Foo";

                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.property.DefaultValue = propertyBindings;
                });
                this.property.ValidateDefaultValueHasValueOrValueProvider(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 0);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenDefaultValueHasOnlyValueProvider_ThenValidateDefaultValueHasBothValueOrValueProviderSucceeds()
            {
                var propertyBindings = new PropertyBindingSettings
                {
                    Value = string.Empty,
                    ValueProvider = new ValueProviderBindingSettings
                    {
                        TypeId = "FooType"
                    }
                };

                this.property.DefaultValue = propertyBindings;

                this.property.ValidateDefaultValueHasValueOrValueProvider(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 0);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenDefaultValueHasValueAndValueProvider_ThenValidateDefaultValueHasBothValueOrValueProviderFails()
            {
                PropertyBindingSettings propertyBindings = new PropertyBindingSettings();
                propertyBindings.Value = "Foo";
                propertyBindings.ValueProvider = new ValueProviderBindingSettings
                {
                    TypeId = "FooType"
                };

                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.property.DefaultValue = propertyBindings;
                });
                this.property.ValidateDefaultValueHasValueOrValueProvider(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 1);
                Assert.True(validationContext.ValidationSubjects.IndexOf(this.property) == 0);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenValueProviderAndNoDefaultValue_ThenValidateValueProviderOrDefaultValueSucceeds()
            {
                PropertyBindingSettings defaultValueBindings = new PropertyBindingSettings();
                defaultValueBindings.Value = "";
                defaultValueBindings.ValueProvider = new ValueProviderBindingSettings
                {
                    TypeId = ""
                };
                ValueProviderBindingSettings valueProviderBindings = new ValueProviderBindingSettings
                {
                    TypeId = "Foo"
                };
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.property.DefaultValue = defaultValueBindings;
                    this.property.ValueProvider = valueProviderBindings;
                });
                this.property.ValidateValueProviderOrDefaultValue(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 0);
                Assert.True(validationContext.ValidationSubjects.IndexOf(this.property) == 0);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenNoValueProviderDefaultValue_ThenValidateValueProviderOrDefaultValueSucceeds()
            {
                PropertyBindingSettings defaultValueBindings = new PropertyBindingSettings();
                defaultValueBindings.Value = "Foo";
                defaultValueBindings.ValueProvider = new ValueProviderBindingSettings
                {
                    TypeId = ""
                };
                ValueProviderBindingSettings valueProviderBindings = new ValueProviderBindingSettings
                {
                    TypeId = ""
                };
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.property.DefaultValue = defaultValueBindings;
                    this.property.ValueProvider = valueProviderBindings;
                });
                this.property.ValidateValueProviderOrDefaultValue(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 0);
                Assert.True(validationContext.ValidationSubjects.IndexOf(this.property) == 0);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenValueProviderAndDefaultValue_ThenValidateValueProviderOrDefaultValueFails()
            {
                PropertyBindingSettings defaultValueBindings = new PropertyBindingSettings();
                defaultValueBindings.Value = "Foo1";
                defaultValueBindings.ValueProvider = new ValueProviderBindingSettings
                {
                    TypeId = ""
                };
                ValueProviderBindingSettings valueProviderBindings = new ValueProviderBindingSettings
                {
                    TypeId = "Foo3"
                };
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.property.DefaultValue = defaultValueBindings;
                    this.property.ValueProvider = valueProviderBindings;
                });
                this.property.ValidateValueProviderOrDefaultValue(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 1);
                Assert.True(validationContext.ValidationSubjects.IndexOf(this.property) == 0);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenValueProviderAndDefaultValueProvider_ThenValidateValueProviderOrDefaultValueFails()
            {
                PropertyBindingSettings defaultValueBindings = new PropertyBindingSettings();
                defaultValueBindings.Value = "";
                defaultValueBindings.ValueProvider = new ValueProviderBindingSettings
                {
                    TypeId = "Foo"
                };
                ValueProviderBindingSettings valueProviderBindings = new ValueProviderBindingSettings
                {
                    TypeId = "Foo3"
                };
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.property.DefaultValue = defaultValueBindings;
                    this.property.ValueProvider = valueProviderBindings;
                });
                this.property.ValidateValueProviderOrDefaultValue(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 1);
                Assert.True(validationContext.ValidationSubjects.IndexOf(this.property) == 0);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenDisplayNameIsReserved_ThenValidateDisplayNameIsNotReservedFails()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.property.DisplayName = "Name";
                });
                this.property.ValidateDisplayNameIsNotReserved(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 1);
                Assert.True(validationContext.ValidationSubjects.IndexOf(this.property) == 0);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenDisplayNameIsNotReserved_ThenValidateDisplayNameIsNotReservedSucceeds()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.property.DisplayName = "Foo";
                });
                this.property.ValidateDisplayNameIsNotReserved(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 0);
            }
        }

        [TestClass]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public class GivenAnElementOwnedProperty
        {
            private ElementSchema elementOwner;
            private PropertySchema element;
            private DslTestStore<PatternModelDomainModel> store = new DslTestStore<PatternModelDomainModel>();

            [TestInitialize]
            public virtual void Initialize()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.elementOwner = this.store.ElementFactory.CreateElement<ElementSchema>();
                    this.element = this.store.ElementFactory.CreateElement<PropertySchema>();
                    this.elementOwner.Properties.Add(this.element);
                });
                validationContext = new ValidationContext(ValidationCategories.Save, this.element);
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenValidateNameIsUniqueSucceeds()
            {
                this.element.ValidateNameIsUnique(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 0);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenSameNamedElementAddedToDifferentOwner_ThenValidateNameIsUniqueSucceeds()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    ElementSchema elementOwner2 = this.store.ElementFactory.CreateElement<ElementSchema>();
                    PropertySchema element2 = this.store.ElementFactory.CreateElement<PropertySchema>();
                    element2.Name = this.element.Name;
                    elementOwner2.Properties.Add(element2);
                });
                this.element.ValidateNameIsUnique(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 0);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenSameNamedElementAddedToSameOwner_ThenValidateNameIsUniqueFails()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    PropertySchema element2 = this.store.ElementFactory.CreateElement<PropertySchema>();
                    element2.Name = this.element.Name;
                    this.elementOwner.Properties.Add(element2);
                });
                this.element.ValidateNameIsUnique(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 1);
                Assert.True(validationContext.ValidationSubjects.IndexOf(this.element) == 0);
                Assert.True(validationContext.CurrentViolations[0].Code == Properties.Resources.Validate_PropertyNameIsNotUniqueCode);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenSameNamedSystemElementAddedToSameOwner_ThenValidateNameIsUniqueFails()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    PropertySchema element2 = this.store.ElementFactory.CreateElement<PropertySchema>();
                    element2.Name = this.element.Name;
                    element2.IsSystem = true;
                    this.elementOwner.Properties.Add(element2);
                });
                this.element.ValidateNameIsUnique(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 1);
                Assert.True(validationContext.ValidationSubjects.IndexOf(this.element) == 0);
                Assert.True(validationContext.CurrentViolations[0].Code == Properties.Resources.Validate_PropertyNameSameAsSystemCode);
            }
        }
    }
}