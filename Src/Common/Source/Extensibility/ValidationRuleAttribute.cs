using System;
using Microsoft.VisualStudio.Patterning.Runtime;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;

namespace Microsoft.VisualStudio.Patterning.Extensibility
{
	/// <summary>
	/// Defines a way to export a validation rule to MEF.
	/// </summary>
	[CLSCompliant(false)]
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class ValidationRuleAttribute : FeatureComponentAttribute
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ValidationRuleAttribute"/> class.
		/// </summary>
		public ValidationRuleAttribute()
			: base(typeof(IValidationRule))
		{
		}
	}
}