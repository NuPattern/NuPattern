using System;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;

namespace NuPattern.Runtime
{
	/// <summary>
	/// Proxy interface for property container elements such as a product or element.
	/// </summary>
	/// <typeparam name="TInterface">The type of the strong-typed interface for the container.</typeparam>
	[CLSCompliant(false)]
	public interface IPropertyProxy<TInterface> : IFluentInterface
	{
		/// <summary>
		/// Gets the value of a class or variable property with a name equal to the 
		/// property referenced by the expression, which must be of the form 'this.PropertyName'.
		/// </summary>
		/// <param name="propertyExpresion">The property expression.</param>
		TProperty GetValue<TProperty>(Expression<Func<TProperty>> propertyExpresion);

		/// <summary>
		/// Sets the value of a class or variable property with a name equal to the 
		/// property referenced by the expression, which must be of the form 'this.PropertyName'.
		/// </summary>
		/// <param name="propertyExpresion">The property expression.</param>
		/// <param name="value">The property value.</param>
		void SetValue<TProperty>(Expression<Func<TProperty>> propertyExpresion, TProperty value);
	}
}
