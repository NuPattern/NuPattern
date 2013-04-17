using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NuPattern.Runtime.Design;
using NuPattern.Runtime.Properties;

namespace Microsoft.VisualStudio.TeamArchitect.PowerTools.Features
{
    /// <summary>
    /// Default validator for components, which uses 
    /// the DataAnnotations <see cref="ValidationAttribute"/>s 
    /// specified for properties to validate.
    /// </summary>
    internal class DataAnnotationsValidator : IValidator
    {
        /// <summary>
        /// Validates the object by evaluating the <see cref="ValidationAttribute"/>s 
        /// against the property values.
        /// </summary>
        public IEnumerable<ValidationResult> Validate(object component)
        {
            return from descriptor in TypeDescriptor.GetProperties(component).Cast<PropertyDescriptor>()
                   where descriptor.IsBindableProperty()
                   from validation in descriptor.Attributes.OfType<ValidationAttribute>()
                   where !validation.IsValid(descriptor.GetValue(component))
                   select new ValidationResult(
                       descriptor.Name,
                       validation.ErrorMessage ?? string.Format(Resources.Culture, Resources.DataAnnotationsValidator_ValidationFailed, validation.GetType().Name));
        }
    }
}