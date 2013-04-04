using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuPattern.Authoring.WorkflowDesign;
using NuPattern.Modeling;

namespace NuPattern.Authoring.UnitTests
{
    [TestClass]
    public class ProductionToolSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public class GivenAProductionTool
        {
            private ProductionTool tool;
            private DslTestStore<WorkflowDesignDomainModel> store = new DslTestStore<WorkflowDesignDomainModel>();

            [TestInitialize]
            public virtual void Initialize()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.tool = this.store.ElementFactory.CreateElement<ProductionTool>();
                });
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenNotSatisfyingVariability()
            {
                Assert.False(this.tool.IsSatisfyingVariability);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenConnectedToAVariabilityRequirement_ThenSatisfyingVariability()
            {
                this.tool.WithTransaction(tool => tool.VariabilityRequirements.Add(this.store.ElementFactory.CreateElement<VariabilityRequirement>()));

                Assert.True(this.tool.IsSatisfyingVariability);
            }
        }
    }
}
