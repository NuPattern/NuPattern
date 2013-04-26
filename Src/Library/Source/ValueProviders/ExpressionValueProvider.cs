using System;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using NuPattern.ComponentModel.Design;
using NuPattern.Diagnostics;
using NuPattern.Library.Properties;
using NuPattern.Runtime;

namespace NuPattern.Library.ValueProviders
{
    /// <summary>
    /// A custom value provider that is used to provide values at runtime for other types of configured automation.
    /// </summary>
    [DisplayNameResource(@"ExpressionValueProvider_DisplayName", typeof(Resources))]
    [DescriptionResource(@"ExpressionValueProvider_Description", typeof(Resources))]
    [CategoryResource(@"AutomationCategory_General", typeof(Resources))]
    [CLSCompliant(false)]
    public class ExpressionValueProvider : ValueProvider
    {
        private static readonly ITracer tracer = Tracer.Get<ExpressionValueProvider>();

        /// <summary>
        /// Gets the current element in the pattern model upon which this ValueProvider is configured.
        /// </summary>
        [Import(AllowDefault = true)]
        public IInstanceBase CurrentElement
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the expression to evaluate.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [DisplayNameResource(@"ExpressionValueProvider_Expression_DisplayName", typeof(Resources))]
        [DescriptionResource(@"ExpressionValueProvider_Expression_Description", typeof(Resources))]
        public string Expression
        {
            get;
            set;
        }

        /// <summary>
        /// Returns the result of evaluation of this provider.
        /// </summary>
        public override object Evaluate()
        {
            this.ValidateObject();

            var productElement = this.CurrentElement as IProductElement;
            var elementName = (productElement != null) ? productElement.InstanceName : this.CurrentElement.Info.DisplayName;

            tracer.Info(
                Resources.ExpressionValueProvider_TraceInitial, this.Expression, elementName);

            var result = ExpressionEvaluator.Evaluate(this.CurrentElement, this.Expression);

            if (result == null)
            {
                tracer.Warn(
                    Resources.ExpressionValueProvider_TraceResolvedNullExpression, this.Expression, elementName);
            }

            tracer.Info(
                Resources.ExpressionValueProvider_TraceEvaluation, this.Expression, elementName, result);

            return result;
        }
    }
}
