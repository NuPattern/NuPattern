using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.Modeling.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.Patterning.Authoring.WorkflowDesign;

namespace Microsoft.VisualStudio.Patterning.Authoring.UnitTests
{
	[TestClass]
	public class ProductionToolValidationSpec
	{
		internal static readonly IAssertion Assert = new Assertion();
		private static ValidationContext validationContext;

		[TestClass]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
		public class GivenAnProductionTool
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
				validationContext = new ValidationContext(ValidationCategories.Save, this.tool);
			}

			[TestMethod]
			public void ThenValidateNameIsUniqueSucceeds()
			{
				this.tool.ValidateNameIsUnique(validationContext);

				Assert.True(validationContext.CurrentViolations.Count == 0);
			}

			[TestMethod]
			public void WhenSameNamedElementAddedToDesign_ThenValidateNameIsUniqueFails()
			{
				this.store.TransactionManager.DoWithinTransaction(() =>
				{
					ProductionTool tool2 = this.store.ElementFactory.CreateElement<ProductionTool>();
					tool2.Name = this.tool.Name;
				});
				this.tool.ValidateNameIsUnique(validationContext);

				Assert.True(validationContext.CurrentViolations.Count == 1);
				Assert.True(validationContext.ValidationSubjects.IndexOf(this.tool) == 0);
			}
		}
	}
}
