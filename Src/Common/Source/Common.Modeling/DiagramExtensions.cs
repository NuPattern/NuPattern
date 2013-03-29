using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Modeling.Diagrams;

namespace NuPattern.Modeling
{
    /// <summary>
    /// Diagram extension methods.
    /// </summary>
    public static class DiagramExtensions
    {
        /// <summary>
        /// Gets the shapes on the diagram.
        /// </summary>
        /// <param name="diagram">The diagram.</param>
        public static IEnumerable<TShape> GetShapes<TShape>(this Diagram diagram) where TShape : ShapeElement
        {
            return diagram.Store.ElementDirectory.AllElements.OfType<TShape>()
                .Where(s => s.Diagram.Id.Equals(diagram.Id));
        }
    }
}