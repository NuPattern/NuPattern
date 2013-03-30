using System.Collections.Generic;
using System.Linq;

namespace NuPattern.Runtime
{
    /// <summary>
    /// Extensions for named element schema.
    /// </summary>
    public static class NamedElementSchemaExtensions
    {
        /// <summary>
        /// Gets the pattern schema for the root pattern.
        /// The root pattern is the top most pattern that has no parent pattern.
        /// </summary>
        public static IPatternSchema GetRoot(this INamedElementSchema element)
        {
            var current = element;
            while (current.Parent != null)
            {
                current = current.Parent;
            }

            return current as IPatternSchema;
        }

        /// <summary>
        /// Gets the pattern schema for the owning pattern.
        /// For an extension pattern this is the owning pattern.
        /// For a non-parented pattern, or elements in a non-parent pattern, this is the root pattern (<see cref="GetRoot"/>/>.
        /// </summary>
        public static IPatternSchema GetProduct(this INamedElementSchema element)
        {
            var current = element;

            // Check for an extension pattern
            var extensionProduct = current as IPatternSchema;
            if (extensionProduct != null && extensionProduct.Parent != null)
            {
                // Move into parent pattern (view or abstractelement)
                current = extensionProduct.Parent;
            }

            // Get the parent until we reach the next pattern
            while (current != null && !(current is IPatternSchema))
            {
                current = current.Parent;
            }

            return current as IPatternSchema;

        }

        /// <summary>
        /// Returns all properties of all descendant elements of the given element.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static IEnumerable<IPropertySchema> GetAllProperties(this INamedElementSchema element)
        {
            var pattern = element as IPatternSchema;
            if (pattern != null)
            {
                var properties = pattern.Views
                    .SelectMany(v => ((IElementSchemaContainer)v).Elements)
                    .SelectMany(e => e.FindDescendants().Concat(new[] { e }))
                    .Concat(new[] { pattern })
                    .SelectMany(d => d.Properties);

                return properties;
            }

            var view = element as IViewSchema;
            if (view != null)
            {
                var properties = ((IElementSchemaContainer)view).Elements
                    .SelectMany(e => e.FindDescendants().Concat(new[] { e }))
                    .SelectMany(d => d.Properties);

                return properties;
            }

            var productElement = element as IPatternElementSchema;
            if (productElement != null)
            {
                var properties = productElement.FindDescendants()
                    .Concat(new[] { productElement })
                    .SelectMany(d => d.Properties);

                return properties;
            }

            return null;
        }

        private static IEnumerable<IPatternElementSchema> FindDescendants(this IPatternElementSchema element)
        {
            var container = element as IElementSchemaContainer;
            if (container != null)
            {
                return container.Elements.Concat(container.Elements.SelectMany(e => e.FindDescendants()));
            }
            else
            {
                return Enumerable.Empty<IPatternElementSchema>();
            }
        }
    }
}
