using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NuPattern.Runtime;

namespace NuPattern.Extensibility
{
	/// <summary>
	/// Defines a <see cref="ValidationRule"/> that is in the dependency injection container.
	/// </summary>
	[ValidationRule]
	public abstract class ValidationRule : IValidationRule
	{
		/// <summary>
		/// Run the defined validation.
		/// </summary>
		/// <returns>The <see cref="ValidationResult"/> collection with the validation errors. If the validation was
		/// successful returns an empty enumerable or <see langword="null"/>.</returns>
		public abstract IEnumerable<ValidationResult> Validate();
	}
}