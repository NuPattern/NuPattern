using System;
using System.Linq.Expressions;

namespace NuPattern.Runtime.ToolkitInterface
{
    /// <summary>
    /// Proxy interface for property container elements such as a pattern or element.
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Expresion", Justification = "It's the right name in the given context.")]
        TProperty GetValue<TProperty>(Expression<Func<TProperty>> propertyExpresion);

        /// <summary>
        /// Sets the value of a class or variable property with a name equal to the 
        /// property referenced by the expression, which must be of the form 'this.PropertyName'.
        /// </summary>
        /// <param name="propertyExpresion">The property expression.</param>
        /// <param name="value">The property value.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Expresion", Justification = "It's the right name in the given context.")]
        void SetValue<TProperty>(Expression<Func<TProperty>> propertyExpresion, TProperty value);
    }
}
