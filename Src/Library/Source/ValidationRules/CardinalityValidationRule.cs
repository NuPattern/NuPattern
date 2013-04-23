using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using NuPattern.ComponentModel.Design;
using NuPattern.Library.Properties;
using NuPattern.Runtime;
using NuPattern.Runtime.Validation;

namespace NuPattern.Library.ValidationRules
{
    /// <summary>
    /// Specifies the numeric range constraints for the cardinality.
    /// </summary>
    [DescriptionResource(@"CardinalityValidationRule_Description", typeof(Resources))]
    [DisplayNameResource(@"CardinalityValidationRule_DisplayName", typeof(Resources))]
    [CategoryResource(@"AutomationCategory_Automation", typeof(Resources))]
    public class CardinalityValidationRule : ValidationRule
    {
        /// <summary>
        /// Gets or sets the current element.
        /// </summary>
        [Required]
        [Import(AllowDefault = true)]
        public IProductElement CurrentElement { get; set; }

        /// <summary>
        /// The name of the child element to check cardinality on.
        /// </summary>
        [Required]
        [DescriptionResource(@"CardinalityValidationRule_ChildElementNameDescription", typeof(Resources))]
        [DisplayNameResource(@"CardinalityValidationRule_ChildElementNameDisplayName", typeof(Resources))]
        public string ChildElementName { get; set; }

        /// <summary>
        /// Gets or sets the maximum allowed field value.
        /// </summary>
        [Required]
        [DescriptionResource(@"CardinalityValidationRule_MaximumDescription", typeof(Resources))]
        [DisplayNameResource(@"CardinalityValidationRule_MaximumDisplayName", typeof(Resources))]
        [DefaultValue(Int32.MaxValue)]
        [Range(1, Int32.MaxValue)]
        public int Maximum { get; set; }

        /// <summary>
        /// Gets or sets the minimum allowed field value.
        /// </summary>
        [Required]
        [DescriptionResource(@"CardinalityValidationRule_MinimumDescription", typeof(Resources))]
        [DisplayNameResource(@"CardinalityValidationRule_MinimumDisplayName", typeof(Resources))]
        [DefaultValue(Int32.MinValue)]
        [Range(0, Int32.MaxValue)]
        public int Minimum { get; set; }

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

            if (Minimum > Maximum)
            {
                throw new InvalidOperationException(Properties.Resources.CardinalityValidationRule_RangeException);
            }

            if (this.CurrentElement != null)
            {
                var childElementInfos = this.CurrentElement.Info.FindAllChildren().Where(c => c.Name.Equals(ChildElementName, StringComparison.OrdinalIgnoreCase));
                if (!(childElementInfos.Any()))
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                        Properties.Resources.CardinalityValidationRule_ChildElementInfoNotFound, this.CurrentElement.InstanceName, ChildElementName));
                }

                var childElementInfo = childElementInfos.FirstOrDefault();

                if (childElementInfo != null)
                {
                    var count = this.CurrentElement.GetChildren().Where(ele => ele.Info == childElementInfo).Count();

                    if (count < Minimum || count > Maximum)
                    {
                        yield return new ValidationResult(string.Format(CultureInfo.CurrentCulture,
                            Properties.Resources.CardinalityValidationRule_ErrorMessage, this.CurrentElement.InstanceName, childElementInfo.DisplayName, this.Minimum, this.Maximum));
                    }
                }
            }
        }
    }
}