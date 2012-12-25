using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using NuPattern.Runtime;

namespace NuPattern.Extensibility
{
	/// <summary>
	/// Converter that lists automation settings of the type
	/// <typeparamref name="TSettings"/> available
	/// in the containing element of the current element.
	/// </summary>
	/// <typeparam name="TSettings">The type of the settings.</typeparam>
	public class SettingsReferenceTypeConverter<TSettings> : TypeConverter
		where TSettings : IAutomationSettings
	{
		/// <summary>
		/// Converts the given value object to the specified type, using the specified context and culture information.
		/// </summary>
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			var settings = value as IAutomationSettings;

			if (destinationType == typeof(string) && settings != null)
			{
				return settings.Name;
			}

			return base.ConvertTo(context, culture, value, destinationType);
		}

		/// <summary>
		/// Returns whether the collection of standard values returned from <see cref="M:System.ComponentModel.TypeConverter.GetStandardValues"/> is an exclusive list of possible values, using the specified context.
		/// </summary>
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			return true;
		}

		/// <summary>
		/// Returns whether this object supports a standard set of values that can be picked from a list, using the specified context.
		/// </summary>
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		/// <summary>
		/// Returns a collection of standard values for the data type this type converter is designed for when provided with a format context.
		/// </summary>
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			if (context == null)
			{
				return new StandardValuesCollection(new ArrayList());
			}

			var settings = (IAutomationSettings)context.Instance;

			return new StandardValuesCollection(settings
				.Owner
				.AutomationSettings
				.Select(s => s.As<TSettings>())
				.Where(s => s != null)
				.ToArray());
		}
	}
}
