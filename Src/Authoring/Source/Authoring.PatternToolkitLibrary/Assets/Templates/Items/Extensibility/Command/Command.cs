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
    /// A custom command that performs some automation.
    /// </summary>
    [DisplayName("$safeitemname$")]
    [Category("General")]
    [Description("Performs some custom automation.")]
    [CLSCompliant(false)]
    public class $safeitemname$ : NuPattern.Runtime.Command
    {
        private static readonly ITracer tracer = Tracer.Get<$safeitemname$>();

        /// <summary>
        /// Gets or sets a configurable property for this command.
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
        /// Executes this commmand.
        /// </summary>
        /// <remarks></remarks>
        public override void Execute()
        {
            // Verify all [Required] and [Import]ed properties have valid values.
            this.ValidateObject();

            // Make initial trace statement for this command
            tracer.Info(
                "Executing $safeitemname$ on current element '{0}' with AProperty '{1}'", this.CurrentElement.InstanceName, this.AProperty);

            // TODO: Implement command automation code
            // TODO: Use tracer.Warn() to note expected and recoverable errors
            // TODO: Use tracer.Verbose() to note internal execution logic decisions
            // TODO: Use tracer.Info() to note key results of execution
            // TODO: Raise exceptions for all other errors
        }
    }
}
