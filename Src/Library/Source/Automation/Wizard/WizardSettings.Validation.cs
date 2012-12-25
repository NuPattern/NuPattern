using System;
using System.Globalization;
using Microsoft.VisualStudio.Modeling.Validation;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using NuPattern.Extensibility;
using NuPattern.Library.Properties;

namespace NuPattern.Library.Automation
{
	/// <summary>
	/// Custom validation rules.
	/// </summary>
	[ValidationState(ValidationState.Enabled)]
	public partial class WizardSettings
	{
		private static readonly ITraceSource tracer = Tracer.GetSourceFor<WizardSettings>();

		/// <summary>
		/// Validates that the TypeId is not empty, and exists.
		/// </summary>
		[ValidationMethod(ValidationCategories.Save | ValidationCategories.Menu)]
		internal void ValidateTypeNameNotEmpty(ValidationContext context)
		{
			try
			{
				// Ensure not empty
				if (string.IsNullOrEmpty(this.TypeName))
				{
					context.LogError(
						string.Format(
							CultureInfo.CurrentCulture,
							Resources.Validate_WizardSettingsTypeIsNotEmpty,
							this.Name),
						Resources.Validate_WizardSettingsTypeIsNotEmptyCode, this.Extends);
				}
			}
			catch (Exception ex)
			{
				tracer.TraceError(
					ex,
					Resources.ValidationMethodFailed_Error,
					Reflector<WizardSettings>.GetMethod(n => n.ValidateTypeNameNotEmpty(context)).Name);

				throw;
			}
		}
	}
}