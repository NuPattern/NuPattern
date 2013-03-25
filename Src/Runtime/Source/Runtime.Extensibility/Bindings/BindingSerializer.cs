using System;
using NuPattern.Runtime.Properties;
using NuPattern.Runtime.Serialization;

namespace NuPattern.Runtime.Bindings
{
    /// <summary>
    /// Provides simple Json serialization of a serializable object or 
    /// data contract.
    /// </summary>
    public static class BindingSerializer
    {
        private static readonly JsonConverter bindingConverter = new BindingSettingsConverter();
        private static readonly JsonConverter propertyConverter = new PropertyBindingSettingsConverter();
        private static readonly JsonConverter valueProviderConverter = new ValueProviderBindingSettingsConverter();

        /// <summary>
        /// Deserializes the specified serialized value.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object,System.Object)")]
        public static T Deserialize<T>(string serializedValue)
             where T : class
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(serializedValue,
                    new IsoDateTimeConverter(),
                    propertyConverter, valueProviderConverter, bindingConverter);
            }
            catch (JsonSerializationException ex)
            {
                throw new BindingSerializationException(string.Format(
                    Resources.BindingSerializationException_DeserializationFailed,
                    typeof(T),
                    serializedValue),
                    ex);
            }
        }

        /// <summary>
        /// Deserializes the specified serialized value.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object,System.Object)")]
        public static object Deserialize(string serializedValue, Type objectType)
        {
            try
            {
                return JsonConvert.DeserializeObject(serializedValue, objectType,
                    new IsoDateTimeConverter(),
                    propertyConverter, valueProviderConverter, bindingConverter);
            }
            catch (JsonSerializationException ex)
            {
                throw new BindingSerializationException(string.Format(
                    Resources.BindingSerializationException_DeserializationFailed,
                    objectType,
                    serializedValue),
                    ex);
            }
        }

        /// <summary>
        /// Serializes the specified value.
        /// </summary>
        public static string Serialize<T>(T value)
             where T : class
        {
            try
            {
                return JsonConvert.SerializeObject(value, Formatting.Indented, new IsoDateTimeConverter());

            }
            catch (JsonSerializationException ex)
            {
                throw new BindingSerializationException(ex.Message, ex);
            }
        }

        private class BindingSettingsConverter : CustomCreationConverter<IBindingSettings>
        {
            public override IBindingSettings Create(System.Type objectType)
            {
                Guard.NotNull(() => objectType, objectType);

                if (objectType.IsInterface)
                {
                    return new BindingSettings();
                }

                return (IBindingSettings)Activator.CreateInstance(objectType);
            }
        }

        private class PropertyBindingSettingsConverter : CustomCreationConverter<IPropertyBindingSettings>
        {
            public override IPropertyBindingSettings Create(System.Type objectType)
            {
                Guard.NotNull(() => objectType, objectType);

                if (objectType.IsInterface)
                {
                    return new PropertyBindingSettings();
                }

                return (IPropertyBindingSettings)Activator.CreateInstance(objectType);
            }
        }

        private class ValueProviderBindingSettingsConverter : CustomCreationConverter<IValueProviderBindingSettings>
        {
            public override IValueProviderBindingSettings Create(System.Type objectType)
            {
                Guard.NotNull(() => objectType, objectType);

                if (objectType.IsInterface)
                {
                    return new ValueProviderBindingSettings();
                }

                return (IValueProviderBindingSettings)Activator.CreateInstance(objectType);
            }
        }
    }
}