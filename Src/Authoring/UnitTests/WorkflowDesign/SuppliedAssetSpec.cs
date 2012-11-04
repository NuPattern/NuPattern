﻿using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.Patterning.Authoring.WorkflowDesign;

namespace Microsoft.VisualStudio.Patterning.Authoring.UnitTests
{
	[TestClass]
	public class SuppliedAssetSpec : AssetSpec
	{
		[TestClass]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
		public class GivenAnSuppliedAsset : GivenAnAsset<SuppliedAsset>
		{
			[TestMethod, TestCategory("Unit")]
			public void WhenSuppliedToATool_ThenSuppliesATool()
			{
				this.Asset.WithTransaction(asset => asset.ProductionTools.Add(this.Asset.Store.ElementFactory.CreateElement<ProductionTool>()));

				Assert.True(this.Asset.IsSuppliedToTool);
			}

			[TestMethod, TestCategory("Unit")]
			public void ThenIsNotUserSupplied()
			{
				Assert.False(this.Asset.IsUserSupplied);
			}
		}
	}
}
