using System.Collections.Generic;

namespace Microsoft.VisualStudio.Patterning.Runtime
{
    /// <summary>
    /// Base interface for collection, element and view info, which can 
    /// contain <see cref="IAbstractElementInfo"/>s and <see cref="IPatternInfo"/>s.
    /// </summary>
    public interface IElementInfoContainer : INamedElementInfo
    {
        /// <summary>
        /// Gets the contained elements.
        /// </summary>
        [Hidden]
        IEnumerable<IAbstractElementInfo> Elements { get; }

        /// <summary>
        /// Gets the contained extension points.
        /// </summary>
        [Hidden]
        IEnumerable<IExtensionPointInfo> ExtensionPoints { get; }
    }
}
