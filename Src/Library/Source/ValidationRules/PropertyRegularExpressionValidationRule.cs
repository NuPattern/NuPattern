using System.ComponentModel.DataAnnotations;
using System.Globalization;
using NuPattern.ComponentModel.Design;
using NuPattern.Library.Properties;

namespace NuPattern.Library.ValidationRules
{
    /// <summary>
    /// Specifies that a data field value in ASP.NET Dynamic Data must match the specified regular expression.
    /// </summary>
    [DescriptionResource("PropertyRegularExpressionValidationRule_Description", typeof(Resources))]
    [DisplayNameResource("PropertyRegularExpressionValidationRule_DisplayName", typeof(Resources))]
    public class PropertyRegularExpressionValidationRule : PropertyAttributeValidationRule
    {
        /// <summary>
        /// Gets or sets the regular expression pattern.
        /// </summary>
        [DescriptionResource("PropertyRegularExpressionValidationRule_PatternDescription", typeof(Resources))]
        [DisplayNameResource("PropertyRegularExpressionValidationRule_PatternDisplayName", typeof(Resources))]
        public string Pattern { get; set; }

        /// <summary>
        /// Creates the configured attribute validator.
        /// </summary>
        protected override ValidationAttribute CreateValidator()
        {
            return new RegularExpressionAttribute(this.Pattern);
        }

        /// <summary>
        /// Sets the default message if no custom message was specified.
        /// </summary>
        public override void EndInit()
        {
            base.EndInit();

            if (string.IsNullOrEmpty(this.ErrorMessage) && this.CurrentProperty != null)
                this.ErrorMessage = string.Format(CultureInfo.CurrentCulture,
                    Resources.Validate_PropertyRegularExpressionValidationRule_ErrorMessage,
                        this.CurrentProperty.Owner.InstanceName, this.Pattern, this.CurrentProperty.DefinitionName);
        }
    }
}