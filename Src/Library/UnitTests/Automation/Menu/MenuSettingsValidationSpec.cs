using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.Modeling.Extensibility;
using Microsoft.VisualStudio.Modeling.Validation;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.Library.Automation;
using NuPattern.Modeling;
using NuPattern.Runtime;
using NuPattern.Runtime.Extensibility;
using NuPattern.Runtime.Schema;

namespace NuPattern.Library.UnitTests.Automation.Menu
{
    [TestClass]
    public class MenuSettingsValidationSpec
    {
        internal static readonly IAssertion Assert = new Assertion();
        private static ValidationContext validationContext;

        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "TestCleanup")]
        [TestClass]
        public class GivenMenuSettings
        {
            private DslTestStore<PatternModelDomainModel> store = new DslTestStore<PatternModelDomainModel>(typeof(LibraryDomainModel));
            private MenuSettings settings;
            private MenuSettingsValidations validator;

            [TestInitialize]
            public void InitializeContext()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    var patternModel = this.store.ElementFactory.CreateElement<PatternModelSchema>();
                    var pattern = patternModel.Create<PatternSchema>();
                    var automationSettings = pattern.Create<AutomationSettingsSchema>();
                    this.settings = automationSettings.AddExtension<MenuSettings>();
                });

                this.validator = new MenuSettingsValidations(Mock.Of<IFeatureCompositionService>())
                {
                    ProjectTypeProvider = Mock.Of<INuPatternProjectTypeProvider>(),
                };

                validationContext = new ValidationContext(ValidationCategories.Save, this.settings);
            }

            [TestCleanup]
            public void Cleanup()
            {
                this.store.Dispose();
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenValidateCommandIdAndWizardIdIsNotEmptyFails()
            {
                this.validator.ValidateCommandIdAndWizardIdIsNotEmpty(validationContext, this.settings);

                Assert.True(validationContext.CurrentViolations.Count == 1);
                Assert.True(validationContext.ValidationSubjects.IndexOf(this.settings) == 0);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCommandIdIsNotEmpty_ThenValidateCommandIdAndWizardIdIsNotEmptySucceeds()
            {
                this.settings.WithTransaction(setting => setting.CommandId = Guid.NewGuid());
                this.validator.ValidateCommandIdAndWizardIdIsNotEmpty(validationContext, this.settings);

                Assert.True(validationContext.CurrentViolations.Count == 0);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenWizardIdIsNotEmpty_ThenValidateCommandIdAndWizardIdIsNotEmptySucceeds()
            {
                this.settings.WithTransaction(setting => setting.WizardId = Guid.NewGuid());
                this.validator.ValidateCommandIdAndWizardIdIsNotEmpty(validationContext, this.settings);

                Assert.True(validationContext.CurrentViolations.Count == 0);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenWizardIdIsInvalid_ThenValidateWizardIdDoesNotSucceed()
            {
                this.settings.WithTransaction(setting => setting.WizardId = Guid.NewGuid());
                this.validator.ValidateWizardIdIsValid(validationContext, this.settings);

                Assert.True(validationContext.CurrentViolations.Count == 1);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCommandIdIsInvalid_ThenValidateCommandIdDoesNotSucceed()
            {
                this.settings.WithTransaction(setting => setting.CommandId = Guid.NewGuid());
                this.validator.ValidateCommandIdIsValid(validationContext, this.settings);

                Assert.True(validationContext.CurrentViolations.Count == 1);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCommandIdAndWizardIdIsNotEmpty_ThenValidateCommandIdAndWizardIdIsNotEmptySucceeds()
            {
                this.settings.WithTransaction(setting => setting.CommandId = Guid.NewGuid());
                this.settings.WithTransaction(setting => setting.WizardId = Guid.NewGuid());
                this.validator.ValidateCommandIdAndWizardIdIsNotEmpty(validationContext, this.settings);

                Assert.True(validationContext.CurrentViolations.Count == 0);
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenValidateMenuTextIsNotEmptyFails()
            {
                this.validator.ValidateTextIsNotEmpty(validationContext, this.settings);

                Assert.True(validationContext.CurrentViolations.Count == 1);
                Assert.True(validationContext.ValidationSubjects.IndexOf(this.settings) == 0);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenMenuIdIsNotEmpty_ThenValidateMenuIdIsNotEmptySucceeds()
            {
                this.settings.WithTransaction(setting => setting.Text = "foo");
                this.validator.ValidateTextIsNotEmpty(validationContext, this.settings);

                Assert.True(validationContext.CurrentViolations.Count == 0);
            }
        }
    }
}
