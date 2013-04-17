
namespace Microsoft.VisualStudio.TeamArchitect.PowerTools.Features
{
    /// <summary>
    /// A result of a validation
    /// </summary>
    public class ValidationResult
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ValidationResult"/> class.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="errorMessage"></param>
        public ValidationResult(string propertyName, string errorMessage)
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