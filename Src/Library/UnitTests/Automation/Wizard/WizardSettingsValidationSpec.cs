using Microsoft.VisualStudio.Modeling.Extensibility;
using Microsoft.VisualStudio.Modeling.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuPattern.Library.Automation;
using NuPattern.Modeling;
using NuPattern.Runtime.Schema;

namespace NuPattern.Library.UnitTests.Automation.Event
{
    [TestClass]
    public class WizardSettingsValidationSpec
    {
        internal static readonly IAssertion Assert = new Assertion();
        private static ValidationContext validationContext;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "TestCleanup")]
        [TestClass]
        public class GivenWizardSettings
        {
            private DslTestStore<PatternModelDomainModel> store = new DslTestStore<PatternModelDomainModel>(typeof(LibraryDomainModel));
            private WizardSettings settings;

            [TestInitialize]
            public void InitializeContext()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    var patternModel = this.store.ElementFactory.CreateElement<PatternModelSchema>();
                    var pattern = patternModel.Create<PatternSchema>();
                    var automationSettings = pattern.Create<AutomationSettingsSchema>();
                    this.settings = automationSettings.AddExtension<WizardSettings>();
                });
                validationContext = new ValidationContext(ValidationCategories.Save, this.settings);
            }

            [TestCleanup]
            public void Cleanup()
            {
                this.store.Dispose();
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenValidateTypeIdIsNotEmptyFails()
            {
                this.settings.ValidateTypeNameNotEmpty(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 1);
                Assert.True(validationContext.ValidationSubjects.IndexOf(this.settings) == 0);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenTypeIsNotEmpty_ThenValidateTypeIdIsNotEmptySucceeds()
            {
                this.settings.WithTransaction(setting => setting.TypeName = "foo");
                this.settings.ValidateTypeNameNotEmpty(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 0);
            }
        }
    }
}
