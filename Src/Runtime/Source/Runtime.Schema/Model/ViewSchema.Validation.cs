using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.Modeling.Validation;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using NuPattern.Extensibility;
using NuPattern.Runtime.Schema.Properties;

namespace NuPattern.Runtime.Schema
{
	/// <summary>
	/// Customizations for the ViewSchema class.
	/// </summary>
	[ValidationState(ValidationState.Enabled)]
	public partial class ViewSchema
	{
		private static readonly ITraceSource tracer = Tracer.GetSourceFor<ViewSchema>();

		/// <summary>
		/// Validates if an view has a duplicate name (with another view).
		/// </summary>
		[ValidationMethod(ValidationCategories.Save | ValidationCategories.Menu)]
		internal void ValidateNameIsUnique(ValidationContext context)
		{
			try
			{
				IEnumerable<ViewSchema> sameNamedElements = this.Pattern.Views
					.Where(view => view.Name.Equals(this.Name, System.StringComparison.OrdinalIgnoreCase));

				if (sameNamedElements.Count() > 1)
				{
					context.LogError(
						string.Format(CultureInfo.CurrentCulture, Resources.Validate_ViewNameIsNotUnique, this.Name),
						Resources.Validate_ViewNameIsNotUniqueCode, this);
				}
			}
			catch (Exception ex)
			{
				tracer.TraceError(
					ex,
					Resources.ValidationMethodFailed_Error,
					Reflector<ViewSchema>.GetMethod(n => n.ValidateNameIsUnique(context)).Name);

				throw;
			}
		}

		/// <summary>
		/// Validates if the view is default and is hidden.
		/// </summary>
		[ValidationMethod(ValidationCategories.Save | ValidationCategories.Menu)]
		internal void ValidateDefaultIsHidden(ValidationContext context)
		{
			try
			{
				if (this.IsDefault && !this.IsVisible)
				{
					context.LogError(
						string.Format(CultureInfo.CurrentCulture, Resources.Validate_ViewDefaultIsHidden, this.Name),
						Resources.Validate_ViewDefaultIsHidden, this);
				}
			}
			catch (Exception ex)
			{
				tracer.TraceError(
					ex,
					Resources.ValidationMethodFailed_Error,
					Reflector<ViewSchema>.GetMethod(n => n.ValidateDefaultIsHidden(context)).Name);

				throw;
			}
		}
	}
}