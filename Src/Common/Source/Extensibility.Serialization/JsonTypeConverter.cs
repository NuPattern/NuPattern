using System;
using System.ComponentModel;
using Newtonsoft.Json;

namespace Microsoft.VisualStudio.Patterning.Extensibility.Serialization
{
    /// <summary>
    /// A type converter that converts to and from a Json string serialization of a value 
    /// of the given <typeparamref name="TValue"/>.
    /// </summary>
    public class JsonTypeConverter<TValue> : StringConverter
        where TValue : new()
    {
        /// <summary>
        /// Serializes the value to a Json string.
        /// </summary>
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                if (value == null)
                    return string.Empty;

                return JsonConvert.SerializeObject((TValue)value);
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        /// <summary>
        /// Converts the given string value to the typed value.
        /// </summary>
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            var stringValue = (string)value;

            if (string.IsNullOrWhiteSpace(stringValue))
                return new TValue();

            return JsonConvert.DeserializeObject<TValue>(stringValue);
        }
    }
}
