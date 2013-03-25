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

namespace NuPattern.Library.UnitTests.Automation.Event
{
    public class EventSettingsValidationSpec
    {
        internal static readonly IAssertion Assert = new Assertion();
        private static ValidationContext validationContext;

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "TestCleanup")]
        public class GivenEventSettings
        {
            private DslTestStore<PatternModelDomainModel> store = new DslTestStore<PatternModelDomainModel>(typeof(LibraryDomainModel));
            private EventSettings settings;
            private EventSettingsValidations validator;

            [TestInitialize]
            public void InitializeContext()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    var patternModel = this.store.ElementFactory.CreateElement<PatternModelSchema>();
                    var pattern = patternModel.Create<PatternSchema>();
                    var automationSettings = pattern.Create<AutomationSettingsSchema>();
                    this.settings = automationSettings.AddExtension<EventSettings>();
                });

                this.validator = new EventSettingsValidations(Mock.Of<IFeatureCompositionService>())
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
            public void ThenValidateEventIdIsNotEmptyFails()
            {
                this.validator.ValidateEventIdIsNotEmpty(validationContext, this.settings);

                Assert.True(validationContext.CurrentViolations.Count == 1);
                Assert.True(validationContext.ValidationSubjects.IndexOf(this.settings) == 0);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenEventIdIsNotEmpty_ThenValidateEventIdIsNotEmptySucceeds()
            {
                this.settings.WithTransaction(setting => setting.EventId = "foo");
                this.validator.ValidateEventIdIsNotEmpty(validationContext, this.settings);

                Assert.True(validationContext.CurrentViolations.Count == 0);
            }
        }
    }
}
