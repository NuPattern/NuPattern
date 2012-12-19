using Microsoft.VisualStudio.Modeling.Validation;

namespace NuPattern.Runtime.Store
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