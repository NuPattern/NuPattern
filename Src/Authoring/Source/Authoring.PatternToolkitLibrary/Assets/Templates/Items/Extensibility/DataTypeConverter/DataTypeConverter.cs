using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using NuPattern.Diagnostics;

namespace $rootnamespace$
{
    /// <summary>
    /// A custom type converter for converting between a string and a custom data type.
    /// </summary>
    /// <remarks>
    /// This converter must convert between a System.String and the custom data type.
    /// See http://msdn.microsoft.com/en-us/library/ayybcxe5.aspx for more details on how to implement a type converter.
    /// </remarks>
    [DisplayName("$safeitemname$ Custom Data Type Converter")]
    [Category("General")]
    [Description("Converts between a string and a custom data type.")]
    [CLSCompliant(false)]
    public class $safeitemname$ : TypeConverter
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<$safeitemname$>();
    
        /// <summary>
        /// Determines if this converter can convert from the source type.
        /// </summary>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if ((sourceType == typeof(string))
                || (typeof(YourCustomDataType).IsAssignableFrom(sourceType)))
            {
                return true;
            }
            else
            {
                return base.CanConvertFrom(context, sourceType);
            }
        }
        
        /// <summary>
        /// Determines if this converter can convert to the destination type.
        /// </summary>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if ((destinationType == typeof(string))
                || (destinationType == typeof(YourCustomDataType)))
            {
                return true;
            }
            else
            {
                return base.CanConvertTo(context, destinationType);
            }
        }

        /// <summary>
        /// Converts from a <see cref="System.String"/> to a custom data type.
        /// </summary>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var instance = TryCreate(value);
            if (instance != null)
            {
                return instance;
            }
            else
            {
                return base.ConvertFrom(context, culture, value);
            }
        }

        /// <summary>
        /// Converts to a <see cref="System.String"/> from an instance of the custom data type.
        /// </summary>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            var instance =  value as YourCustomDataType;
            if ((destinationType == typeof(string)) && (instance != null))
            {
                // TODO: Return the string representation of an the instance of the custom data type.
                return instance.ToString();
            }
            else
            {
                return base.ConvertTo(context, culture, value, destinationType);
            }
        }

        /// <summary>
        /// Determines if the value is valid with respect to its range and bounds.
        /// </summary>
        public override bool IsValid(ITypeDescriptorContext context, object value)
        {
            var instance = TryCreate(value);
            if (instance != null)
            {
                // TODO: Verify the basic range and bounds, and return false if not valid.
                // Note: Validation rules on properties will verify actual current value.
                return true;
            }
            else
            {
                return (value is YourCustomDataType);
            }
        }

        /// <summary>
        /// Determines if the string value represents an instance of the custom data type.
        /// </summary>
        private YourCustomDataType TryCreate(object value)
        {
            var stringValue = value as string;
            if (stringValue != null)
            {
                // TODO: Parse and construct a new instance of the custom data type with the string value.
                return new YourCustomDataType(stringValue);
            }
            else
            {
                if (value is YourCustomDataType)
                {
                    // TODO: Construct a new instance of the custom data type with the existing instance data.
                    return new YourCustomDataType(value);
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
