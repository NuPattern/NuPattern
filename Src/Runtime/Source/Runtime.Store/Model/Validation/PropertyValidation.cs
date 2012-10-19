using System;
using System.Linq;
using Microsoft.VisualStudio.Patterning.Runtime.Store.Properties;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using Microsoft.VisualStudio.Modeling.Validation;

namespace Microsoft.VisualStudio.Patterning.Runtime.Store
{
	/// <summary>
	/// Performs runtime property validation.
	/// </summary>
	public partial class Property
	{
		/// <summary>
		/// Triggers this notification rule whether a <see cref="Property"/> is saved.
		/// </summary>
		[ValidationMethod(ValidationCategories.Save)]
		public void OnSaved(ValidationContext context)
		{
			Guard.NotNull(() => context, context);

			this.SaveProvidedValue();
		}
	}
}