using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.Modeling.Extensibility;
using Microsoft.VisualStudio.Modeling.Validation;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.Library.Automation;
using NuPattern.Modeling;
using NuPattern.Runtime.Extensibility;
using NuPattern.Runtime.Schema;

namespace NuPattern.Library.UnitTests.Automation.Command
{
    [TestClass]
    public class CommandSettingsValidationSpec
    {
        internal static readonly IAssertion Assert = new Assertion();
        private static ValidationContext validationContext;

        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "TestCleanup")]
        [TestClass]
        public class GivenCommandSettings
        {
            private DslTestStore<PatternModelDomainModel> store = new DslTestStore<PatternModelDomainModel>(typeof(LibraryDomainModel));
            private PatternSchema product;
            private CommandSettings settings;
            private CommandSettingsValidations validators;

            [TestInitialize]
            public void InitializeContext()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    var patternModel = this.store.ElementFactory.CreateElement<PatternModelSchema>();
                    this.product = patternModel.Create<PatternSchema>();
                    var automationSettings = product.Create<AutomationSettingsSchema>();
                    this.settings = automationSettings.AddExtension<CommandSettings>();
                });

                validationContext = new ValidationContext(ValidationCategories.Save, this.settings);

                this.validators = new CommandSettingsValidations(Mock.Of<IFeatureCompositionService>());
            }

            [TestCleanup]
            public void Cleanup()
            {
                this.store.Dispose();
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenValidateTypeIsNotEmptyFails()
            {
                this.validators.ValidateTypeIsNotEmpty(validationContext, this.settings);

                Assert.True(validationContext.CurrentViolations.Count == 1);
                Assert.True(validationContext.ValidationSubjects.IndexOf(this.settings) == 0);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenTypeIsNotEmpty_ThenValidateTypeIsNotEmptySucceeds()
            {
                this.settings.WithTransaction(setting => setting.TypeId = "foo");
                this.validators.ValidateTypeIsNotEmpty(validationContext, this.settings);

                Assert.True(validationContext.CurrentViolations.Count == 0);
            }
        }
    }
}
