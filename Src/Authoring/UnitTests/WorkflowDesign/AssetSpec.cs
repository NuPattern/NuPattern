using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.Patterning.Authoring.WorkflowDesign;

namespace Microsoft.VisualStudio.Patterning.Authoring.UnitTests
{
	[TestClass]
	public abstract class AssetSpec
	{
		internal static readonly IAssertion Assert = new Assertion();

		[TestClass]
		[SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
		public abstract class GivenAnAsset<TAsset> where TAsset : Asset
		{
            private DslTestStore<WorkflowDesignDomainModel> store = new DslTestStore<WorkflowDesignDomainModel>();

			[TestInitialize]
			public virtual void Initialize()
			{
				this.store.TransactionManager.DoWithinTransaction(() =>
				{
					this.Asset = this.store.ElementFactory.CreateElement<TAsset>();
				});
			}

			protected TAsset Asset { get; private set; }

			[TestMethod, TestCategory("Unit")]
			public void ThenSourceReferenceEmpty()
			{
				Assert.Equal(this.Asset.SourceReference, string.Empty);
			}

			[TestMethod, TestCategory("Unit")]
			public void ThenNotSupplyingAnyTools()
			{
				Assert.False(this.Asset.IsSuppliedToTool);
			}
		}
	}
}
