using System;
using System.Linq;
using Microsoft.VisualStudio.Modeling.Diagrams;

namespace NuPattern.Runtime.Schema
{
    /// <summary>
    /// Extension methods over <see cref="CompartmentShape"/>
    /// </summary>
    internal static class CompartmentShapeExtension
    {
        /// <summary>
        /// Finds the diagram item.
        /// </summary>
        /// <typeparam name="TModelElement">The type of the model element.</typeparam>
        /// <param name="shape">The shape.</param>
        /// <param name="elementSelector">The element selector.</param>
        public static DiagramItem FindDiagramItem<TModelElement>(this CompartmentShape shape, Func<TModelElement, bool> elementSelector)
        {
            Guard.NotNull(() => shape, shape);

            var compartments = shape.NestedChildShapes.OfType<ElementListCompartment>().ToList();

            foreach (var compartment in compartments)
            {
                for (var diagramItem = compartment.ListField.FindFirstChild(compartment, false);
                     diagramItem != null;
                     diagramItem = compartment.ListField.FindNextChild(diagramItem, false))
                {
                    if (diagramItem.RepresentedElements.OfType<TModelElement>().Any(element => elementSelector(element)))
                    {
                        return diagramItem;
                    }
                }
            }

            return null;
        }
    }
}