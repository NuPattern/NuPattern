using Microsoft.VisualStudio.Modeling.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuPattern.Extensibility;

namespace NuPattern.Runtime.Schema.UnitTests
{
    [TestClass]
    public class ViewSchemaValidationSpec
    {
        internal static readonly IAssertion Assert = new Assertion();
        private static ValidationContext validationContext;

        [TestClass]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public class GivenAnOwnedView
        {
            private PatternSchema elementOwner;
            private ViewSchema view;
            private DslTestStore<PatternModelDomainModel> store = new DslTestStore<PatternModelDomainModel>();

            [TestInitialize]
            public virtual void Initialize()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.elementOwner = this.store.ElementFactory.CreateElement<PatternSchema>();
                    this.view = this.store.ElementFactory.CreateElement<ViewSchema>();
                    this.elementOwner.Views.Add(this.view);
                });
                validationContext = new ValidationContext(ValidationCategories.Save, this.view);
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenValidateNameIsUniqueSucceeds()
            {
                this.view.ValidateNameIsUnique(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 0);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenSameNamedElementAddedToDifferentOwner_ThenValidateNameIsUniqueSucceeds()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    PatternSchema elementOwner2 = this.store.ElementFactory.CreateElement<PatternSchema>();
                    ViewSchema view2 = this.store.ElementFactory.CreateElement<ViewSchema>();
                    view2.Name = this.view.Name;
                    elementOwner2.Views.Add(view2);
                });
                this.view.ValidateNameIsUnique(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 0);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenSameNamedElementAddedToSameOwner_ThenValidateNameIsUniqueFails()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    ViewSchema view2 = this.store.ElementFactory.CreateElement<ViewSchema>();
                    view2.Name = this.view.Name;
                    this.elementOwner.Views.Add(view2);
                });
                this.view.ValidateNameIsUnique(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 1);
                Assert.True(validationContext.ValidationSubjects.IndexOf(this.view) == 0);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenHiddenAndNotDefault_ThenValidateDefaultIsHiddenSucceeds()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    ViewSchema view2 = this.store.ElementFactory.CreateElement<ViewSchema>();
                    this.elementOwner.Views.Add(view2);
                    view2.IsDefault = true;

                    this.view.IsVisible = false;
                });
                this.view.ValidateDefaultIsHidden(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 0);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenHiddenAndDefault_ThenValidateDefaultIsHiddenFails()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.view.IsVisible = false;
                    this.view.IsDefault = true;
                });
                this.view.ValidateDefaultIsHidden(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 1);
                Assert.True(validationContext.ValidationSubjects.IndexOf(this.view) == 0);
            }
        }
    }
}
