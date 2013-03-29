using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using NuPattern;
using NuPattern.Runtime;
using NuPattern.Runtime.Validation;

namespace $rootnamespace$
{
    /// <summary>
    /// A custom element validation rule that verifies instances of elements.
    /// </summary>
    [DisplayName("$safeitemname$ is Valid")]
    [Category("General")]
    [Description("Validates that the current element has correct configuration.")]
    [CLSCompliant(false)]
    public class $safeitemname$ : ValidationRule
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<$safeitemname$>();

        /// <summary>
        /// Gets or sets a configurable property for this rule.
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
        /// Gets or sets the current element to validate.
        /// </summary>
        [Required]
        [Import(AllowDefault = true)]
        public IProductElement CurrentElement
        {
            get;
            set;
        }

        /// <summary>
        /// Evaluates the violations for the rule.
        /// </summary>
        /// <remarks></remarks>
        public override IEnumerable<ValidationResult> Validate()
        {
            List<ValidationResult> errors = new List<ValidationResult>();

            // Verify all [Required] and [Import]ed properties have valid values.
            this.ValidateObject();

            // Make initial trace statement for this rule
            tracer.TraceInformation(
                "Validating $safeitemname$ on current element '{0}' with AProperty '{1}'", this.CurrentElement.InstanceName, this.AProperty);

            // TODO: Implement provider automation code to determine the violations
            errors.Add(new ValidationResult(
                string.Format(CultureInfo.CurrentCulture,
                "The element '{0}' is not valid in some way. <Some prescriptive action to make it valid.>",
                this.CurrentElement.InstanceName)));

            //	TODO: Use tracer.TraceWarning() to note expected and recoverable errors
            //	TODO: Use tracer.TraceVerbose() to note internal execution logic decisions
            //	TODO: Use tracer.TraceInformation() to note key results of execution
            //	TODO: Raise exceptions for all other errors

            tracer.TraceInformation(
                "Validated $safeitemname$ on current element '{0}' with AProperty '{1}', as '{2}'", this.CurrentElement.InstanceName, this.AProperty, !errors.Any());

            return errors;
        }
    }
}
