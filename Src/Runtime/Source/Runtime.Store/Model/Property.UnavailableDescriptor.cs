using System;
using System.ComponentModel;
using Microsoft.VisualStudio.Patterning.Runtime.Store.Properties;

namespace Microsoft.VisualStudio.Patterning.Runtime.Store
{
	/// <summary>
	/// Custom descriptor for properties that don't have a corresponding information schema.
	/// </summary>
	internal class PropertyUnavailableDescriptor : PropertyDescriptor
	{
		private Property property;

		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyPropertyDescriptor"/> class.
		/// </summary>
		/// <param name="property">The property.</param>
		public PropertyUnavailableDescriptor(Property property)
			: base(/*  make up a unique name. we never show this property anywhere */ Guid.NewGuid().ToString(), BuildAttributes())
		{
			this.property = property;
		}

		/// <summary>
		/// Returns <see cref="ProductElement"/>.
		/// </summary>
		public override Type ComponentType
		{
			get { return typeof(ProductElement); }
		}

		/// <summary>
		/// Always returns <see langword="true"/>.
		/// </summary>
		public override bool IsReadOnly
		{
			get { return true; }
		}

		/// <summary>
		/// Always returns <see cref="System.String"/>.
		/// </summary>
		public override Type PropertyType
		{
			get { return typeof(string); }
		}

		/// <summary>
		/// Always returns <see langword="false"/>.
		/// </summary>
		public override bool CanResetValue(object component)
		{
			return false;
		}

		/// <summary>
		/// Returns the raw value of the property, as no type conversion can be performed.
		/// </summary>
		public override object GetValue(object component)
		{
			return this.property.RawValue;
		}

		/// <summary>
		/// Throws <see cref="InvalidOperationException"/>.
		/// </summary>
		public override void ResetValue(object component)
		{
			throw new InvalidOperationException(Resources.Property_UnavailableException);
		}

		/// <summary>
		/// Throws <see cref="InvalidOperationException"/>.
		/// </summary>
		public override void SetValue(object component, object value)
		{
			throw new InvalidOperationException(Resources.Property_UnavailableException);
		}

		/// <summary>
		/// Always returns <see langword="true"/>.
		/// </summary>
		public override bool ShouldSerializeValue(object component)
		{
			return true;
		}

		private static Attribute[] BuildAttributes()
		{
			return new Attribute[] 
			{
				new BrowsableAttribute(false),
				new ReadOnlyAttribute(true),
			};
		}
	}
}
