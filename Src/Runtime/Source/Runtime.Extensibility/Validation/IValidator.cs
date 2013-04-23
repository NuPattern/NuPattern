using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NuPattern.Runtime.Bindings;

namespace NuPattern.Runtime.Validation
{
    /// <summary>
    /// Validator interface used by <see cref="Binding{T}"/> to 
    /// determine if a bound component is in a valid state.
    /// </summary>
    public interface IValidator
    {
        /// <summary>
        /// Validates the object by evaluating the <see cref="ValidationAttribute"/>s 
        /// against the property values.
        /// </summary>
        IEnumerable<PropertyValidationResult> Validate(object component);
    }
}