using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NuPattern.Runtime
{
	/// <summary>
	/// Define a validation rule that applies to an element.
	/// </summary>
	public interface IValidationRule
	{
		/// <summary>
		/// Run the defined validation.
		/// </summary>
		/// <returns>The <see cref="ValidationResult"/> collection with the validation errors. If the validation was
		/// successful returns an empty enumerable or <see langword="null"/>.</returns>
		IEnumerable<ValidationResult> Validate();
	}
}