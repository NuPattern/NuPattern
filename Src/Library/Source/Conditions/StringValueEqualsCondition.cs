using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using NuPattern.ComponentModel.Design;
using NuPattern.Diagnostics;
using NuPattern.Library.Properties;
using NuPattern.Runtime;

namespace NuPattern.Library.Conditions
{
    /// <summary>
    /// A <see cref="Condition"/> that evaluates to true id the two strings compare with given comparison.
    /// </summary>
    [DisplayNameResource(@"StringValueEqualsCondition_DisplayName", typeof(Resources))]
    [DescriptionResource(@"StringValueEqualsCondition_Description", typeof(Resources))]
    [CategoryResource(@"AutomationCategory_General", typeof(Resources))]
    [CLSCompliant(false)]
    public class StringValueEqualsCondition : Condition
    {
        private static readonly ITracer tracer = Tracer.Get<StringValueEqualsCondition>();

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
        [Required]
        [DefaultValue(StringComparison.OrdinalIgnoreCase)]
        [DisplayNameResource(@"StringValueEqualsCondition_ComparisonKind_DisplayName", typeof(Resources))]
        [DescriptionResource(@"StringValueEqualsCondition_ComparisonKind_Description", typeof(Resources))]
        public StringComparison ComparisonKind { get; set; }

        /// <summary>
        /// Gets the left value to compare
        /// </summary>
        [Required]
        [DisplayNameResource(@"StringValueEqualsCondition_LeftValue_DisplayName", typeof(Resources))]
        [DescriptionResource(@"StringValueEqualsCondition_LeftValue_Description", typeof(Resources))]
        public string LeftValue { get; set; }

        /// <summary>
        /// Gets the right value to compare.
        /// </summary>
        [Required]
        [DisplayNameResource(@"StringValueEqualsCondition_RightValue_DisplayName", typeof(Resources))]
        [DescriptionResource(@"StringValueEqualsCondition_RightValue_Description", typeof(Resources))]
        public string RightValue { get; set; }

        /// <summary>
        /// Evaluates the result of the comparison.
        /// </summary>
        /// <returns></returns>
        public override bool Evaluate()
        {
            this.ValidateObject();

            tracer.Info(
                Resources.StringValueEqualsCondition_TraceInitial, this.LeftValue, this.RightValue, this.ComparisonKind);

            var result = string.Equals(this.LeftValue, this.RightValue, this.ComparisonKind);

            tracer.Info(
                Resources.StringValueEqualsCondition_TraceEvaluation, this.LeftValue, this.RightValue, this.ComparisonKind, result);

            return result;
        }
    }
}