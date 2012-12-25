using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.Modeling.Validation;

namespace NuPattern.Authoring.WorkflowDesign
{
	/// <summary>
	/// Customizations for the ProductionTool class.
	/// </summary>
	[ValidationState(ValidationState.Enabled)]
	public partial class ProductionTool
	{
		/// <summary>
		/// Validates if an tool has a duplicate name (with another tool).
		/// </summary>
		[ValidationMethod(ValidationCategories.Save | ValidationCategories.Menu)]
		internal void ValidateNameIsUnique(ValidationContext context)
		{
			IEnumerable<ProductionTool> sameNamedElements = this.Store.ElementDirectory.AllElements.OfType<ProductionTool>()
				.Where(element => element.Name == this.Name);

			if (sameNamedElements.Count() > 1)
			{
				context.LogError(
					string.Format(CultureInfo.CurrentCulture, Properties.Resources.Validate_ProductionToolNameIsNotUnique, this.Name),
					Properties.Resources.Validate_ProductionToolNameIsNotUniqueCode, this);
			}
		}
	}
}
