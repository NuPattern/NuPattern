using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using NuPattern.Extensibility.Binding;

namespace NuPattern.Extensibility
{
    /// <summary>
    /// Provides a <see cref="TypeConverter"/> for a bindable property for runtime, that converts Json values.
    /// </summary>
    public class CollectionConverter<T> : CollectionConverter where T : class
    {
        /// <summary>
        /// Whether this converter can convert from the sourceType to the object, using the specified context.
        /// </summary>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return (sourceType == typeof(string)) || base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        /// Converts the given value to the object value, using the specified context and culture information. 
        /// </summary>
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            var stringValue = value as string;
            if (!string.IsNullOrEmpty(stringValue))
            {
                return BindingSerializer.Deserialize<Collection<T>>(stringValue);
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}
