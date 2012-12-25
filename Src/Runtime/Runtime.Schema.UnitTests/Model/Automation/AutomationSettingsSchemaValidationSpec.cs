using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.Modeling.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuPattern.Extensibility;

namespace NuPattern.Runtime.Schema.UnitTests
{
    [TestClass]
    public class AutomationSettingsSchemaValidationSpec
    {
        internal static readonly IAssertion Assert = new Assertion();
        private static ValidationContext validationContext;

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public class GivenAnOwnedAutomation
        {
            private ElementSchema elementOwner;
            private AutomationSettingsSchema element;
            private DslTestStore<PatternModelDomainModel> store = new DslTestStore<PatternModelDomainModel>();

            [TestInitialize]
            public virtual void Initialize()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.elementOwner = this.store.ElementFactory.CreateElement<ElementSchema>();
                    this.element = this.store.ElementFactory.CreateElement<AutomationSettingsSchema>();
                    this.elementOwner.AutomationSettings.Add(this.element);
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
                    AutomationSettingsSchema element2 = this.store.ElementFactory.CreateElement<AutomationSettingsSchema>();
                    element2.Name = this.element.Name;
                    elementOwner2.AutomationSettings.Add(element2);
                });
                this.element.ValidateNameIsUnique(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 0);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenSameNamedElementAddedToSameOwner_ThenValidateNameIsUniqueFails()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    AutomationSettingsSchema element2 = this.store.ElementFactory.CreateElement<AutomationSettingsSchema>();
                    element2.Name = this.element.Name;
                    this.elementOwner.AutomationSettings.Add(element2);
                });
                this.element.ValidateNameIsUnique(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 1);
                Assert.True(validationContext.ValidationSubjects.IndexOf(this.element) == 0);
                Assert.True(validationContext.CurrentViolations[0].Code == Properties.Resources.Validate_AutomationSettingsNameIsNotUniqueCode);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenSameNamedSystemElementAddedToSameOwner_ThenValidateNameIsUniqueFails()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    AutomationSettingsSchema element2 = this.store.ElementFactory.CreateElement<AutomationSettingsSchema>();
                    element2.Name = this.element.Name;
                    element2.IsSystem = true;
                    this.elementOwner.AutomationSettings.Add(element2);
                });
                this.element.ValidateNameIsUnique(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 1);
                Assert.True(validationContext.ValidationSubjects.IndexOf(this.element) == 0);
                Assert.True(validationContext.CurrentViolations[0].Code == Properties.Resources.Validate_AutomationSettingsNameSameAsSystemCode);
            }
        }
    }
}
