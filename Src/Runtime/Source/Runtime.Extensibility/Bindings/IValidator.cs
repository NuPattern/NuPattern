using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Microsoft.VisualStudio.TeamArchitect.PowerTools.Features
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
        IEnumerable<ValidationResult> Validate(object component);
    }
}