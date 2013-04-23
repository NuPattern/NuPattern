
namespace NuPattern.Runtime.Validation
{
    /// <summary>
    /// A result of a property validation
    /// </summary>
    public class PropertyValidationResult
    {
        /// <summary>
        /// Creates a new instance of the <see cref="PropertyValidationResult"/> class.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="errorMessage"></param>
        public PropertyValidationResult(string propertyName, string errorMessage)
        {
            this.PropertyName = propertyName;
            this.ErrorMessage = errorMessage;
        }

        /// <summary>
        /// Gets the name of the proeprty to evaluate
        /// </summary>
        public string PropertyName { get; private set; }

        /// <summary>
        /// Gets the validation error message
        /// </summary>
        public string ErrorMessage { get; private set; }
    }
}