using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using NuPattern.Extensibility;
using NuPattern.Library.Properties;
using NuPattern.Runtime;

namespace NuPattern.Library.ValidationRules
{
    /// <summary>
    /// Specifies the forbidden characters that cannot exist in a data field.
    /// </summary>
    [DescriptionResource("PropertyStringValueForbiddenCharsValidationRule_Description", typeof(Resources))]
    [DisplayNameResource("PropertyStringValueForbiddenCharsValidationRule_DisplayName", typeof(Resources))]
    public class PropertyStringValueForbiddenCharsValidationRule : ValidationRule
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<PropertyStringValueForbiddenCharsValidationRule>();

        /// <summary>
        /// Gets or sets the characters that are forbidden.
        /// </summary>
        [DescriptionResource("PropertyStringValueForbiddenCharsValidationRule_ForbiddenChars_Description", typeof(Resources))]
        [DisplayNameResource("PropertyStringValueForbiddenCharsValidationRule_ForbiddenChars_DisplayName", typeof(Resources))]
        public string ForbiddenChars
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the current property.
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

            this.ValidateObject();

            tracer.TraceInformation(
                Resources.PropertyStringValueForbiddenCharsValidationRule_TraceInitial, this.CurrentProperty.DefinitionName, this.ForbiddenChars);

            if (this.CurrentProperty.Info.Type.Equals(typeof(string).FullName, StringComparison.OrdinalIgnoreCase))
            {
                var propertyValue = this.CurrentProperty.Value.ToString();
                if (!string.IsNullOrEmpty(propertyValue)
                    && !string.IsNullOrEmpty(this.ForbiddenChars))
                {
                    if (propertyValue.IndexOfAny(this.ForbiddenChars.ToCharArray()) != -1)
                    {
                        errors.Add(new ValidationResult(
                            string.Format(CultureInfo.CurrentCulture,
                            Resources.PropertyStringValueForbiddenCharsValidationRule_ContainsForbiddenChars,
                            this.CurrentProperty.DefinitionName, this.CurrentProperty.Owner.InstanceName, this.ForbiddenChars)));
                    }
                }
            }

            tracer.TraceInformation(
                Resources.PropertyStringValueForbiddenCharsValidationRule_TraceEvaluation, this.CurrentProperty.DefinitionName, this.ForbiddenChars, !errors.Any());

            return errors;
        }
    }
}
