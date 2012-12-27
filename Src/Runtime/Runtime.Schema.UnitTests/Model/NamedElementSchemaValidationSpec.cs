using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.Modeling.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuPattern.Extensibility;

namespace NuPattern.Runtime.Schema.UnitTests
{
    [TestClass]
    public class NamedElementSchemaValidationSpec
    {
        private const string TestStringOnlyAlphas = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqurstuvwxyz1234567890";
        private const string TestStringWithSpaces = TestStringOnlyAlphas + " " + TestStringOnlyAlphas;
        private const string TestStringWithAnAsterix = TestStringOnlyAlphas + "*" + TestStringOnlyAlphas;
        private static readonly string TestStringWith513Chars = new string('A', 513);

        private static readonly IAssertion Assert = new Assertion();
        private static ValidationContext validationContext;

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public class GivenANamedElement
        {
            private NamedElementSchema element = null;
            private DslTestStore<PatternModelDomainModel> store = new DslTestStore<PatternModelDomainModel>();

            [TestInitialize]
            public void Initialize()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.element = this.store.ElementFactory.CreateElement<ElementSchema>();
                });
                validationContext = new ValidationContext(ValidationCategories.Save, this.element);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenNameIsAlpanumeric_ThenValidateNameIsLegalSucceeds()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.element.Name = TestStringOnlyAlphas;
                });
                this.element.ValidateNameIsLegal(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 0);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenNameIsEmpty_ThenValidateNameIsLegalFails()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.element.Name = string.Empty;
                });
                this.element.ValidateNameIsLegal(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 1);
                Assert.True(validationContext.ValidationSubjects.IndexOf(this.element) == 0);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCodeIdentifierIsReserved_ThenValidateCodeIdentiferIsNotReservedFails()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.element.Name = "InstanceName";
                });
                this.element.ValidateCodeIdentifierIsNotReserved(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 1);
                Assert.True(validationContext.ValidationSubjects.IndexOf(this.element) == 0);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCodeIdentifierIsNotReserved_ThenValidateCodeIdentiferIsNotReservedSucceeds()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.element.Name = "Foo";
                });
                this.element.ValidateCodeIdentifierIsNotReserved(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 0);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenNameHasSpaces_ThenValidateNameIsLegalFails()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.element.Name = TestStringWithSpaces;
                });
                this.element.ValidateNameIsLegal(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 1);
                Assert.True(validationContext.ValidationSubjects.IndexOf(this.element) == 0);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenNameHasSpecialChars_ThenValidateNameIsLegalFails()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.element.Name = TestStringWithAnAsterix;
                });
                this.element.ValidateNameIsLegal(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 1);
                Assert.True(validationContext.ValidationSubjects.IndexOf(this.element) == 0);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenNameHasMoreThan512Chars_ThenValidateNameIsLegalFails()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.element.Name = TestStringWith513Chars;
                });
                this.element.ValidateNameIsLegal(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 1);
                Assert.True(validationContext.ValidationSubjects.IndexOf(this.element) == 0);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenDisplayNameIsAplhanumeric_ThenValidateDisplayNameIsLegalSucceeds()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.element.DisplayName = TestStringOnlyAlphas;
                });
                this.element.ValidateDisplayNameIsLegal(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 0);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenDisplayNameIsEmpty_ThenValidateDisplayNameIsLegalFails()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.element.DisplayName = string.Empty;
                });
                this.element.ValidateDisplayNameIsLegal(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 1);
                Assert.True(validationContext.ValidationSubjects.IndexOf(this.element) == 0);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenDisplayNameHasSpaces_ThenValidateDisplayNameIsLegalSucceeds()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.element.DisplayName = TestStringWithSpaces;
                });
                this.element.ValidateDisplayNameIsLegal(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 0);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenDisplayNameHasMoreThan512Chars_ThenValidateDisplayNameIsLegalFails()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.element.DisplayName = TestStringWith513Chars;
                });
                this.element.ValidateDisplayNameIsLegal(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 1);
                Assert.True(validationContext.ValidationSubjects.IndexOf(this.element) == 0);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCodeIdentifierIsAlpanumeric_ThenValidateCodeIdentifierIsLegalSucceeds()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.element.CodeIdentifier = TestStringOnlyAlphas;
                });
                this.element.ValidateCodeIdentifierIsLegal(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 0);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCodeIdentifierIsEmpty_ThenValidateCodeIdentifierIsLegalFails()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.element.CodeIdentifier = string.Empty;
                });
                this.element.ValidateCodeIdentifierIsLegal(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 1);
                Assert.True(validationContext.ValidationSubjects.IndexOf(this.element) == 0);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCodeIdentifierIsDuplicate_ThenValidateCodeIdentifierIsUniqueFails()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.element.CodeIdentifier = "Foo";
                    var element2 = this.store.ElementFactory.CreateElement<ElementSchema>();
                    element2.CodeIdentifier = this.element.CodeIdentifier;
                });
                this.element.ValidateCodeIdentifierIsUnique(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 1);
                Assert.True(validationContext.ValidationSubjects.IndexOf(this.element) == 0);
            }
        }
    }
}