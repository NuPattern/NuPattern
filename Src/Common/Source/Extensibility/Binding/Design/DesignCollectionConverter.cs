using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using NuPattern.Extensibility.Properties;
using NuPattern.Extensibility.Serialization;

namespace NuPattern.Extensibility.Binding
{
    /// <summary>
    /// Converts between collections and string values for both runtime and design time.
    /// </summary>
    public class DesignCollectionConverter<TCollection, TCollected> : DesignCollectionConverter
        where TCollection : Collection<TCollected>
        where TCollected : class
    {
        /// <summary>
        /// Creates a new instance of the <see cref="DesignCollectionConverter"/> class.
        /// </summary>
        public DesignCollectionConverter()
            : base(typeof(TCollection))
        {
        }
    }

    /// <summary>
    /// Converts between collections and string values for both runtime and design time.
    /// </summary>
    /// <remarks>
    /// CanConvertTo and ConvertTo are handling design-time conversion of objects to the string shown to user.
    /// CanConvertFrom and ConvertFrom are handling runtime conversion of string to the collection or objects.
    /// </remarks>
    public class DesignCollectionConverter : TypeConverter
    {
        private Type type;

        /// <summary>
        /// Creates a new instance of the <see cref="DesignCollectionConverter"/> class.
        /// </summary>
        public DesignCollectionConverter(Type type)
        {
            Guard.NotNull(() => type, type);

            this.type = type;
        }

        /// <summary>
        /// Whether this converter can convert the object to the destinationType, using the specified context.
        /// </summary>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(InstanceDescriptor) || base.CanConvertTo(context, destinationType);
        }

        /// <summary>
        /// Converts the given object value to the destinationType type, using the specified context and culture information.
        /// </summary>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            return destinationType == typeof(string) ?
                Resources.DesignCollectionPropertyDescriptor_ToString :
                base.ConvertTo(context, culture, value, destinationType);
        }

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
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            // Copy value back to descriptor at design time
            if (value is Collection<object>)
            {
                return value as Collection<object>;
            }

            // Deserializing value at runtime
            var stringValue = value as string;
            if (!string.IsNullOrEmpty(stringValue))
            {
                try
                {
                    return BindingSerializer.Deserialize(stringValue, this.type);
                }
                catch (JsonSerializationException)
                {
                    return base.ConvertFrom(context, culture, value);
                }
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}
