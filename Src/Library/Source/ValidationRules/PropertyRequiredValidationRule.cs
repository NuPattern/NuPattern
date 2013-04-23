using System.ComponentModel.DataAnnotations;
using System.Globalization;
using NuPattern.ComponentModel.Design;
using NuPattern.Library.Properties;

namespace NuPattern.Library.ValidationRules
{
    /// <summary>
    /// Specifies that a data field value is required.
    /// </summary>
    [DescriptionResource(@"PropertyRequiredValidationRule_Description", typeof(Resources))]
    [DisplayNameResource(@"PropertyRequiredValidationRule_DisplayName", typeof(Resources))]
    [CategoryResource(@"AutomationCategory_General", typeof(Resources))]
    public class PropertyRequiredValidationRule : PropertyAttributeValidationRule
    {
        /// <summary>
        /// Creates the configured attribute validator.
        /// </summary>
        protected override ValidationAttribute CreateValidator()
        {
            return new RequiredAttribute
            {
                AllowEmptyStrings = false
            };
        }

        /// <summary>
        /// Sets the default message if no custom message was specified.
        /// </summary>
        public override void EndInit()
        {
            base.EndInit();

            if (string.IsNullOrEmpty(this.ErrorMessage) && this.CurrentProperty != null)
                this.ErrorMessage = string.Format(CultureInfo.CurrentCulture,
                    Resources.Validate_PropertyRequiredValidationRule_ErrorMessage, this.CurrentProperty.Owner.InstanceName, this.CurrentProperty.DefinitionName);
        }
    }
}