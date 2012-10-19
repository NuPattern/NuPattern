using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.Modeling.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.Patterning.Authoring.WorkflowDesign;

namespace Microsoft.VisualStudio.Patterning.Authoring.UnitTests
{
	[TestClass]
	public class SuppliedAssetValidationSpec
	{
		internal static readonly IAssertion Assert = new Assertion();
		private static ValidationContext validationContext;

		[TestClass]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
		public class GivenAnSuppliedAsset
		{
			private SuppliedAsset asset;
            private DslTestStore<WorkflowDesignDomainModel> store = new DslTestStore<WorkflowDesignDomainModel>();

			[TestInitialize]
			public virtual void Initialize()
			{
				this.store.TransactionManager.DoWithinTransaction(() =>
				{
					this.asset = this.store.ElementFactory.CreateElement<SuppliedAsset>();
				});
				validationContext = new ValidationContext(ValidationCategories.Save, this.asset);
			}

			[TestMethod]
			public void ThenValidateNameIsUniqueSucceeds()
			{
				this.asset.ValidateNameIsUnique(validationContext);

				Assert.True(validationContext.CurrentViolations.Count == 0);
			}

			[TestMethod]
			public void WhenSameNamedElementAddedToDesign_ThenValidateNameIsUniqueFails()
			{
				this.store.TransactionManager.DoWithinTransaction(() =>
				{
					SuppliedAsset asset2 = this.store.ElementFactory.CreateElement<SuppliedAsset>();
					asset2.Name = this.asset.Name;
				});
				this.asset.ValidateNameIsUnique(validationContext);

				Assert.True(validationContext.CurrentViolations.Count == 1);
				Assert.True(validationContext.ValidationSubjects.IndexOf(this.asset) == 0);
			}
		}
	}
}
