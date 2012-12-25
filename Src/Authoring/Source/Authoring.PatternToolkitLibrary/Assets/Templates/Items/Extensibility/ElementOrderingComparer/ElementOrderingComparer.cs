using System;
using System.ComponentModel;
using NuPattern.Runtime;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;

namespace $rootnamespace$
{
    /// <summary>
    /// A custom value comparer to determine the ordering of elements.
    /// </summary>
    [DisplayName("$safeitemname$")]
    [Category("General")]
    [Description("Compares elements for ordering.")]
    [CLSCompliant(false)]
    public class $safeitemname$ : ProductElementComparer
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<$safeitemname$>();

        /// <summary>
        /// Compares elements for ordering.
        /// </summary>
        /// <remarks></remarks>
        public override int Compare(IProductElement x, IProductElement y)
        {
            // Make initial trace statement for this comparer
            tracer.TraceInformation(
				"Comparer $safeitemname$ between element '{0}' and element '{1}'", x.InstanceName, y.InstanceName);

            // TODO: Implement comparison automation code to determine the result
			var result = String.Compare(x.InstanceName, y.InstanceName, true);

			//	TODO: Use tracer.TraceWarning() to note expected and recoverable errors
			//	TODO: Use tracer.TraceVerbose() to note internal execution logic decisions
			//	TODO: Use tracer.TraceInformation() to note key results of execution
			//	TODO: Raise exceptions for all other errors

            // Make resulting trace statement for this comparer
            tracer.TraceInformation(
				"Compared $safeitemname$ between element '{0}' and element '{1}', as '{2}'", x.InstanceName, y.InstanceName, result);

            return result;
        }
    }
}
