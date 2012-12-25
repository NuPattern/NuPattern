using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.Modeling.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuPattern.Extensibility;

namespace NuPattern.Runtime.Schema.UnitTests
{
    public class CollectionSchemaValidationSpec
    {
        internal static readonly IAssertion Assert = new Assertion();
        private static ValidationContext validationContext;

        [TestClass]
        [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "Test code")]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public class GivenAViewOwnedCollection
        {
            private ViewSchema elementOwner;
            private CollectionSchema element;
            private DslTestStore<PatternModelDomainModel> store = new DslTestStore<PatternModelDomainModel>();

            [TestInitialize]
            public virtual void Initialize()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.elementOwner = this.store.ElementFactory.CreateElement<ViewSchema>();
                    this.element = this.store.ElementFactory.CreateElement<CollectionSchema>();
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
                    CollectionSchema element2 = this.store.ElementFactory.CreateElement<CollectionSchema>();
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
                    CollectionSchema element2 = this.store.ElementFactory.CreateElement<CollectionSchema>();
                    element2.Name = this.element.Name;
                    this.elementOwner.Elements.Add(element2);
                });
                this.element.ValidateNameIsUnique(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 1);
                Assert.True(validationContext.ValidationSubjects.IndexOf(this.element) == 0);
            }
        }

        [TestClass]
        [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "Test code")]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public class GivenACollectionOwnedCollection
        {
            private CollectionSchema elementOwner;
            private CollectionSchema element;
            private DslTestStore<PatternModelDomainModel> store = new DslTestStore<PatternModelDomainModel>();

            [TestInitialize]
            public virtual void Initialize()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.elementOwner = this.store.ElementFactory.CreateElement<CollectionSchema>();
                    this.element = this.store.ElementFactory.CreateElement<CollectionSchema>();
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
                    CollectionSchema element2 = this.store.ElementFactory.CreateElement<CollectionSchema>();
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
                    CollectionSchema element2 = this.store.ElementFactory.CreateElement<CollectionSchema>();
                    element2.Name = this.element.Name;
                    this.elementOwner.Elements.Add(element2);
                });
                this.element.ValidateNameIsUnique(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 1);
                Assert.True(validationContext.ValidationSubjects.IndexOf(this.element) == 0);
            }
        }

        [TestClass]
        [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "Test code")]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public class GivenAnElementOwnedCollection
        {
            private ElementSchema elementOwner;
            private CollectionSchema element;
            private DslTestStore<PatternModelDomainModel> store = new DslTestStore<PatternModelDomainModel>();

            [TestInitialize]
            public virtual void Initialize()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.elementOwner = this.store.ElementFactory.CreateElement<ElementSchema>();
                    this.element = this.store.ElementFactory.CreateElement<CollectionSchema>();
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
                    CollectionSchema element2 = this.store.ElementFactory.CreateElement<CollectionSchema>();
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
                    CollectionSchema element2 = this.store.ElementFactory.CreateElement<CollectionSchema>();
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