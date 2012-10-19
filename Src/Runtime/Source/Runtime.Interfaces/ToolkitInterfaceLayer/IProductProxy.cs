using System;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;

namespace Microsoft.VisualStudio.Patterning.Runtime
{
	/// <summary>
	/// Proxy interface for products.
	/// </summary>
	/// <typeparam name="TInterface">The type of the strong-typed interface for the pattern.</typeparam>
	[CLSCompliant(false)]
	public interface IProductProxy<TInterface> : IPropertyProxy<TInterface>, IFluentInterface
	{
		/// <summary>
		/// Gets the view from the container that matches the <paramref name="propertyExpression"/> type, 
		/// which must be of the form 'this.ViewName', and calls the <paramref name="viewProxyFactory"/> to
		/// create the actual implementation.
		/// </summary>
		/// <param name="propertyExpression">The property expression.</param>
		/// <param name="viewProxyFactory">The view proxy factory.</param>
		TView GetView<TView>(Expression<Func<TView>> propertyExpression, Func<IView, TView> viewProxyFactory) where TView : class;
	}
}