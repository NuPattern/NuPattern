using System.Collections.Generic;

namespace NuPattern.Runtime
{
    /// <summary>
    /// Base interface for collection, element and view schema, which can 
    /// contain <see cref="IAbstractElementSchema"/>s and <see cref="IExtensionPointSchema"/>s
    /// </summary>
    public interface IElementSchemaContainer : INamedElementSchema
    {
        /// <summary>
        /// Gets the contained elements.
        /// </summary>
        [Hidden]
        IEnumerable<IAbstractElementSchema> Elements { get; }

        /// <summary>
        /// Gets the contained extension points.
        /// </summary>
        [Hidden]
        IEnumerable<IExtensionPointSchema> ExtensionPoints { get; }
    }
}
