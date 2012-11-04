using Microsoft.VisualStudio.Modeling.Validation;
using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.Patterning.Runtime.Store;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.VisualStudio.Patterning.Runtime.UnitTests
{
    [TestClass]
    public class PatternElementValidationSpec
    {
        internal static readonly IAssertion Assert = new Assertion();
        private static ValidationContext validationContext;

        [TestClass]
        public class GivenAProductInfo
        {
            private DslTestStore<ProductStateStoreDomainModel> store = new DslTestStore<ProductStateStoreDomainModel>();
            private Product product;

            [TestInitialize]
            public void Initialize()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    var productStore = this.store.ElementFactory.CreateElement<ProductState>();
                    this.product = productStore.Create<Product>();
                });

                validationContext = new ValidationContext(ValidationCategories.Custom, this.product);
            }

            [TestCleanup]
            public void CleanUp()
            {
                this.store.Dispose();
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenInstanceNameIsEmpty_ThenValidateInstanceNameIsLegalFails()
            {
                this.product.InstanceName = string.Empty;

                this.product.ValidateInstanceNameIsLegal(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 1);
                Assert.Equal(validationContext.ValidationSubjects[0], this.product);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenInstanceNameStartWithAlpha_ThenValidateInstanceNameIsLegalSucceeds()
            {
                this.product.InstanceName = "9foo";

                this.product.ValidateInstanceNameIsLegal(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 0);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenInstanceNameStartWithUnderscore_ThenValidateInstanceNameIsLegalSucceeds()
            {
                this.product.InstanceName = "_9foo";

                this.product.ValidateInstanceNameIsLegal(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 0);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenInstanceNameNotStartWithAlpha_ThenValidateInstanceNameIsLegalFails()
            {
                this.product.InstanceName = "*9foo";

                this.product.ValidateInstanceNameIsLegal(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 1);
                Assert.Equal(validationContext.ValidationSubjects[0], this.product);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenInstanceNameStartsWithSpaces_ThenValidateInstanceNameIsLegalFails()
            {
                this.product.InstanceName = " foo";

                this.product.ValidateInstanceNameIsLegal(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 1);
                Assert.Equal(validationContext.ValidationSubjects[0], this.product);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenInstanceNameContainsSpaces_ThenValidateInstanceNameIsLegalSucceeds()
            {
                this.product.InstanceName = "foo and bar";

                this.product.ValidateInstanceNameIsLegal(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 0);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenInstanceNameEndsWithSpace_ThenValidateInstanceNameIsLegalFails()
            {
                this.product.InstanceName = "Foo bar ";

                this.product.ValidateInstanceNameIsLegal(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 1);
                Assert.Equal(validationContext.ValidationSubjects[0], this.product);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenInstanceNameSingleCommonChar_ThenValidateInstanceNameIsLegalFails()
            {
                this.product.InstanceName = "*";

                this.product.ValidateInstanceNameIsLegal(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 1);
                Assert.Equal(validationContext.ValidationSubjects[0], this.product);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenInstanceNameSingleChar_ThenValidateInstanceNameIsLegalSucceeds()
            {
                this.product.InstanceName = "d";

                this.product.ValidateInstanceNameIsLegal(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 0);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenInstanceNameSingleNumeral_ThenValidateInstanceNameIsLegalSucceeds()
            {
                this.product.InstanceName = "2";

                this.product.ValidateInstanceNameIsLegal(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 0);
            }
        }
    }
}
