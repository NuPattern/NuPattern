using System;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.Patterning.Library.Properties;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;

namespace Microsoft.VisualStudio.Patterning.Library.ValueProviders
{
    /// <summary>
    /// A <see cref=" ValueProvider"/> that provides solution name.
    /// </summary>
    [DisplayNameResource("SolutionNameValueProvider_DisplayName", typeof(Resources))]
    [DescriptionResource("SolutionNameValueProvider_Description", typeof(Resources))]
    [CategoryResource("AutomationCategory_Automation", typeof(Resources))]
    [CLSCompliant(false)]
    public class SolutionNameValueProvider : ValueProvider
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<SolutionNameValueProvider>();

        /// <summary>
        /// The current solution.
        /// </summary>
        [Required]
        [Import(AllowDefault = true)]
        public ISolution Solution
        {
            get;
            set;
        }

        /// <summary>
        /// Evaluates this provider.
        /// </summary>
        public override object Evaluate()
        {
            this.ValidateObject();

            tracer.TraceInformation(
                Resources.SolutionNameValueProvider_TraceInitial);

            var result = this.Solution.Name;

            tracer.TraceInformation(
                Resources.SolutionNameValueProvider_TraceEvaluation, result);

            return result;
        }
    }
}