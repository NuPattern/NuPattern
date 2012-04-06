using Microsoft.VisualStudio.Modeling.Validation;
using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.VisualStudio.Patterning.Runtime.Schema.UnitTests
{
    [TestClass]
    public class PatternSchemaValidationSpec
    {
        internal static readonly IAssertion Assert = new Assertion();
        private static ValidationContext validationContext;

        [TestClass]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public class GivenAPattern
        {
            private PatternSchema pattern;
            private DslTestStore<PatternModelDomainModel> store = new DslTestStore<PatternModelDomainModel>();

            [TestInitialize]
            public virtual void Initialize()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.pattern = this.store.ElementFactory.CreateElement<PatternSchema>();
                });
                validationContext = new ValidationContext(ValidationCategories.Save, this.pattern);
            }

            [TestMethod]
            public void ThenValidateAtLeastOneViewFails()
            {
                this.pattern.ValidateAtLeastOneView(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 1);
                Assert.True(validationContext.ValidationSubjects.IndexOf(this.pattern) == 0);
            }

            [TestMethod]
            public void ThenValidateSingleDefaultViewFails()
            {
                this.pattern.ValidateSingleDefaultView(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 1);
                Assert.True(validationContext.ValidationSubjects.IndexOf(this.pattern) == 0);
            }

            [TestMethod]
            public void ThenValidateAtLeastOneVisibleViewFails()
            {
                this.pattern.ValidateAtLeastOneVisibleView(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 1);
                Assert.True(validationContext.ValidationSubjects.IndexOf(this.pattern) == 0);
            }

            [TestMethod]
            public void ThenValidatePatternNotNamedDefaultSucceeds()
            {
                this.pattern.ValidatePatternNotNamedDefault(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 0);
            }

            [TestMethod]
            public void WhenNameIsDefaultName_ThenValidatePatternNotNamedDefaultFails()
            {
                this.pattern.WithTransaction(pattern => pattern.Name = Properties.Resources.PatternSchema_DefaultName);
                this.pattern.ValidatePatternNotNamedDefault(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 1);
                Assert.True(validationContext.ValidationSubjects.IndexOf(this.pattern) == 0);
            }
        }

        [TestClass]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public class GivenAPatternWithAView
        {
            private PatternSchema pattern;
            private ViewSchema view;
            private DslTestStore<PatternModelDomainModel> store = new DslTestStore<PatternModelDomainModel>();

            [TestInitialize]
            public virtual void Initialize()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    var patternModel = this.store.ElementFactory.CreateElement<PatternModelSchema>();
                    this.pattern = patternModel.Create<PatternSchema>();
                    this.view = this.pattern.Create<ViewSchema>();
                });
                validationContext = new ValidationContext(ValidationCategories.Save, this.pattern);
            }

            [TestMethod]
            public void ThenValidateAtLeastOneViewSucceeds()
            {
                this.pattern.ValidateAtLeastOneView(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 0);
            }

            [TestMethod]
            public void ThenValidateSingleDefaultViewSucceeds()
            {
                this.pattern.ValidateSingleDefaultView(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 0);
            }

            [TestMethod]
            public void ThenValidateAtLeastOneVisibleViewSucceeds()
            {
                this.pattern.ValidateAtLeastOneVisibleView(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 0);
            }

            [TestMethod]
            public void WhenHidden_ThenValidateAtLeastOneVisibleViewFails()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.view.IsVisible = false;
                });

                this.pattern.ValidateAtLeastOneVisibleView(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 1);
                Assert.True(validationContext.ValidationSubjects.IndexOf(this.pattern) == 0);
            }

            [TestMethod]
            public void WhenHiddenAndAnotherViewIsVisible_ThenValidateAtLeastOneVisibleViewSucceeds()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    var view2 = this.pattern.Create<ViewSchema>();
                    view2.IsVisible = true;
                    this.view.IsVisible = false;
                });

                this.pattern.ValidateAtLeastOneVisibleView(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 0);
            }

            [TestMethod]
            public void WhenNotDefaultAndAnotherViewIsDefault_ThenValidateSingleDefaultViewSucceeds()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    var view2 = this.pattern.Create<ViewSchema>();
                    view2.IsDefault = true;
                    this.view.IsDefault = false;
                });

                this.pattern.ValidateSingleDefaultView(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 0);
            }
        }
    }
}
