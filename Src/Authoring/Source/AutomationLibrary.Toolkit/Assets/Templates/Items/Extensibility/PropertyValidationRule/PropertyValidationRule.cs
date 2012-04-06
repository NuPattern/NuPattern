using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.Patterning.Runtime;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;

namespace $rootnamespace$
{
	/// <summary>
	/// A custom property validation rule that verifies attributes of a property.
	/// </summary>
	[DisplayName("$safeitemname$ is Valid")]
	[Category("General")]
	[Description("Validates that the current property has correct configuration.")]
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
        /// Gets or sets the current property to validate.
        /// </summary>
        [Required]
        [Import(AllowDefault = true)]
        public IProperty CurrentProperty
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
				"Validating $safeitemname$ on property '{0}' of element '{1}' with AProperty '{2}'", this.CurrentProperty.DefinitionName, this.CurrentProperty.Owner.InstanceName, this.AProperty);

            // TODO: Implement provider automation code to determine the violations
            errors.Add(new ValidationResult(
                string.Format(CultureInfo.CurrentCulture,
                "The value of property '{0}' for element '{1}' is not valid in some way. <Some prescriptive action to make it valid.>",
                this.CurrentProperty.DefinitionName, this.CurrentProperty.Owner.InstanceName)));

			//	TODO: Use tracer.TraceWarning() to note expected and recoverable errors
			//	TODO: Use tracer.TraceVerbose() to note internal execution logic decisions
			//	TODO: Use tracer.TraceInformation() to note key results of execution
			//	TODO: Raise exceptions for all other errors

			tracer.TraceInformation(
				"Validated $safeitemname$ on property '{0}' of element '{1}' with AProperty '{2}', as '{3}'", this.CurrentProperty.DefinitionName, this.CurrentProperty.Owner.InstanceName, this.AProperty, !errors.Any());

			return errors;
		}
	}
}
