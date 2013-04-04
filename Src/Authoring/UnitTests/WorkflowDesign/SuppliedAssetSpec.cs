using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuPattern.Authoring.WorkflowDesign;
using NuPattern.Modeling;

namespace NuPattern.Authoring.UnitTests
{
    [TestClass]
    public class SuppliedAssetSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public class GivenAnSuppliedAsset
        {
            private DslTestStore<WorkflowDesignDomainModel> store = new DslTestStore<WorkflowDesignDomainModel>();
            private SuppliedAsset asset;

            [TestInitialize]
            public void InitializeContext()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.asset = this.store.ElementFactory.CreateElement<SuppliedAsset>();
                });
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenSourceReferenceEmpty()
            {
                Assert.Equal(this.asset.SourceReference, string.Empty);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenSuppliedToATool_ThenSuppliesATool()
            {
                this.asset.WithTransaction(asset => asset.ProductionTools.Add(this.asset.Store.ElementFactory.CreateElement<ProductionTool>()));

                Assert.True(this.asset.IsSuppliedToTool);
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenIsNotUserSupplied()
            {
                Assert.False(this.asset.IsUserSupplied);
            }
        }
    }
}
