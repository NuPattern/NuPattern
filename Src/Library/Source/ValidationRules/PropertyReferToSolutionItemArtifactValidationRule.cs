using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.Patterning.Library.Properties;
using Microsoft.VisualStudio.Patterning.Runtime;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;

namespace Microsoft.VisualStudio.Patterning.Library.ValidationRules
{
    /// <summary>
    /// Specifies that a data field value is required.
    /// </summary>
    [DescriptionResource("PropertyReferToSolutionItemArtifactValidationRule_Description", typeof(Resources))]
    [DisplayNameResource("PropertyReferToSolutionItemArtifactValidationRule_DisplayName", typeof(Resources))]
    [CLSCompliant(false)]
    public class PropertyReferToSolutionItemArtifactValidationRule : ValidationRule
    {
        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        [DescriptionResource("PropertyReferToSolutionItemArtifactValidationRule_ErrorMessageDescription", typeof(Resources))]
        [DisplayNameResource("PropertyReferToSolutionItemArtifactValidationRule_ErrorMessageDisplayName", typeof(Resources))]
        public virtual string ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the current property.
        /// </summary>
        [Required]
        [Import(AllowDefault = true)]
        public IProperty CurrentProperty { get; set; }


        /// <summary>
        /// Gets or sets the solution.
        /// </summary>
        [Required]
        [Import(AllowDefault = true)]
        public ISolution Solution { get; set; }

        /// <summary>
        /// Run the defined validation.
        /// </summary>
        /// <returns>
        /// The <see cref="ValidationResult"/> collection with the validation errors. If the validation was
        /// successful returns an empty enumerable or <see langword="null"/>.
        /// </returns>
        public override IEnumerable<ValidationResult> Validate()
        {
            this.ValidateObject();

            if (!string.IsNullOrEmpty(this.CurrentProperty.RawValue))
            {
                if (this.Solution != null)
                {
                    if (!this.Solution.Find(string.Concat(@"*\", this.CurrentProperty.RawValue)).Any())
                    {
                        yield return new ValidationResult(
                            string.IsNullOrEmpty(this.ErrorMessage) ? Resources.PropertyReferToSolutionItemArtifactValidationRule_DefaultErrorMessage : this.ErrorMessage);
                    }
                }
            }
        }
    }
}