using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;

namespace Microsoft.VisualStudio.Patterning.Extensibility
{
	/// <summary>
	/// An <see cref="ExpandableObjectConverter"/> for string types.
	/// </summary>
	public abstract class ExpandableObjectStringConverter : ExpandableObjectConverter
	{
		/// <summary>
		/// Adds support for converting the property value to string.
		/// </summary>
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return ((destinationType == typeof(InstanceDescriptor)) || base.CanConvertTo(context, destinationType));
		}

		/// <summary>
		/// Adds support for converting from a string to the value of this property.
		/// </summary>
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return ((sourceType == typeof(string)) || base.CanConvertFrom(context, sourceType));
		}

		/// <summary>
		/// Converts the given property value to a string.
		/// </summary>
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			Guard.NotNull(() => destinationType, destinationType);

			if (destinationType == typeof(string))
			{
				if (value == null)
				{
					return null;
				}

				return value.ToString();
			}

			if ((destinationType == typeof(InstanceDescriptor))
				&& (value is string))
			{
				return new InstanceDescriptor(typeof(string).GetConstructor(Type.EmptyTypes), null);
			}

			return base.ConvertTo(context, culture, value, destinationType);
		}

		/// <summary>
		/// Converts the given string to the value of the property.
		/// </summary>
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			string text = value as string;
			if (text == null)
			{
				return base.ConvertFrom(context, culture, value);
			}

			text = text.Trim();
			if (text.Length == 0)
			{
				return null;
			}

			return text;
		}

		/// <summary>
		/// Gets a value indicating whether this object supports properties using the specified context.
		/// </summary>
		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			return (context != null);
		}
	}
}
