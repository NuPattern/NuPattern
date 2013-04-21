using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NuPattern.ComponentModel.Design;
using NuPattern.Diagnostics;
using NuPattern.Library.Properties;
using NuPattern.Runtime;
using NuPattern.VisualStudio.Solution;

namespace NuPattern.Library.ValueProviders
{
    /// <summary>
    /// Returns the first item in the solution that matches the path expression. 
    /// </summary>
    [DisplayNameResource("FirstSolutionElementMatchValueProvider_DisplayName", typeof(Resources))]
    [DescriptionResource("FirstSolutionElementMatchValueProvider_Description", typeof(Resources))]
    [CategoryResource("AutomationCategory_VisualStudio", typeof(Resources))]
    public class FirstSolutionElementMatchValueProvider : ValueProvider
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<FirstSolutionElementMatchValueProvider>();

        /// <summary>
        /// Gets the solution
        /// </summary>
        [Import]
        public ISolution Solution { get; set; }

        /// <summary>
        /// Gets or sets the path expression.
        /// </summary>
        [Required]
        [DisplayNameResource("FirstSolutionElementMatchValueProvider_PathExpression_DisplayName", typeof(Resources))]
        [DescriptionResource("FirstSolutionElementMatchValueProvider_PathExpression_Description", typeof(Resources))]
        public string PathExpression { get; set; }

        /// <summary>
        /// Evaluates the provider.
        /// </summary>
        public override object Evaluate()
        {
            this.ValidateObject();

            tracer.TraceInformation(
                Resources.FirstSolutionElementMatchValueProvider_TraceInitial, this.PathExpression);

            var result = Solution.Find(PathExpression).FirstOrDefault();

            tracer.TraceInformation(
                Resources.FirstSolutionElementMatchValueProvider_TraceEvaluation, this.PathExpression, result.GetLogicalPath());

            return result;
        }
    }
}
