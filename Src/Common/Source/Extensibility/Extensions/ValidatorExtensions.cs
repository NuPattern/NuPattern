using System.ComponentModel.DataAnnotations;

namespace Microsoft.VisualStudio.Patterning.Extensibility
{
    /// <summary>
    /// Extensions to the <see cref="Validator"/> class.
    /// </summary>
    public static class ValidatorExtensions
    {
        /// <summary>
        /// Determines whether the specified object is valid using the validation context.
        /// </summary>
        /// <remarks>
        /// See <see cref="Validator.ValidateObject(object,System.ComponentModel.DataAnnotations.ValidationContext)"/>
        /// </remarks>
        /// <param name="instance">The object to validate.</param>
        public static void ValidateObject(this object instance)
        {
            Validator.ValidateObject(instance, new ValidationContext(instance, null, null), true);
        }
    }
}
