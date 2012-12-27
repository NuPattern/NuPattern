using System;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using NuPattern.Extensibility;
using NuPattern.Library.Properties;

namespace NuPattern.Library.ValueProviders
{
    /// <summary>
    /// A <see cref=" ValueProvider"/> that returns the current date and time.
    /// </summary>
    [DisplayNameResource("CurrentDateTimeValueProvider_DisplayName", typeof(Resources))]
    [DescriptionResource("CurrentDateTimeValueProvider_Description", typeof(Resources))]
    [CategoryResource("AutomationCategory_General", typeof(Resources))]
    [CLSCompliant(false)]
    public class CurrentDateTimeValueProvider : ValueProvider
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<CurrentDateTimeValueProvider>();

        /// <summary>
        /// Evaluates this provider.
        /// </summary>
        public override object Evaluate()
        {
            this.ValidateObject();

            tracer.TraceInformation(
                Resources.CurrentDateTimeValueProvider_TraceInitial);

            var result = DateTime.Now;

            tracer.TraceInformation(
                Resources.CurrentDateTimeValueProvider_TraceEvaluation, result);

            return result;
        }
    }
}
