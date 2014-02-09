using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using NuPattern.ComponentModel.Design;
using NuPattern.Diagnostics;
using NuPattern.Library.Properties;
using NuPattern.Runtime;

namespace NuPattern.Library.Conditions
{
    /// <summary>
    /// Indicates whether the string value is null or empty
    /// </summary>
    [DisplayNameResource(@"StringNotNullOrEmptyCondition_DisplayName", typeof(Resources))]
    [DescriptionResource(@"StringNotNullOrEmptyCondition_Description", typeof(Resources))]
    [CategoryResource(@"AutomationCategory_General", typeof(Resources))]
    public class StringNotNullOrEmptyCondition : Condition
    {
        private static readonly ITracer tracer = Tracer.Get<StringNotNullOrEmptyCondition>();

        /// <summary>
        /// Gets the current element.
        /// </summary>
        [Required]
        [Import(AllowDefault = true)]
        public IProductElement CurrentElement { get; set; }

        /// <summary>
        /// Gets or sets the value
        /// </summary>
        [Required]
        [DisplayNameResource(@"StringNotNullOrEmptyCondition_Value_DisplayName", typeof(Resources))]
        [DescriptionResource(@"StringNotNullOrEmptyCondition_Value_Description", typeof(Resources))]
        public string Value { get; set; }

        /// <summary>
        /// Evaluates the condition
        /// </summary>
        public override bool Evaluate()
        {
            this.ValidateObject();

            tracer.Info(
                Resources.StringNotNullOrEmptyCondition_TraceInitial, this.Value);

            var value = (!string.IsNullOrEmpty(this.Value) && this.CurrentElement != null) 
                ? ExpressionEvaluator.Evaluate(this.CurrentElement, this.Value) : this.Value;

            var result = string.IsNullOrEmpty(value);

            tracer.Info(
                Resources.StringNotNullOrEmptyCondition_TraceEvaluation, this.Value, result);

            return result;
        }
    }
}
