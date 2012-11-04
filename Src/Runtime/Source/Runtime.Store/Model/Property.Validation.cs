using System;
using System.Linq;
using Microsoft.VisualStudio.Modeling.Validation;
using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.Patterning.Runtime.Store.Properties;

namespace Microsoft.VisualStudio.Patterning.Runtime.Store
{
	[ValidationState(ValidationState.Enabled)]
	public partial class Property
	{
		[ValidationMethod(ValidationCategories.Custom, CustomCategory = ValidationConstants.RuntimeValidationCategory)]
		private void ValidateInstance(ValidationContext context)
		{
			try
			{
				if (this.Info == null || this.Info.ValidationSettings == null || !this.Info.ValidationSettings.Any())
					return;

				foreach (var binding in this.ValidationBindings)
				{
					if (binding.Evaluate(this.BindingContext))
					{
						var results = binding.Value.Validate();
						if (results != null) // null result is considered Success
						{
							foreach (var result in results)
							{
								context.LogError(result.ErrorMessage, Resources.Property_ValidationFailedErrorCode, this);
							}
						}
					}
					else
					{
						context.LogError(Resources.Property_BindingEvaluationFailed, Resources.Property_BindingEvaluationErrorCode, this);
					}
				}
			}
			catch (Exception ex)
			{
				tracer.TraceError(
					ex,
					Resources.ValidationMethodFailed_Error,
					Reflector<Property>.GetMethod(n => n.ValidateInstance(context)).Name);

				throw;
			}
		}
	}
}
