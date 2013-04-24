using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NuPattern;
using NuPattern.Diagnostics;
using NuPattern.Runtime;

namespace $rootnamespace$
{
    /// <summary>
    /// A custom value provider that provides a value to other automation classes.
    /// </summary>
    [DisplayName("$safeitemname$")]
    [Category("General")]
    [Description("Provides a custom value.")]
    [CLSCompliant(false)]
    public class $safeitemname$ : NuPattern.Runtime.ValueProvider
    {
        private static readonly ITracer tracer = Tracer.Get<$safeitemname$>();

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
            tracer.Info(
                "Evaluating $safeitemname$ on current element '{0}' with AProperty '{1}'", this.CurrentElement.InstanceName, this.AProperty);

            // TODO: Implement provider automation code to determine the evaluated result
            var result = "Some value";

            // TODO: Use tracer.Warn() to note expected and recoverable errors
            // TODO: Use tracer.Verbose() to note internal execution logic decisions
            // TODO: Use tracer.Info() to note key results of execution
            // TODO: Raise exceptions for all other errors

            // Make resulting trace statement for this value provider
            tracer.Info(
                "Evaluated $safeitemname$ on current element '{0}' with AProperty '{1}', as '{2}'", this.CurrentElement.InstanceName, this.AProperty, result);

            return result;
        }
    }
}
