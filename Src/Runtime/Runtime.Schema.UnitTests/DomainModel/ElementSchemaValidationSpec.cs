using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.Modeling.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuPattern.Modeling;

namespace NuPattern.Runtime.Schema.UnitTests
{
    public class ElementSchemaValidationSpec
    {
        internal static readonly IAssertion Assert = new Assertion();
        private static ValidationContext validationContext;

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public class GivenAViewOwnedElement
        {
            private ViewSchema elementOwner;
            private ElementSchema element;
            private DslTestStore<PatternModelDomainModel> store = new DslTestStore<PatternModelDomainModel>();

            [TestInitialize]
            public virtual void Initialize()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.elementOwner = this.store.ElementFactory.CreateElement<ViewSchema>();
                    this.element = this.store.ElementFactory.CreateElement<ElementSchema>();
                    this.elementOwner.Elements.Add(this.element);
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
                    ViewSchema elementOwner2 = this.store.ElementFactory.CreateElement<ViewSchema>();
                    ElementSchema element2 = this.store.ElementFactory.CreateElement<ElementSchema>();
                    element2.Name = this.element.Name;
                    elementOwner2.Elements.Add(element2);
                });
                this.element.ValidateNameIsUnique(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 0);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenSameNamedElementAddedToSameOwner_ThenValidateNameIsUniqueFails()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    ElementSchema element2 = this.store.ElementFactory.CreateElement<ElementSchema>();
                    element2.Name = this.element.Name;
                    this.elementOwner.Elements.Add(element2);
                });
                this.element.ValidateNameIsUnique(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 1);
                Assert.True(validationContext.ValidationSubjects.IndexOf(this.element) == 0);
            }
        }

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public class GivenAElementOwnedElement
        {
            private ElementSchema elementOwner;
            private ElementSchema element;
            private DslTestStore<PatternModelDomainModel> store = new DslTestStore<PatternModelDomainModel>();

            [TestInitialize]
            public virtual void Initialize()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.elementOwner = this.store.ElementFactory.CreateElement<ElementSchema>();
                    this.element = this.store.ElementFactory.CreateElement<ElementSchema>();
                    this.elementOwner.Elements.Add(this.element);
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
                    ElementSchema element2 = this.store.ElementFactory.CreateElement<ElementSchema>();
                    element2.Name = this.element.Name;
                    elementOwner2.Elements.Add(element2);
                });
                this.element.ValidateNameIsUnique(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 0);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenSameNamedElementAddedToSameOwner_ThenValidateNameIsUniqueFails()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    ElementSchema element2 = this.store.ElementFactory.CreateElement<ElementSchema>();
                    element2.Name = this.element.Name;
                    this.elementOwner.Elements.Add(element2);
                });
                this.element.ValidateNameIsUnique(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 1);
                Assert.True(validationContext.ValidationSubjects.IndexOf(this.element) == 0);
            }
        }

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public class GivenACollectionOwnedElement
        {
            private CollectionSchema elementOwner;
            private ElementSchema element;
            private DslTestStore<PatternModelDomainModel> store = new DslTestStore<PatternModelDomainModel>();

            [TestInitialize]
            public virtual void Initialize()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.elementOwner = this.store.ElementFactory.CreateElement<CollectionSchema>();
                    this.element = this.store.ElementFactory.CreateElement<ElementSchema>();
                    this.elementOwner.Elements.Add(this.element);
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
                    CollectionSchema elementOwner2 = this.store.ElementFactory.CreateElement<CollectionSchema>();
                    ElementSchema element2 = this.store.ElementFactory.CreateElement<ElementSchema>();
                    element2.Name = this.element.Name;
                    elementOwner2.Elements.Add(element2);
                });
                this.element.ValidateNameIsUnique(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 0);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenSameNamedElementAddedToSameOwner_ThenValidateNameIsUniqueFails()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    ElementSchema element2 = this.store.ElementFactory.CreateElement<ElementSchema>();
                    element2.Name = this.element.Name;
                    this.elementOwner.Elements.Add(element2);
                });
                this.element.ValidateNameIsUnique(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 1);
                Assert.True(validationContext.ValidationSubjects.IndexOf(this.element) == 0);
            }
        }
    }
}
