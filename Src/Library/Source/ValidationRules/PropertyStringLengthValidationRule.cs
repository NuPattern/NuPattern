using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using NuPattern.ComponentModel.Design;
using NuPattern.Library.Properties;

namespace NuPattern.Library.ValidationRules
{
    /// <summary>
    /// Specifies the minimum and maximum length of characters that are allowed in a data field.
    /// </summary>
    [DescriptionResource("PropertyStringLengthValidationRule_Description", typeof(Resources))]
    [DisplayNameResource("PropertyStringLengthValidationRule_DisplayName", typeof(Resources))]
    public class PropertyStringLengthValidationRule : PropertyAttributeValidationRule
    {
        /// <summary>
        /// Gets or sets the maximum length of a string.
        /// </summary>
        /// <value>The maximum length a string.</value>
        [Range(1, int.MaxValue)]
        [DescriptionResource("PropertyStringLengthValidationRule_MaximumLengthDescription", typeof(Resources))]
        [DisplayNameResource("PropertyStringLengthValidationRule_MaximumLengthDisplayName", typeof(Resources))]
        public int MaximumLength { get; set; }

        /// <summary>
        /// Gets or sets the minimum length of a string.
        /// </summary>
        /// <value>The minimum length of a string.</value>
        [Range(0, int.MaxValue)]
        [DescriptionResource("PropertyStringLengthValidationRule_MinimumLengthDescription", typeof(Resources))]
        [DisplayNameResource("PropertyStringLengthValidationRule_MinimumLengthDisplayName", typeof(Resources))]
        public int MinimumLength { get; set; }

        /// <summary>
        /// Creates the configured attribute validator.
        /// </summary>
        protected override ValidationAttribute CreateValidator()
        {
            if (this.MinimumLength > this.MaximumLength)
            {
                throw new InvalidOperationException(Resources.PropertyStringLengthValidationRule_SelectedRangeInvalid);
            }

            return new StringLengthAttribute(this.MaximumLength) { MinimumLength = this.MinimumLength };
        }

        /// <summary>
        /// Sets the default message if no custom message was specified.
        /// </summary>
        public override void EndInit()
        {
            base.EndInit();

            if (string.IsNullOrEmpty(this.ErrorMessage) && this.CurrentProperty != null)
                this.ErrorMessage = string.Format(CultureInfo.CurrentCulture,
                    Resources.Validate_PropertyStringLengthValidationRule_ErrorMessage,
                        this.CurrentProperty.Owner.InstanceName, this.MinimumLength, this.MaximumLength, this.CurrentProperty.DefinitionName);
        }
    }
}