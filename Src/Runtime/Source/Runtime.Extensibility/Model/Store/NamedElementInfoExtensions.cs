using System.Collections.Generic;
using System.Linq;

namespace NuPattern.Runtime.Extensibility
{
    /// <summary>
    /// Extensions for named element information.
    /// </summary>
    public static class NamedElementInfoExtensions
    {
        /// <summary>
        /// Gets the pattern information for the root pattern.
        /// The root pattern is the top most pattern that has no parent pattern.
        /// </summary>
        public static IPatternInfo GetRoot(this INamedElementInfo element)
        {
            var current = element;
            while (current.Parent != null)
            {
                current = current.Parent;
            }

            return current as IPatternInfo;
        }

        /// <summary>
        /// Gets the pattern information for the owning pattern.
        /// For an extension pattern this is the owning pattern.
        /// For a non-parented pattern, or elements in a non-parent pattern, this is the root pattern (<see cref="GetRoot"/>/>.
        /// </summary>
        public static IPatternInfo GetProduct(this INamedElementInfo element)
        {
            var current = element;

            // Check for an extension pattern
            var extensionProduct = current as IPatternInfo;
            if (extensionProduct != null && extensionProduct.Parent != null)
            {
                // Move into parent pattern (view or abstractelement)
                current = extensionProduct.Parent;
            }

            // Get the parent until we reach the next pattern
            while (current != null && !(current is IPatternInfo))
            {
                current = current.Parent;
            }

            return current as IPatternInfo;

        }

        /// <summary>
        /// Returns all properties of all descendant elements of the given element.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static IEnumerable<IPropertyInfo> GetAllProperties(this INamedElementInfo element)
        {
            var pattern = element as IPatternInfo;
            if (pattern != null)
            {
                var properties = pattern.Views
                    .SelectMany(v => v.Elements)
                    .SelectMany(e => e.FindDescendants().Concat(new[] { e }))
                    .Concat(new[] { pattern })
                    .SelectMany(d => d.Properties);

                return properties;
            }

            var view = element as IViewInfo;
            if (view != null)
            {
                var properties = view.Elements
                    .SelectMany(e => e.FindDescendants().Concat(new[] { e }))
                    .SelectMany(d => d.Properties);

                return properties;
            }

            var productElement = element as IPatternElementInfo;
            if (productElement != null)
            {
                var properties = productElement.FindDescendants()
                    .Concat(new[] { productElement })
                    .SelectMany(d => d.Properties);

                return properties;
            }

            return null;
        }

        private static IEnumerable<IPatternElementInfo> FindDescendants(this IPatternElementInfo element)
        {
            var container = element as IElementInfoContainer;
            if (container != null)
            {
                return container.Elements.Concat(container.Elements.SelectMany(e => e.FindDescendants()));
            }
            else
            {
                return Enumerable.Empty<IPatternElementInfo>();
            }
        }
    }
}
