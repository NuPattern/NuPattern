using System.Collections.Generic;

namespace Microsoft.VisualStudio.TeamArchitect.PowerTools.Features
{
    /// <summary>
    /// The result of a binding
    /// </summary>
    public class BindingResult
    {
        /// <summary>
        /// Creates a new instance of the <see cref="BindingResult"/> class.
        /// </summary>
        /// <param name="propertyName"></param>
        public BindingResult(string propertyName)
        {
            this.PropertyName = propertyName;
            this.Errors = new List<string>();
            this.InnerResults = new List<BindingResult>();
        }

        /// <summary>
        /// Gets the errors of the result.
        /// </summary>
        public ICollection<string> Errors { get; private set; }

        /// <summary>
        /// Gets the detailed results
        /// </summary>
        public ICollection<BindingResult> InnerResults { get; private set; }

        /// <summary>
        /// Get the name of the bound property
        /// </summary>
        public string PropertyName { get; private set; }

        /// <summary>
        /// Gets the value of the bound property.
        /// </summary>
        public object Value { get; internal set; }
    }
}