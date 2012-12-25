using System.Collections.Generic;
using System.Linq;
using NuPattern.Runtime.Schema;

namespace NuPattern.Runtime
{
    /// <summary>
    /// Extension to the <see cref="IElementSchemaContainer"/> interface.
    /// </summary>
    public static class ElementSchemaContainerExtensions
    {
        /// <summary>
        /// Returns the aggregation of all the abstract elements and extension points for a container.
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        public static IEnumerable<PatternElementSchema> AllElements(this IElementSchemaContainer container)
        {
            return container.Elements.Cast<PatternElementSchema>()
                .Concat(container.ExtensionPoints.Cast<PatternElementSchema>());
        }
    }
}
