using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.VisualStudio.Patterning;
using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.Patterning.Runtime;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;

namespace $rootnamespace$
{
    /// <summary>
    /// A custom condition that determines a specific condition.
    /// </summary>
    [DisplayName("Element is $safeitemname$")]
    [Category("General")]
    [Description("Used to verify that the current element meets a custom condition.")]
    [CLSCompliant(false)]
    public class $safeitemname$ : Condition
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<$safeitemname$>();

        /// <summary>
        /// Gets or sets a configurable property for this condition.
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
        /// Evaluates this condition.
        /// </summary>
        /// <remarks></remarks>
        public override bool Evaluate()
        {
            // Verify all [Required] and [Import]ed properties have valid values.
			this.ValidateObject();

            // Make initial trace statement for this command
            tracer.TraceInformation(
				"Determining $safeitemname$ on current element '{0}' with AProperty '{1}'", this.CurrentElement.InstanceName, this.AProperty);

            // TODO: Implement condition automation code to determine the evaluated result
			var result = true;

			//	TODO: Use tracer.TraceWarning() to note expected and recoverable errors
			//	TODO: Use tracer.TraceVerbose() to note internal execution logic decisions
			//	TODO: Use tracer.TraceInformation() to note key results of execution
			//	TODO: Raise exceptions for all other errors

            // Make resulting trace statement for this condition
            tracer.TraceInformation(
				"Determined $safeitemname$ on current element '{0}' with AProperty '{1}', as '{2}'", this.CurrentElement.InstanceName, this.AProperty, result);

            return result;
        }
    }
}
