using Microsoft.VisualStudio.Modeling;

namespace NuPattern.Runtime.Schema
{
    /// <summary>
    /// Marks model elements that can be intercepted.
    /// </summary>
    internal interface IInterceptorTarget
    {
        /// <summary>
        /// Gets the model element.
        /// </summary>
        /// <returns>The model element.</returns>
        ModelElement Element { get; }
    }
}