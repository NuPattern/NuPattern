using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NuPattern.Properties;

namespace NuPattern.ComponentModel
{
    /// <summary>
    /// Provides validation of arbitrary objects, using 
    /// the DataAnnotations <see cref="ValidationAttribute"/>s 
    /// specified for properties to validate.
    /// </summary>
    public static class ObjectValidator
    {
        /// <summary>
        /// Validates the specified component.
        /// </summary>
        /// <param name="component">The component.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        [Obsolete(@"This method is obsolete, use System.ComponentModel.DataAnnotations.Validator.TryValidateProperty() method instead.", true)]
        public static IEnumerable<ValidationResult> Validate(object component, string propertyName)
        {
            Guard.NotNull(() => component, component);
            Guard.NotNull(() => propertyName, propertyName);

            return from descriptor in TypeDescriptor.GetProperties(component).Cast<PropertyDescriptor>()
                   where descriptor.Name.Equals(propertyName, StringComparison.Ordinal)
                   from validation in descriptor.Attributes.OfType<ValidationAttribute>()
                   where !validation.IsValid(descriptor.GetValue(component))
                   select new ValidationResult(GetErrorMessage(validation, descriptor.DisplayName), new[] { descriptor.Name });
        }

        /// <summary>
        /// Validates the object by evaluating the <see cref="ValidationAttribute"/>s 
        /// against the property values.
        /// </summary>
        [Obsolete(@"This method is obsolete, use System.ComponentModel.DataAnnotations.Validator.TryValidateObject() method instead.", true)]
        public static IEnumerable<ValidationResult> Validate(object component)
        {
            Guard.NotNull(() => component, component);

            return from descriptor in TypeDescriptor.GetProperties(component).Cast<PropertyDescriptor>()
                   from validation in descriptor.Attributes.OfType<ValidationAttribute>()
                   where !validation.IsValid(descriptor.GetValue(component))
                   select new ValidationResult(GetErrorMessage(validation, descriptor.DisplayName), new[] { descriptor.Name });
        }

        /// <summary>
        /// Throws <see cref="InvalidOperationException"/> if the <paramref name="component"/> 
        /// fails to validate.
        /// </summary>
        [Obsolete(@"This method is obsolete, use System.ComponentModel.DataAnnotations.Validator.ValidateObject() method instead.", true)]
        public static void ThrowIfInvalid(object component)
        {
            Guard.NotNull(() => component, component);

            var results = Validate(component);

            if (results.Any())
            {
                Func<ValidationResult, string> formatResult = result => string.Format(
                    Resources.Culture,
                    Resources.ObjectValidator_ValidationResult,
                    result.ErrorMessage,
                    string.Join(@", ", result.MemberNames));

                var message = results.Aggregate(
                    string.Format(Resources.Culture, Resources.ObjectValidator_ValidationException, component),
                    (errors, result) => errors + Environment.NewLine + formatResult(result));

                throw new InvalidOperationException(message);
            }
        }

        private static string GetErrorMessage(ValidationAttribute validation, string displayName)
        {
            return validation.FormatErrorMessage(displayName) ?? string.Format(Resources.Culture, Resources.ObjectValidator_ValidatorDefaultMessage, validation.GetType().Name);
        }
    }
}