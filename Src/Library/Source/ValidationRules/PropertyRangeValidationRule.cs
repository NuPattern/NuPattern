using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Drawing.Design;
using System.Globalization;
using NuPattern.ComponentModel.Design;
using NuPattern.Library.Properties;
using NuPattern.Runtime.Design;

namespace NuPattern.Library.ValidationRules
{
    /// <summary>
    /// Specifies the numeric range constraints for the value of a data field.
    /// </summary>
    [DescriptionResource(@"PropertyRangeValidationRule_Description", typeof(Resources))]
    [DisplayNameResource(@"PropertyRangeValidationRule_DisplayName", typeof(Resources))]
    [CategoryResource(@"AutomationCategory_General", typeof(Resources))]
    public class PropertyRangeValidationRule : PropertyAttributeValidationRule
    {
        /// <summary>
        /// Gets or sets the maximum allowed field value.
        /// </summary>
        [DescriptionResource(@"PropertyRangeValidationRule_MaximumDescription", typeof(Resources))]
        [DisplayNameResource(@"PropertyRangeValidationRule_MaximumDisplayName", typeof(Resources))]
        public string Maximum { get; set; }

        /// <summary>
        /// Gets or sets the minimum allowed field value.
        /// </summary>
        [DescriptionResource(@"PropertyRangeValidationRule_MinimumDescription", typeof(Resources))]
        [DisplayNameResource(@"PropertyRangeValidationRule_MinimumDisplayName", typeof(Resources))]
        public string Minimum { get; set; }

        /// <summary>
        /// Gets or sets the type of the data field whose value must be validated.
        /// </summary>
        [DescriptionResource(@"PropertyRangeValidationRule_OperandTypeDescription", typeof(Resources))]
        [DisplayNameResource(@"PropertyRangeValidationRule_OperandTypeDisplayName", typeof(Resources))]
        [Editor(typeof(StandardValuesEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(FullTypeTypeConverter<IComparable>))]
        public Type OperandType { get; set; }

        /// <summary>
        /// Creates the configured attribute validator.
        /// </summary>
        protected override ValidationAttribute CreateValidator()
        {
            return new RangeAttribute(this.OperandType, this.Minimum, this.Maximum);
        }

        /// <summary>
        /// Sets the default message if no custom message was specified.
        /// </summary>
        public override void EndInit()
        {
            base.EndInit();

            if (string.IsNullOrEmpty(this.ErrorMessage) && this.CurrentProperty != null)
                this.ErrorMessage = string.Format(CultureInfo.CurrentCulture,
                    Resources.Validate_PropertyRangeValidationRule_ErrorMessage,
                        this.CurrentProperty.Owner.InstanceName, this.Minimum, this.Maximum, this.CurrentProperty.DefinitionName);
        }
    }
}