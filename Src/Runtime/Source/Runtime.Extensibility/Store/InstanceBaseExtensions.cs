using System.Collections.Generic;
using System.Linq;

namespace NuPattern.Runtime
{
    /// <summary>
    /// Helper methods for dealing with runtime element instances.
    /// </summary>
    public static class InstanceBaseExtensions
    {
        /// <summary>
        /// Traverses from the specified parent to all the children.
        /// </summary>
        /// <param name="parent">The parent to traverse.</param>
        public static IEnumerable<IInstanceBase> Traverse(this IInstanceBase parent)
        {
            return Traverse(new[] { parent });
        }

        /// <summary>
        /// Traverses from the specified elements to all the children.
        /// </summary>
        /// <param name="elements">The elements to traverse.</param>
        public static IEnumerable<IInstanceBase> Traverse(this IEnumerable<IInstanceBase> elements)
        {
            foreach (var product in elements.OfType<IProduct>())
            {
                yield return product;

                foreach (var element in Traverse(product.Views))
                {
                    yield return element;
                }
            }

            foreach (var container in elements.OfType<IElementContainer>())
            {
                yield return container;

                foreach (var element in Traverse(container.Elements))
                {
                    yield return element;
                }

                foreach (var element in Traverse(container.Extensions))
                {
                    yield return element;
                }
            }
        }
    }
}