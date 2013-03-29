using System;
using System.Linq;

namespace NuPattern.Runtime.Schema
{
    /// <summary>
    /// Diagram extension methods.
    /// </summary>
    internal static class DiagramExtensions
    {
        /// <summary>
        /// Determines whether [is view represented] [the specified diagram].
        /// </summary>
        /// <param name="diagram">The diagram.</param>
        /// <param name="view">The view to verify.</param>
        /// <returns>
        /// 	<c>true</c> if [is view represented] [the specified diagram]; otherwise, <c>false</c>.
        /// </returns>
        internal static bool IsViewRepresented(this PatternModelSchemaDiagram diagram, ViewSchema view)
        {
            return diagram.Id.ToString().Equals(view.DiagramId, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Gets the represented view.
        /// </summary>
        /// <param name="diagram">The diagram.</param>
        /// <returns>The view represented by the diagram.</returns>
        internal static ViewSchema GetRepresentedView(this PatternModelSchemaDiagram diagram)
        {
            return diagram.Store.GetViews().Single(view => diagram.IsViewRepresented(view));
        }
    }
}