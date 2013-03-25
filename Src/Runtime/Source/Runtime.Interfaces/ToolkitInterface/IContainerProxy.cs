using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace NuPattern.Runtime.ToolkitInterface
{
    /// <summary>
    /// Proxy interface for element containers such as views, collections and elements.
    /// </summary>
    /// <typeparam name="TInterface">The type of the strong-typed interface for the container.</typeparam>
    [CLSCompliant(false)]
    public interface IContainerProxy<TInterface> : IFluentInterface
    {
        /// <summary>
        /// Creates a child element of the given type.
        /// </summary>
        /// <typeparam name="TElement">The type of the toolkit interface layer of element to create.</typeparam>
        /// <param name="name">The name of the element to create.</param>
        /// <param name="initializer">The optional initializer.</param>
        /// <param name="raiseInstantiateEvents">Whether the creation will trigger the element instantiate event.</param>
        TElement CreateElement<TElement>(string name, Action<TElement> initializer = null, bool raiseInstantiateEvents = true)
            where TElement : class;

        /// <summary>
        /// Creates a child collection of the given type.
        /// </summary>
        /// <typeparam name="TCollection">The type of the toolkit interface layer of collection to create.</typeparam>
        /// <param name="name">The name of the collection to create.</param>
        /// <param name="initializer">The optional initializer.</param>
        /// <param name="raiseInstantiateEvents">Whether the creation will trigger the element instantiate event.</param>
        TCollection CreateCollection<TCollection>(string name, Action<TCollection> initializer = null, bool raiseInstantiateEvents = true)
            where TCollection : class;

        /// <summary>
        /// Creates an extended pattern of the given type.
        /// </summary>
        /// <typeparam name="TExtension">The type of the toolkit interface layer of extension to create.</typeparam>
        /// <param name="name">The name of the pattern to create.</param>
        /// <param name="productId">The id of the extended pattern.</param>
        /// <param name="toolkitId">The id of the toolkit containing the pattern.</param>
        /// <param name="initializer">The optional initializer.</param>
        /// <param name="raiseInstantiateEvents">Whether the creation will trigger the element instantiate event.</param>
        TExtension CreateExtension<TExtension>(string name, Guid productId, string toolkitId, Action<TExtension> initializer = null, bool raiseInstantiateEvents = true)
            where TExtension : class;

        /// <summary>
        /// Gets the element from the container that matches the <paramref name="propertyExpression"/> type, 
        /// calling the <paramref name="elementProxyFactory"/> to create the actual implementation.
        /// </summary>
        /// <param name="propertyExpression">The property expression.</param>
        /// <param name="elementProxyFactory">The element proxy factory.</param>
        TElement GetElement<TElement>(Expression<Func<TElement>> propertyExpression, Func<IAbstractElement, TElement> elementProxyFactory)
            where TElement : class;

        /// <summary>
        /// Gets the elements from the container that match the <paramref name="propertyExpression"/> type, 
        /// calling the <paramref name="elementProxyFactory"/> to create the actual implementation.
        /// </summary>
        /// <param name="propertyExpression">The property expression.</param>
        /// <param name="elementProxyFactory">The element proxy factory.</param>
        IEnumerable<TElement> GetElements<TElement>(Expression<Func<IEnumerable<TElement>>> propertyExpression, Func<IAbstractElement, TElement> elementProxyFactory)
            where TElement : class;

        /// <summary>
        /// Gets the extension from the container that matches the <paramref name="propertyExpression"/> type, 
        /// calling the <paramref name="productProxyFactory"/> to create the actual implementation.
        /// </summary>
        /// <param name="propertyExpression">The property expression.</param>
        /// <param name="productProxyFactory">The pattern proxy factory.</param>
        TExtension GetExtension<TExtension>(Expression<Func<TExtension>> propertyExpression, Func<IProduct, TExtension> productProxyFactory)
            where TExtension : class;

        /// <summary>
        /// Gets the extensions from the container that match the <paramref name="propertyExpression"/> type, 
        /// calling the <paramref name="productProxyFactory"/> to create the actual implementation.
        /// </summary>
        /// <param name="propertyExpression">The property expression.</param>
        /// <param name="productProxyFactory">The pattern proxy factory.</param>
        IEnumerable<TExtension> GetExtensions<TExtension>(Expression<Func<IEnumerable<TExtension>>> propertyExpression, Func<IProduct, TExtension> productProxyFactory)
            where TExtension : class;
    }
}