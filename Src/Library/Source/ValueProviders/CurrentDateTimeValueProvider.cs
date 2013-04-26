using System;
using NuPattern.ComponentModel.Design;
using NuPattern.Diagnostics;
using NuPattern.Library.Properties;
using NuPattern.Runtime;

namespace NuPattern.Library.ValueProviders
{
    /// <summary>
    /// A <see cref=" ValueProvider"/> that returns the current date and time.
    /// </summary>
    [DisplayNameResource(@"CurrentDateTimeValueProvider_DisplayName", typeof(Resources))]
    [DescriptionResource(@"CurrentDateTimeValueProvider_Description", typeof(Resources))]
    [CategoryResource(@"AutomationCategory_General", typeof(Resources))]
    [CLSCompliant(false)]
    public class CurrentDateTimeValueProvider : ValueProvider
    {
        private static readonly ITracer tracer = Tracer.Get<CurrentDateTimeValueProvider>();

        /// <summary>
        /// Evaluates this provider.
        /// </summary>
        public override object Evaluate()
        {
            this.ValidateObject();

            tracer.Info(
                Resources.CurrentDateTimeValueProvider_TraceInitial);

            var result = DateTime.Now;

            tracer.Info(
                Resources.CurrentDateTimeValueProvider_TraceEvaluation, result);

            return result;
        }
    }
}
