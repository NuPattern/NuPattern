using System.Collections.Generic;

namespace Microsoft.VisualStudio.Patterning.Runtime
{
    /// <summary>
    /// Provides a container for automation extensions.
    /// </summary>
    public partial interface IProductElement
    {
        /// <summary>
        /// Gets the owning pattern for this element instance.
        /// For an extension pattern this is the owning pattern.
        /// For a non-parented pattern, or elements in a non-parent pattern, this is the root pattern (<see cref="IInstanceBase.Root"/>/>.
        /// </summary>
        [Hidden]
        IProduct Product { get; }

        /// <summary>
        /// Gets the automation extensions.
        /// </summary>
        [Hidden]
        IEnumerable<IAutomationExtension> AutomationExtensions { get; }

        /// <summary>
        /// Creates a new automation extension
        /// </summary>
        void AddAutomationExtension(IAutomationExtension extension);
    }
}
