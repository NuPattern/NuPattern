using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.Modeling.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuPattern.Modeling;
using NuPattern.Runtime.Extensibility;

namespace NuPattern.Runtime.Schema.UnitTests
{
    public partial class CustomizableElementSchemaSpec
    {
        private static ValidationContext validationContext;

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public class GivenADisabledCustomizableElement
        {
            private CustomizableElementSchema element = null;
            private DslTestStore<PatternModelDomainModel> store = new DslTestStore<PatternModelDomainModel>();

            [TestInitialize]
            public void Initialize()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.element = this.store.ElementFactory.CreateElement<ElementSchema>();
                });
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.element.DisableCustomization();
                });
                validationContext = new ValidationContext(ValidationCategories.Save, this.element);
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenValidateCustomizedSettingsOverriddenByCustomizableStateSucceeds()
            {
                this.element.ValidateCustomizedSettingsOverriddenByCustomizableState(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 0);
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenValidateCustomizedStateWithNoCustomizableSettingsSucceeds()
            {
                this.element.ValidateCustomizedStateWithNoCustomizableSettings(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 0);
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenValidateCustomizableStateRedundantSucceeds()
            {
                this.element.ValidateCustomizableStateRedundant(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 0);
            }
        }

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public class GivenACustomizableElementWithNoParentAndASetting
        {
            private CustomizableElementSchema element = null;
            private CustomizableSettingSchema setting = null;
            private DslTestStore<PatternModelDomainModel> store = new DslTestStore<PatternModelDomainModel>();

            [TestInitialize]
            public void Initialize()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.element = this.store.ElementFactory.CreateElement<ElementSchema>();
                });
                this.setting = this.element.Policy.Create<CustomizableSettingSchema>();
                validationContext = new ValidationContext(ValidationCategories.Save, this.element);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenIsCustomizableIsFalseAndSettingIsModified_ThenValidateCustomizedSettingsOverriddenByCustomizableStateFails()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.element.IsCustomizable = CustomizationState.False;
                    this.setting.Value = !this.setting.DefaultValue;
                });
                this.element.ValidateCustomizedSettingsOverriddenByCustomizableState(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 1);
                Assert.True(validationContext.ValidationSubjects.IndexOf(this.element) == 0);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenIsCustomizableIsTrueAndNoSettingsModified_ThenValidateCustomizedStateWithNoCustomizableSettingsFails()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.element.IsCustomizable = CustomizationState.True;
                    this.element.Policy.Settings.ForEach(setting => setting.Value = false);
                });
                this.element.ValidateCustomizedStateWithNoCustomizableSettings(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 1);
                Assert.True(validationContext.ValidationSubjects.IndexOf(this.element) == 0);
            }
        }

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public class GivenACustomizableElementWithParentWhereParentIsPolicyIsTrue
        {
            private ElementSchema childElement = null;
            private ElementSchema parentElement;
            private DslTestStore<PatternModelDomainModel> store = new DslTestStore<PatternModelDomainModel>();

            [TestInitialize]
            public void Initialize()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.parentElement = this.store.ElementFactory.CreateElement<ElementSchema>();
                });
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.parentElement.IsCustomizable = CustomizationState.True;
                });
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.childElement = this.store.ElementFactory.CreateElement<ElementSchema>();
                });
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.parentElement.Elements.Add(this.childElement);
                });
                validationContext = new ValidationContext(ValidationCategories.Save, this.childElement);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenIsCustomizableIsTrueAndParentIsCustomizable_ThenValidateCustomizableStateRedundantFails()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.childElement.IsCustomizable = CustomizationState.True;
                    this.parentElement.IsCustomizable = CustomizationState.True;
                });
                this.childElement.ValidateCustomizableStateRedundant(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 1);
                Assert.True(validationContext.ValidationSubjects.IndexOf(this.childElement) == 0);
            }
        }
    }
}