using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.Modeling.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuPattern.Authoring.WorkflowDesign;
using NuPattern.Extensibility;

namespace NuPattern.Authoring.UnitTests
{
	public partial class ProducedAssetSpec
	{
		private static ValidationContext validationContext;

		[SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
		public partial class GivenAnProducedAsset
		{
			[TestInitialize]
			public override void Initialize()
			{
				base.Initialize();

				validationContext = new ValidationContext(ValidationCategories.Save, this.Asset);
			}

			[TestMethod, TestCategory("Unit")]
			public void WhenNoToolsAndNotFinal_ThenValidateProducedAssetNotIntermediateAndNotFinalFails()
			{
				this.Asset.WithTransaction(asset => asset.IsFinal = false);

				this.Asset.ValidateProducedAssetNotIntermediateAndNotFinal(validationContext);

				Assert.True(validationContext.CurrentViolations.Count == 1);
			}

			[TestMethod, TestCategory("Unit")]
			public void ThenValidateNameIsUniqueSucceeds()
			{
				this.Asset.ValidateNameIsUnique(validationContext);

				Assert.True(validationContext.CurrentViolations.Count == 0);
			}

			[TestMethod, TestCategory("Unit")]
			public void WhenSameNamedElementAddedToDesign_ThenValidateNameIsUniqueFails()
			{
				this.Asset.Store.TransactionManager.DoWithinTransaction(() =>
				{
					ProducedAsset asset2 = this.Asset.Store.ElementFactory.CreateElement<ProducedAsset>();
					asset2.Name = this.Asset.Name;
				});
				this.Asset.ValidateNameIsUnique(validationContext);

				Assert.True(validationContext.CurrentViolations.Count == 1);
				Assert.True(validationContext.ValidationSubjects.IndexOf(this.Asset) == 0);
			}
		}
	}
}
