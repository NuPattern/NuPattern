using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Microsoft.VisualStudio.Patterning.Extensibility
{
	/// <summary>
	/// Custom delegating descriptor that allows externalizing the persistence-related operations 
	/// via anonymous functions.
	/// </summary>
	public class AnonymousPropertyDescriptor<TProperty> : DelegatingPropertyDescriptor
	{
		private Func<TProperty> valueGetter;
		private Action<TProperty> valueSetter;
		private Action valueReset;

		/// <summary>
		/// Initializes a new instance of the <see cref="AnonymousPropertyDescriptor{T}"/> class.
		/// </summary>
		public AnonymousPropertyDescriptor(
			Func<TProperty> valueGetter,
			Action<TProperty> valueSetter,
			PropertyDescriptor innerDescriptor,
			Action valueReset = null,
			params Attribute[] overriddenAttributes)
			: base(innerDescriptor, overriddenAttributes)
		{
			Guard.NotNull(() => valueGetter, valueGetter);
			Guard.NotNull(() => valueSetter, valueSetter);

			this.valueGetter = valueGetter;
			this.valueSetter = valueSetter;
			this.valueReset = valueReset;
		}

		/// <summary>
		/// Gets the type of the property.
		/// </summary>
		/// <value></value>
		/// <returns>A <see cref="T:System.Type"/> that represents the type of the property.</returns>
		public override Type PropertyType { get { return typeof(TProperty); } }

		/// <summary>
		/// Returns whether resetting an object changes its value.
		/// </summary>
		/// <param name="component">The component to test for reset capability.</param>
		/// <returns>Returns <see langword="true"/> if resetting the component changes its value; <see langword="false"/> otherwise.</returns>
		public override bool CanResetValue(object component)
		{
			return this.valueReset != null;
		}

		/// <summary>
		/// Resets the value for this property of the component to the default value.
		/// </summary>
		/// <param name="component">The component with the property value that is to be reset to the default value.</param>
		public override void ResetValue(object component)
		{
			if (CanResetValue(component))
				this.valueReset();
		}

		/// <summary>
		/// Gets the current value of the property on a component.
		/// </summary>
		/// <param name="component">The component with the property for which to retrieve the value.</param>
		/// <returns>
		/// The value of a property for a given component.
		/// </returns>
		public override object GetValue(object component)
		{
			return valueGetter();
		}

		/// <summary>
		/// Sets the value of the component to a different value.
		/// </summary>
		/// <param name="component">The component with the property value that is to be set.</param>
		/// <param name="value">The new value.</param>
		public override void SetValue(object component, object value)
		{
			valueSetter((TProperty)value);
		}
	}
}
