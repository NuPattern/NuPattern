using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.Modeling.Validation;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.Library.Automation;
using NuPattern.Modeling;
using NuPattern.Runtime.Extensibility;
using NuPattern.Runtime.References;
using NuPattern.Runtime.Store;

namespace NuPattern.Library.UnitTests.Automation.Artifact
{
    public class ArtifactReferenceValidationSpec
    {
        internal static readonly IAssertion Assert = new Assertion();
        private static ValidationContext validationContext;

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "TestCleanup")]
        public class GivenAProduct
        {
            private DslTestStore<ProductStateStoreDomainModel> store = new DslTestStore<ProductStateStoreDomainModel>();
            private Product product;
            private ArtifactReferenceValidation validator;
            private Mock<IFxrUriReferenceService> uriService;

            [TestInitialize]
            public void InitializeContext()
            {
                this.uriService = new Mock<IFxrUriReferenceService>();
                this.validator = new ArtifactReferenceValidation();
                this.validator.UriReferenceService = this.uriService.Object;

                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    var productStore = this.store.ElementFactory.CreateElement<ProductState>();
                    this.product = productStore.Create<Product>();
                });

                validationContext = new ValidationContext(ValidationCategories.Custom, this.product);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenNoArtifactReference_ThenValidateArtifactReferenceSucceeds()
            {
                this.validator.ValidateArtifactReferences(validationContext, this.product);

                Assert.True(validationContext.CurrentViolations.Count == 0);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenArtifactReferenceIsEmpty_ThenValidateArtifactReferenceSucceeds()
            {
                this.product.Store.TransactionManager.DoWithinTransaction(() =>
                    {
                        this.product.AddReference(ReferenceKindConstants.ArtifactLink, string.Empty);
                    });

                this.validator.ValidateArtifactReferences(validationContext, this.product);

                Assert.True(validationContext.CurrentViolations.Count == 0);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenArtifactReferenceNotExist_ThenValidateArtifactReferenceFails()
            {
                this.product.Store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.product.AddReference(ReferenceKindConstants.ArtifactLink, "Foo");
                });

                this.validator.ValidateArtifactReferences(validationContext, this.product);

                Assert.True(validationContext.CurrentViolations.Count == 1);
                Assert.True(validationContext.ValidationSubjects.IndexOf(this.product) == 0);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenArtifactReferenceExist_ThenValidateArtifactReferenceSucceeds()
            {
                this.product.Store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.product.AddReference(ReferenceKindConstants.ArtifactLink, "Bar");
                });

                Mock<IItemContainer> item = new Mock<IItemContainer>();
                item.Setup(i => i.Name).Returns("solution://Bar");
                this.uriService.Setup(service => service.ResolveUri<IItemContainer>(It.IsAny<Uri>())).Returns(item.Object);

                this.validator.ValidateArtifactReferences(validationContext, this.product);

                Assert.True(validationContext.CurrentViolations.Count == 0);
            }
        }
    }
}