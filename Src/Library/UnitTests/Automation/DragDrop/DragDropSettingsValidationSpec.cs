using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.Modeling.Extensibility;
using Microsoft.VisualStudio.Modeling.Validation;
using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.Patterning.Library.Automation;
using Microsoft.VisualStudio.Patterning.Runtime.Schema;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Microsoft.VisualStudio.Patterning.Library.UnitTests.Automation.Event
{
    public class DragDropSettingsValidationSpec
    {
        internal static readonly IAssertion Assert = new Assertion();
        private static ValidationContext validationContext;

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "TestCleanup")]
        public class GivenDragDropSettings
        {
            private DslTestStore<PatternModelDomainModel> store = new DslTestStore<PatternModelDomainModel>(typeof(LibraryDomainModel));
            private DragDropSettings settings;
            private DragDropSettingsValidations validator;

            [TestInitialize]
            public void InitializeContext()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    var patternModel = this.store.ElementFactory.CreateElement<PatternModelSchema>();
                    var pattern = patternModel.Create<PatternSchema>();
                    var automationSettings = pattern.Create<AutomationSettingsSchema>();
                    this.settings = automationSettings.AddExtension<DragDropSettings>();
                });

                this.validator = new DragDropSettingsValidations(Mock.Of<IFeatureCompositionService>())
                {
                    ProjectTypeProvider = Mock.Of<IPlatuProjectTypeProvider>(),
                };

                validationContext = new ValidationContext(ValidationCategories.Save, this.settings);
            }

            [TestCleanup]
            public void Cleanup()
            {
                this.store.Dispose();
            }

            [TestMethod]
            public void ThenValidateCommandIdAndWizardIdIsNotEmptyFails()
            {
                this.validator.ValidateCommandIdAndWizardIdIsNotEmpty(validationContext, this.settings);

                Assert.True(validationContext.CurrentViolations.Count == 1);
                Assert.True(validationContext.ValidationSubjects.IndexOf(this.settings) == 0);
            }

            [TestMethod]
            public void WhenCommandIdIsNotEmpty_ThenValidateCommandIdAndWizardIdIsNotEmptySucceeds()
            {
                this.settings.WithTransaction(setting => setting.CommandId = Guid.NewGuid());
                this.validator.ValidateCommandIdAndWizardIdIsNotEmpty(validationContext, this.settings);

                Assert.True(validationContext.CurrentViolations.Count == 0);
            }

            [TestMethod]
            public void WhenWizardIdIsNotEmpty_ThenValidateCommandIdAndWizardIdIsNotEmptySucceeds()
            {
                this.settings.WithTransaction(setting => setting.WizardId = Guid.NewGuid());
                this.validator.ValidateCommandIdAndWizardIdIsNotEmpty(validationContext, this.settings);

                Assert.True(validationContext.CurrentViolations.Count == 0);
            }

            [TestMethod]
            public void WhenWizardIdIsInvalid_ThenValidateWizardIdDoesNotSucceed()
            {
                this.settings.WithTransaction(setting => setting.WizardId = Guid.NewGuid());
                this.validator.ValidateWizardIdIsValid(validationContext, this.settings);

                Assert.True(validationContext.CurrentViolations.Count == 1);
            }

            [TestMethod]
            public void WhenCommandIdIsInvalid_ThenValidateCommandIdDoesNotSucceed()
            {
                this.settings.WithTransaction(setting => setting.CommandId = Guid.NewGuid());
                this.validator.ValidateCommandIdIsValid(validationContext, this.settings);

                Assert.True(validationContext.CurrentViolations.Count == 1);
            }

            [TestMethod]
            public void WhenCommandIdAndWizardIdIsNotEmpty_ThenValidateCommandIdAndWizardIdIsNotEmptySucceeds()
            {
                this.settings.WithTransaction(setting => setting.CommandId = Guid.NewGuid());
                this.settings.WithTransaction(setting => setting.WizardId = Guid.NewGuid());
                this.validator.ValidateCommandIdAndWizardIdIsNotEmpty(validationContext, this.settings);

                Assert.True(validationContext.CurrentViolations.Count == 0);
            }
        }
    }
}
