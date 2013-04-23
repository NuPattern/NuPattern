using Microsoft.VisualStudio.Modeling.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuPattern.Authoring.WorkflowDesign;
using NuPattern.Modeling;

namespace NuPattern.Authoring.UnitTests
{
    [TestClass]
    public class ProducedAssetValidationSpec
    {
        internal static readonly IAssertion Assert = new Assertion();
        private static ValidationContext validationContext;

        [TestClass]
        public class GivenAnProducedAsset
        {
            private DslTestStore<WorkflowDesignDomainModel> store = new DslTestStore<WorkflowDesignDomainModel>();
            private ProducedAsset asset;

            [TestInitialize]
            public void InitializeContext()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.asset = this.store.ElementFactory.CreateElement<ProducedAsset>();
                });
                validationContext = new ValidationContext(ValidationCategories.Save, this.asset);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenNoToolsAndNotFinal_ThenValidateProducedAssetNotIntermediateAndNotFinalFails()
            {
                this.asset.WithTransaction(asset => asset.IsFinal = false);

                this.asset.ValidateProducedAssetNotIntermediateAndNotFinal(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 1);
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenValidateNameIsUniqueSucceeds()
            {
                this.asset.ValidateNameIsUnique(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 0);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenSameNamedElementAddedToDesign_ThenValidateNameIsUniqueFails()
            {
                this.asset.Store.TransactionManager.DoWithinTransaction(() =>
                {
                    ProducedAsset asset2 = this.asset.Store.ElementFactory.CreateElement<ProducedAsset>();
                    asset2.Name = this.asset.Name;
                });
                this.asset.ValidateNameIsUnique(validationContext);

                Assert.True(validationContext.CurrentViolations.Count == 1);
                Assert.True(validationContext.ValidationSubjects.IndexOf(this.asset) == 0);
            }
        }
    }
}
