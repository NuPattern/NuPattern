using System.Collections.Generic;
using System.Linq;
using NuPattern.ComponentModel.Design;
using NuPattern.Diagnostics;
using NuPattern.Library.Properties;
using NuPattern.Runtime;

namespace NuPattern.Library.ValueProviders
{
    /// <summary>
    /// Returns the first or default value of an <see cref="IEnumerable{T}"/> 
    /// </summary>
    [DisplayNameResource(@"FirstOrDefaultValueProvider_DisplayName", typeof(Resources))]
    [DescriptionResource(@"FirstOrDefaultValueProvider_Description", typeof(Resources))]
    [CategoryResource(@"AutomationCategory_General", typeof(Resources))]
    public class FirstOrDefaultValueProvider : ValueProvider
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<FirstOrDefaultValueProvider>();

        /// <summary>
        /// Gets or sets the elements of the <see cref="IEnumerable{T}"/>
        /// </summary>
        [DisplayNameResource(@"FirstOrDefaultValueProvider_Elements_DisplayName", typeof(Resources))]
        [DescriptionResource(@"FirstOrDefaultValueProvider_Elements_Description", typeof(Resources))]
        public IEnumerable<object> Elements { get; set; }

        /// <summary>
        /// Evaluates the provider.
        /// </summary>
        public override object Evaluate()
        {
            this.ValidateObject();

            tracer.TraceInformation(
                Resources.FirstOrDefaultValueProvider_TraceInitial);

            if (this.Elements == null)
            {
                tracer.TraceError(
                    Resources.FirstOrDefaultValueProvider_TraceNoElements);

                return null;
            }

            var result = this.Elements.FirstOrDefault();

            tracer.TraceInformation(
                Resources.FirstOrDefaultValueProvider_TraceEvaluation, result);

            return result;
        }
    }
}
