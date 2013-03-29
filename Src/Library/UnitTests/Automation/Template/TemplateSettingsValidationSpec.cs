using System;
using Microsoft.VisualStudio.Modeling.Extensibility;
using Microsoft.VisualStudio.Modeling.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuPattern.Library.Automation;
using NuPattern.Library.Automation.Template;
using NuPattern.Library.Commands;
using NuPattern.Modeling;
using NuPattern.Runtime;
using NuPattern.Runtime.Schema;

namespace NuPattern.Library.UnitTests
{
    [TestClass]
    public class TemplateSettingsValidationSpec
    {
        internal static readonly IAssertion Assert = new Assertion();
        private static ValidationContext validationContext;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "TestCleanup")]
        [TestClass]
        public class GivenAProductWithTemplateSettings
        {
            private DslTestStore<PatternModelDomainModel> store = new DslTestStore<PatternModelDomainModel>(typeof(LibraryDomainModel));
            private TemplateSettings settings;
            private PatternSchema product;

            [TestInitialize]
            public void InitializeContext()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    var patternModel = this.store.ElementFactory.CreateElement<PatternModelSchema>();
                    this.product = patternModel.Create<PatternSchema>();
                    var automationSettings = this.product.Create<AutomationSettingsSchema>();
                    this.settings = automationSettings.AddExtension<TemplateSettings>();
                });
                validationContext = new ValidationContext(ValidationCategories.Save, this.settings);
            }

            [TestCleanup]
            public void Cleanup()
            {
                this.store.Dispose();
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenValidateTemplateUriIsNotEmptyFails()
            {
                var templateValidator = new TemplateValidator(this.settings.Name,
                    new UnfoldVsTemplateCommand.UnfoldVsTemplateSettings
                    {
                        TemplateUri = string.Empty,
                        OwnerElement = this.product,
                        SettingsElement = (IAutomationSettingsSchema)this.settings.Extends,
                    }, validationContext, this.settings.Store);
                templateValidator.ValidateTemplateUriIsNotEmpty();

                Assert.True(validationContext.CurrentViolations.Count == 1);
                Assert.True(validationContext.ValidationSubjects.IndexOf(this.settings) == 0);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenTemplateUriIsNotEmpty_ThenValidateTemplateUriIsNotEmptySucceeds()
            {
                var templateValidator = new TemplateValidator(this.settings.Name,
                    new UnfoldVsTemplateCommand.UnfoldVsTemplateSettings
                    {
                        TemplateUri = "foo",
                        OwnerElement = this.product
                    }, validationContext, this.settings.Store);
                templateValidator.ValidateTemplateUriIsNotEmpty();

                Assert.True(validationContext.CurrentViolations.Count == 0);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenWizardIdIsInvalid_ThenValidateWizardIdDoesNotSucceed()
            {
                this.settings.WithTransaction(setting => setting.WizardId = Guid.NewGuid());
                this.settings.ValidateWizardIdIsValid(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 1);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCommandIdIsInvalid_ThenValidateCommandIdDoesNotSucceed()
            {
                this.settings.WithTransaction(setting => setting.CommandId = Guid.NewGuid());
                this.settings.ValidateCommandIdIsValid(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 1);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenNoInstantiationAndNoAutomationOnProduct_ThenInstantiationOccursSucceeds()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                    {
                        this.settings.CreateElementOnUnfold = false;
                        this.settings.UnfoldOnElementCreated = false;
                        this.settings.CommandId = Guid.Empty;
                        this.settings.WizardId = Guid.Empty;
                    });
                this.settings.ValidateNoInstantiationOnProductOnly(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 0);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenNoInstantiationAndCommand_ThenValidateNoInstantiationWithAutomationFails()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.settings.CreateElementOnUnfold = false;
                    this.settings.UnfoldOnElementCreated = false;
                    this.settings.CommandId = Guid.NewGuid();
                    this.settings.WizardId = Guid.Empty;
                });
                this.settings.ValidateNoAutomationWithNoInstantiation(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 1);
                Assert.True(validationContext.ValidationSubjects.IndexOf(this.settings) == 0);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenNoInstantiationAndWizard_ThenValidateNoInstantiationWithAutomationFails()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.settings.CreateElementOnUnfold = false;
                    this.settings.UnfoldOnElementCreated = false;
                    this.settings.CommandId = Guid.Empty;
                    this.settings.WizardId = Guid.NewGuid();
                });
                this.settings.ValidateNoAutomationWithNoInstantiation(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 1);
                Assert.True(validationContext.ValidationSubjects.IndexOf(this.settings) == 0);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenOwnerIsProduct_ValidateOwnerIsProductSucceeds()
            {
                this.settings.ValidateOwnerIsPattern(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 0);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "TestCleanup")]
        [TestClass]
        public class GivenAnElementWithTemplateSettings
        {
            private DslTestStore<PatternModelDomainModel> store = new DslTestStore<PatternModelDomainModel>(typeof(LibraryDomainModel));
            private TemplateSettings settings;

            [TestInitialize]
            public void InitializeContext()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    var patternModel = this.store.ElementFactory.CreateElement<PatternModelSchema>();
                    var pattern = patternModel.Create<PatternSchema>();
                    var view = pattern.Create<ViewSchema>();
                    var element = view.Create<ElementSchema>();
                    var automationSettings = element.Create<AutomationSettingsSchema>();
                    this.settings = automationSettings.AddExtension<TemplateSettings>();
                });
                validationContext = new ValidationContext(ValidationCategories.Save, this.settings);
            }

            [TestCleanup]
            public void Cleanup()
            {
                this.store.Dispose();
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenNoInstantiationAndNoAutomationOnElement_ThenInstantiationOccursFails()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.settings.CreateElementOnUnfold = false;
                    this.settings.UnfoldOnElementCreated = false;
                    this.settings.CommandId = Guid.Empty;
                    this.settings.WizardId = Guid.Empty;
                });
                this.settings.ValidateNoInstantiationOnProductOnly(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 1);
                Assert.True(validationContext.ValidationSubjects.IndexOf(this.settings) == 0);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenOwnerIsElement_ValidateOwnerIsProductFails()
            {
                this.settings.ValidateOwnerIsPattern(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 1);
            }

        }
    }
}
