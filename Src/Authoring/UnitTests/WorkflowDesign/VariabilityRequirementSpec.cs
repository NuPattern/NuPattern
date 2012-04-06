using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.Patterning.Authoring.WorkflowDesign;

namespace Microsoft.VisualStudio.Patterning.Authoring.UnitTests
{
	[TestClass]
	public class VariabilityRequirementSpec
	{
		internal static readonly IAssertion Assert = new Assertion();

		[TestClass]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
		public class GivenAVariabilityRequirement
		{
			private VariabilityRequirement requirement;
            private DslTestStore<WorkflowDesignDomainModel> store = new DslTestStore<WorkflowDesignDomainModel>();

			[TestInitialize]
			public virtual void Initialize()
			{
				this.store.TransactionManager.DoWithinTransaction(() =>
				{
					this.requirement = this.store.ElementFactory.CreateElement<VariabilityRequirement>();
				});
			}

			[TestMethod]
			public void ThenNotSatifiedByProductionTools()
			{
				Assert.False(this.requirement.IsSatisfiedByProductionTool);
			}

			[TestMethod]
			public void WhenConnectedToATool_ThenSatifiedByAProductionTool()
			{
				this.requirement.WithTransaction(requirement => requirement.ProductionTools.Add(this.store.ElementFactory.CreateElement<ProductionTool>()));

				Assert.True(this.requirement.IsSatisfiedByProductionTool);
			}
		}
	}
}
