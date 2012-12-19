using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.Modeling.Validation;

namespace NuPattern.Authoring.WorkflowDesign
{
	/// <summary>
	/// Customizations for the ProducedAsset class.
	/// </summary>
	[ValidationState(ValidationState.Enabled)]
	public partial class ProducedAsset
	{
		/// <summary>
		/// Validates if an produced asset has a duplicate name (with another).
		/// </summary>
		[ValidationMethod(ValidationCategories.Save | ValidationCategories.Menu)]
		internal void ValidateNameIsUnique(ValidationContext context)
		{
			IEnumerable<ProducedAsset> sameNamedElements = this.Store.ElementDirectory.AllElements.OfType<ProducedAsset>()
				.Where(element => element.Name == this.Name);

			if (sameNamedElements.Count() > 1)
			{
				context.LogError(
					string.Format(CultureInfo.CurrentCulture, Properties.Resources.Validate_ProducedAssetNameIsNotUnique, this.Name),
					Properties.Resources.Validate_ProducedAssetNameIsNotUniqueCode, this);
			}
		}
	}
}
