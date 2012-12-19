using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NuPattern;
using NuPattern.Runtime;
using NuPattern.Extensibility;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;

namespace $rootnamespace$
{
    /// <summary>
    /// A custom value provider that provides a value to other automation classes.
    /// </summary>
    [DisplayName("$safeitemname$")]
    [Category("General")]
    [Description("Provides a custom value.")]
    [CLSCompliant(false)]
    public class $safeitemname$ : ValueProvider
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<$safeitemname$>();

        /// <summary>
        /// Gets or sets a configurable property for this value provider.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [DisplayName("A Custom Property")]
        [Description("A Description of this property.")]
        public string AProperty
        {
            get;
            set;
        }

        ///// <summary>
        ///// Gets or sets the solution.
        ///// </summary>
        //[Required]
        //[Import(AllowDefault = true)]
        //public ISolution Solution
        //{
        //    get;
        //    set;
        //}

        /// <summary>
        /// Gets or sets the current element.
        /// </summary>
        [Required]
        [Import(AllowDefault = true)]
        public IProductElement CurrentElement
        {
            get;
            set;
        }

        /// <summary>
        /// Returns the provided result.
        /// </summary>
        /// <remarks></remarks>
        public override object Evaluate()
        {
            // Verify all [Required] and [Import]ed properties have valid values.
			this.ValidateObject();

            // Make initial trace statement for this value provider
            tracer.TraceInformation(
				"Evaluating $safeitemname$ on current element '{0}' with AProperty '{1}'", this.CurrentElement.InstanceName, this.AProperty);

            // TODO: Implement provider automation code to determine the evaluated result
			var result = "Some value";

			//	TODO: Use tracer.TraceWarning() to note expected and recoverable errors
			//	TODO: Use tracer.TraceVerbose() to note internal execution logic decisions
			//	TODO: Use tracer.TraceInformation() to note key results of execution
			//	TODO: Raise exceptions for all other errors

            // Make resulting trace statement for this value provider
            tracer.TraceInformation(
				"Evaluated $safeitemname$ on current element '{0}' with AProperty '{1}', as '{2}'", this.CurrentElement.InstanceName, this.AProperty, result);

            return result;
        }
    }
}
