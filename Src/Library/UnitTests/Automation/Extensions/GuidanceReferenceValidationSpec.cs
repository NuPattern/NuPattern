using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.Modeling.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.Library.Automation;
using NuPattern.Modeling;
using NuPattern.Runtime;
using NuPattern.Runtime.Guidance;
using NuPattern.Runtime.References;
using NuPattern.Runtime.Store;

namespace NuPattern.Library.UnitTests.Automation.Guidance
{
    [TestClass]
    public class GuidanceReferenceValidationSpec
    {
        internal static readonly IAssertion Assert = new Assertion();
        private static ValidationContext validationContext;

        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "TestCleanup")]
        [TestClass]
        public class GivenAProduct
        {
            private DslTestStore<ProductStateStoreDomainModel> store = new DslTestStore<ProductStateStoreDomainModel>();
            private Product product;
            private GuidanceReferenceValidation validator;
            private Mock<IGuidanceManager> guidanceManager;

            [TestInitialize]
            public void InitializeContext()
            {
                this.guidanceManager = new Mock<IGuidanceManager>();
                this.validator = new GuidanceReferenceValidation();
                this.validator.GuidanceManager = this.guidanceManager.Object;

                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    var productStore = this.store.ElementFactory.CreateElement<ProductState>();
                    this.product = productStore.Create<Product>();
                });

                validationContext = new ValidationContext(ValidationCategories.Custom, this.product);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenNoGuidanceReference_ThenValidateGuidanceReferenceSucceeds()
            {
                this.validator.ValidateGuidanceReference(validationContext, this.product);

                Assert.True(validationContext.CurrentViolations.Count == 0);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenGuidanceReferenceIsEmpty_ThenValidateGuidanceReferenceSucceeds()
            {
                this.product.Store.TransactionManager.DoWithinTransaction(() =>
                    {
                        this.product.AddReference(ReferenceKindConstants.GuidanceTopic, string.Empty);
                    });

                this.validator.ValidateGuidanceReference(validationContext, this.product);

                Assert.True(validationContext.CurrentViolations.Count == 0);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenGuidanceReferenceNotExist_ThenValidateGuidanceReferenceFails()
            {
                this.product.Store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.product.AddReference(ReferenceKindConstants.GuidanceTopic, "Foo");
                });

                this.validator.ValidateGuidanceReference(validationContext, this.product);

                Assert.True(validationContext.CurrentViolations.Count == 1);
                Assert.True(validationContext.ValidationSubjects.IndexOf(this.product) == 0);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenGuidanceReferenceExist_ThenValidateGuidanceReferenceSucceeds()
            {
                this.product.Store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.product.AddReference(ReferenceKindConstants.GuidanceTopic, "Bar");
                });

                Mock<NuPattern.Runtime.Guidance.IGuidanceExtension> feature = new Mock<NuPattern.Runtime.Guidance.IGuidanceExtension>();
                feature.Setup(f => f.InstanceName).Returns("Bar");
                this.guidanceManager.Setup(manager => manager.InstantiatedGuidanceExtensions).Returns(new[] { feature.Object });

                this.validator.ValidateGuidanceReference(validationContext, this.product);

                Assert.True(validationContext.CurrentViolations.Count == 0);
            }
        }
    }
}