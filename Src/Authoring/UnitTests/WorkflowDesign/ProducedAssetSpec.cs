using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuPattern.Authoring.WorkflowDesign;
using NuPattern.Modeling;

namespace NuPattern.Authoring.UnitTests
{
    [TestClass]
    public partial class ProducedAssetSpec : AssetSpec
    {
        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        partial class GivenAnProducedAsset : GivenAnAsset<ProducedAsset>
        {
            [TestMethod, TestCategory("Unit")]
            public void WhenSuppliedToATool_ThenSuppliesATool()
            {
                this.Asset.WithTransaction(asset => asset.ProductionTools.Add(this.Asset.Store.ElementFactory.CreateElement<ProductionTool>()));

                Assert.True(this.Asset.IsSuppliedToTool);
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenIsNotFinal()
            {
                Assert.False(this.Asset.IsFinal);
            }
        }
    }
}
