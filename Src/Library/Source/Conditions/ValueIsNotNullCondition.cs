using System.ComponentModel.DataAnnotations;
using NuPattern.ComponentModel.Design;
using NuPattern.Diagnostics;
using NuPattern.Library.Properties;
using NuPattern.Runtime;

namespace NuPattern.Library.Conditions
{
    /// <summary>
    /// Indicates whether a value is null.
    /// </summary>
    [DisplayNameResource(@"ValueIsNotNullCondition_DisplayName", typeof(Resources))]
    [DescriptionResource(@"ValueIsNotNullCondition_Description", typeof(Resources))]
    [CategoryResource(@"AutomationCategory_General", typeof(Resources))]
    public class ValueIsNotNullCondition : Condition
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<ValueIsNotNullCondition>();

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        [Required]
        [DisplayNameResource(@"ValueIsNotNullCondition_Value_DisplayName", typeof(Resources))]
        [DescriptionResource(@"ValueIsNotNullCondition_Value_Description", typeof(Resources))]
        public object Value { get; set; }

        /// <summary>
        /// Evaluates the condition
        /// </summary>
        public override bool Evaluate()
        {
            this.ValidateObject();

            tracer.TraceInformation(
                Resources.ValueIsNotNullCondition_TraceInitial, this.Value);

            var result = (Value != null);

            tracer.TraceInformation(
                Resources.ValueIsNotNullCondition_TraceEvaluation, this.Value, result);

            return result;
        }
    }
}
