using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Diagrams;
using NuPattern.Modeling;

namespace NuPattern.Runtime.Schema
{
    /// <summary>
    /// Extensions to the <see cref="ModelElement"/> class.
    /// </summary>
    internal static class ModelElementExtensions
    {
        /// <summary>
        /// Gets the (first) shape representing the given element, in the current view
        /// </summary>
        public static NodeShape GetShape(this ModelElement element)
        {
            Guard.NotNull(() => element, element);

            return element.GetShape(element.Store.GetCurrentDiagram());
        }

        /// <summary>
        /// Gets the (first) shape representing the given element in the current view.
        /// </summary>
        public static T GetShape<T>(this ModelElement element) where T : NodeShape
        {
            return element.GetShape() as T;
        }
    }
}
