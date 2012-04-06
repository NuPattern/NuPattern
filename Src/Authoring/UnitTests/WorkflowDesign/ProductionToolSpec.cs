﻿using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.Patterning.Authoring.WorkflowDesign;

namespace Microsoft.VisualStudio.Patterning.Authoring.UnitTests
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

			[TestMethod]
			public void ThenNotSatisfyingVariability()
			{
				Assert.False(this.tool.IsSatisfyingVariability);
			}

			[TestMethod]
			public void WhenConnectedToAVariabilityRequirement_ThenSatisfyingVariability()
			{
				this.tool.WithTransaction(tool => tool.VariabilityRequirements.Add(this.store.ElementFactory.CreateElement<VariabilityRequirement>()));

				Assert.True(this.tool.IsSatisfyingVariability);
			}
		}
	}
}
