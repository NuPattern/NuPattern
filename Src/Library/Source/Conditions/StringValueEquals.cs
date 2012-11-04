using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.Patterning.Library.Properties;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;

namespace Microsoft.VisualStudio.Patterning.Library.Conditions
{
	/// <summary>
	/// A <see cref="Condition"/> that evaluates to true id the two strings compare with given comparison.
	/// </summary>
	[CategoryResource("AutomationCategory_General", typeof(Resources))]
	[DescriptionResource("StringValueEqualsCondition_Description", typeof(Resources))]
	[DisplayNameResource("StringValueEqualsCondition_DisplayName", typeof(Resources))]
	[CLSCompliant(false)]
	public class StringValueEqualsCondition : Condition
	{
		private static readonly ITraceSource tracer = Tracer.GetSourceFor<StringValueEqualsCondition>();

		/// <summary>
		/// Creates a new instance of the <see cref="StringValueEqualsCondition"/> class.
		/// </summary>
		public StringValueEqualsCondition()
		{
			this.ComparisonKind = StringComparison.OrdinalIgnoreCase;
			this.LeftValue = string.Empty;
			this.RightValue = string.Empty;
		}

		/// <summary>
		/// Gets the kind of comparison.
		/// </summary>
		[DescriptionResource("StringValueEqualsCondition_ComparisonKind_Description", typeof(Resources))]
		[DisplayNameResource("StringValueEqualsCondition_ComparisonKind_DisplayName", typeof(Resources))]
		[Required]
		public StringComparison ComparisonKind { get; set; }

		/// <summary>
		/// Gets the left value to compare
		/// </summary>
		[DescriptionResource("StringValueEqualsCondition_LeftValue_Description", typeof(Resources))]
		[DisplayNameResource("StringValueEqualsCondition_LeftValue_DisplayName", typeof(Resources))]
		[Required]
		public string LeftValue { get; set; }

		/// <summary>
		/// Gets the right value to compare.
		/// </summary>
		[DescriptionResource("StringValueEqualsCondition_RightValue_Description", typeof(Resources))]
		[DisplayNameResource("StringValueEqualsCondition_RightValue_DisplayName", typeof(Resources))]
		[Required]
		public string RightValue { get; set; }

		/// <summary>
		/// Evaluates the result of the comparison.
		/// </summary>
		/// <returns></returns>
		public override bool Evaluate()
		{
			tracer.TraceInformation(
				Resources.StringValueEqualsCondition_TraceInitial, this.LeftValue, this.RightValue, this.ComparisonKind);

			var result = string.Equals(this.LeftValue, this.RightValue, this.ComparisonKind);

			tracer.TraceInformation(
				Resources.StringValueEqualsCondition_TraceEvaluation, this.LeftValue, this.RightValue, this.ComparisonKind, result);

			return result;
		}
	}
}