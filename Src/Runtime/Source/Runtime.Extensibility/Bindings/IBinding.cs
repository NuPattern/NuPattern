using System.Collections.Generic;

namespace Microsoft.VisualStudio.TeamArchitect.PowerTools.Features
{
    /// <summary>
    /// Defines a binding.
    /// </summary>
    public interface IBinding<T> where T : class
    {
        /// <summary>
        /// Gets the evaluation results of the binding.
        /// </summary>
        IEnumerable<BindingResult> EvaluationResults { get; }

        /// <summary>
        /// Gets a value to indicate whether the binding has errors.
        /// </summary>
        bool HasErrors { get; }

        /// <summary>
        /// Gets or sets the user readable message for the binding.
        /// </summary>
        string UserMessage { get; set; }

        /// <summary>
        /// Gets the value of the binding.
        /// </summary>
        T Value { get; }

        /// <summary>
        /// Evaluates the binding.
        /// </summary>
        /// <returns></returns>
        bool Evaluate();
    }
}