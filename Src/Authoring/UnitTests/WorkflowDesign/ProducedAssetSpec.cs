using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.Patterning.Authoring.WorkflowDesign;

namespace Microsoft.VisualStudio.Patterning.Authoring.UnitTests
{
	[TestClass]
	public partial class ProducedAssetSpec : AssetSpec
	{
		[TestClass]
		[SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
		public partial class GivenAnProducedAsset : GivenAnAsset<ProducedAsset>
		{
			[TestMethod]
			public void WhenSuppliedToATool_ThenSuppliesATool()
			{
				this.Asset.WithTransaction(asset => asset.ProductionTools.Add(this.Asset.Store.ElementFactory.CreateElement<ProductionTool>()));

				Assert.True(this.Asset.IsSuppliedToTool);
			}

			[TestMethod]
			public void ThenIsNotFinal()
			{
				Assert.False(this.Asset.IsFinal);
			}
		}
	}
}
