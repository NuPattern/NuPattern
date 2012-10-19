using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;

namespace Microsoft.VisualStudio.Patterning.Runtime
{
	/// <summary>
	/// Proxy interface for element containers such as views, collections and elements.
	/// </summary>
	/// <typeparam name="TInterface">The type of the strong-typed interface for the container.</typeparam>
	[CLSCompliant(false)]
	public interface IContainerProxy<TInterface> : IFluentInterface
	{
		/// <summary>
		/// Gets the element from the container that matches the <paramref name="propertyExpression"/> type, 
		/// calling the <paramref name="elementProxyFactory"/> to create the actual implementation.
		/// </summary>
		/// <param name="propertyExpression">The property expression.</param>
		/// <param name="elementProxyFactory">The element proxy factory.</param>
		TElement GetElement<TElement>(Expression<Func<TElement>> propertyExpression, Func<IAbstractElement, TElement> elementProxyFactory);

		/// <summary>
		/// Gets the elements from the container that match the <paramref name="propertyExpression"/> type, 
		/// calling the <paramref name="elementProxyFactory"/> to create the actual implementation.
		/// </summary>
		/// <param name="propertyExpression">The property expression.</param>
		/// <param name="elementProxyFactory">The element proxy factory.</param>
		IEnumerable<TElement> GetElements<TElement>(Expression<Func<IEnumerable<TElement>>> propertyExpression, Func<IAbstractElement, TElement> elementProxyFactory);

		/// <summary>
		/// Gets the extension from the container that matches the <paramref name="propertyExpression"/> type, 
		/// calling the <paramref name="productProxyFactory"/> to create the actual implementation.
		/// </summary>
		/// <param name="propertyExpression">The property expression.</param>
		/// <param name="productProxyFactory">The product proxy factory.</param>
		TExtension GetExtension<TExtension>(Expression<Func<TExtension>> propertyExpression, Func<IProduct, TExtension> productProxyFactory);

		/// <summary>
		/// Gets the extensions from the container that match the <paramref name="propertyExpression"/> type, 
		/// calling the <paramref name="productProxyFactory"/> to create the actual implementation.
		/// </summary>
		/// <param name="propertyExpression">The property expression.</param>
		/// <param name="productProxyFactory">The product proxy factory.</param>
		IEnumerable<TExtension> GetExtensions<TExtension>(Expression<Func<IEnumerable<TExtension>>> propertyExpression, Func<IProduct, TExtension> productProxyFactory);
	}
}