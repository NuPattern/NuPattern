using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.Modeling.Validation;

namespace NuPattern.Authoring.WorkflowDesign
{
	/// <summary>
	/// Customizations for the SuppliedAsset class.
	/// </summary>
	[ValidationState(ValidationState.Enabled)]
	public partial class SuppliedAsset
	{
		/// <summary>
		/// Validates if an supplied asset has a duplicate name (with another).
		/// </summary>
		[ValidationMethod(ValidationCategories.Save | ValidationCategories.Menu)]
		internal void ValidateNameIsUnique(ValidationContext context)
		{
			IEnumerable<SuppliedAsset> sameNamedElements = this.Store.ElementDirectory.AllElements.OfType<SuppliedAsset>()
				.Where(element => element.Name == this.Name);

			if (sameNamedElements.Count() > 1)
			{
				context.LogError(
					string.Format(CultureInfo.CurrentCulture, Properties.Resources.Validate_SuppliedAssetNameIsNotUnique, this.Name),
					Properties.Resources.Validate_SuppliedAssetNameIsNotUniqueCode, this);
			}
		}
	}
}
