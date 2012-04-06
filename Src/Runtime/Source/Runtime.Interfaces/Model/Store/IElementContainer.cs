using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Patterning.Runtime
{
    /// <summary>
    /// Base interface for collection, element and view, which can 
    /// contain <see cref="IAbstractElement"/> children and <see cref="IProduct"/> 
    /// extensions.
    /// </summary>
    public interface IElementContainer : IInstanceBase
    {
        /// <summary>
        /// Gets the definition information for the element.
        /// </summary>
        [Hidden]
        new IElementInfoContainer Info { get; }

        /// <summary>
        /// Gets the contained elements (unordered).
        /// </summary>
        [Hidden]
        IEnumerable<IAbstractElement> Elements { get; }

        /// <summary>
        /// Gets the contained extension points (unordered).
        /// </summary>
        [Hidden]
        IEnumerable<IProduct> Extensions { get; }

        /// <summary>
        /// Gets the contained elements and extension points (ordered).
        /// </summary>
        [Hidden]
        IEnumerable<IProductElement> AllElements { get; }

        /// <summary>
        /// Creates an instance of a child <see cref="ICollection"/> with an optional initializer to perform 
        /// object initialization within the creation transaction. The child is automatically added to the 
        ///	<see cref="Elements"/> property.
        /// </summary>
        ICollection CreateCollection(Action<ICollection> initializer = null, bool raiseInstantiateEvents = true);

        /// <summary>
        /// Creates an instance of a child <see cref="IElement"/> with an optional initializer to perform 
        /// object initialization within the creation transaction. The child is automatically added to the 
        ///	<see cref="Elements"/> property.
        /// </summary>
        IElement CreateElement(Action<IElement> initializer = null, bool raiseInstantiateEvents = true);

        /// <summary>
        /// Creates an instance of a child <see cref="IProduct"/> with an optional initializer to perform 
        /// object initialization within the creation transaction. The child is automatically added to the 
        ///	<see cref="Extensions"/> property.
        /// </summary>
        IProduct CreateExtension(Action<IProduct> initializer = null, bool raiseInstantiateEvents = true);
    }
}
