using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using Microsoft.VisualStudio.Shell;
using NuPattern.Extensibility;
using NuPattern.Library.Properties;
using NuPattern.Runtime;

namespace NuPattern.Library.ValidationRules
{
    /// <summary>
    /// Provides a base validation rule for validation attributes in data anotations.
    /// </summary>
    public abstract class PropertyAttributeValidationRule : ValidationRule, ISupportInitialize
    {
        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        [DescriptionResource("PropertyAttributeValidationRule_ErrorMessageDescription", typeof(Resources))]
        [DisplayNameResource("PropertyAttributeValidationRule_ErrorMessageDisplayName", typeof(Resources))]
        public virtual string ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the current property.
        /// </summary>
        [Required]
        [Import(AllowDefault = true)]
        public IProperty CurrentProperty { get; set; }

        /// <summary>
        /// Gets or sets the current service provider.
        /// </summary>
        [Required]
        [Import(typeof(SVsServiceProvider), AllowDefault = true)]
        public IServiceProvider ServiceProvider { get; set; }

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

            var validator = this.CreateValidator();
            if (!string.IsNullOrEmpty(this.ErrorMessage))
            {
                validator.ErrorMessage = this.ErrorMessage;
            }

            var result = validator.GetValidationResult(this.CurrentProperty.RawValue, new ValidationContext(this.CurrentProperty, this.ServiceProvider, null));
            if (result == ValidationResult.Success)
            {
                yield break;
            }

            yield return result;
        }

        /// <summary>
        /// Creates the configured attribute validator.
        /// </summary>
        protected abstract ValidationAttribute CreateValidator();


        /// <summary>
        /// Signals the object that initialization is starting, before all bound 
        /// properties and imports are set.
        /// </summary>
        public virtual void BeginInit()
        {
        }

        /// <summary>
        /// Signals the object that initialization is complete, after all 
        /// bound properties and imports are set.
        /// </summary>
        public virtual void EndInit()
        {
        }
    }
}