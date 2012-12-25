using System;
using System.Linq;

namespace NuPattern.Extensibility.Serialization
{
    /// <summary>
    /// A conversion class for serializing objects in Json.
    /// </summary>
    public static class JsonConvert
    {
        /// <summary>
        /// Serializes an object value to Json.
        /// </summary>
        public static string SerializeObject(object value)
        {
            return SerializeObject(value, Formatting.None, (JsonConverter[])null);
        }

        /// <summary>
        /// Serializes an object to Json, using the given settings.
        /// </summary>
        public static string SerializeObject(object value, Formatting formatting, JsonSerializerSettings settings)
        {
            try
            {
                var mappedFormat = (Newtonsoft.Json.Formatting)Enum.Parse(typeof(Newtonsoft.Json.Formatting), formatting.ToString(), true);
                var mappedSettings = new Newtonsoft.Json.JsonSerializerSettings
                {
                    ReferenceLoopHandling = (Newtonsoft.Json.ReferenceLoopHandling)Enum.Parse(typeof(Newtonsoft.Json.ReferenceLoopHandling), settings.ReferenceLoopHandling.ToString(), true),
                    TypeNameHandling = (Newtonsoft.Json.TypeNameHandling)Enum.Parse(typeof(Newtonsoft.Json.TypeNameHandling), settings.TypeNameHandling.ToString(), true),
                };

                return Newtonsoft.Json.JsonConvert.SerializeObject(value, mappedFormat, mappedSettings);
            }
            catch (Newtonsoft.Json.JsonSerializationException)
            {
                throw new JsonSerializationException();
            }
            catch (Newtonsoft.Json.JsonWriterException)
            {
                throw new JsonSerializationException();
            }
        }

        /// <summary>
        /// Serializes an object value to Json, using the given converters.
        /// </summary>
        public static string SerializeObject(object value, Formatting formatting, params JsonConverter[] converters)
        {
            try
            {
                //Convertion functions
                var mappedFormat = (Newtonsoft.Json.Formatting)Enum.Parse(typeof(Newtonsoft.Json.Formatting), formatting.ToString(), true);
                var mappedConverters = converters.ToList().Select(c => c.InnerConverter as Newtonsoft.Json.JsonConverter);

                return Newtonsoft.Json.JsonConvert.SerializeObject(value, mappedFormat, mappedConverters.ToArray());
            }
            catch (Newtonsoft.Json.JsonSerializationException)
            {
                throw new JsonSerializationException();
            }
            catch (Newtonsoft.Json.JsonWriterException)
            {
                throw new JsonSerializationException();
            }
        }

        /// <summary>
        /// Deserializes an object from Json, using the given converters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="converters"></param>
        /// <returns></returns>
        public static T DeserializeObject<T>(string value, params JsonConverter[] converters)
        {
            try
            {
                var mappedConverters = converters.ToList().Select(c => c.InnerConverter as Newtonsoft.Json.JsonConverter);

                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(value, mappedConverters.ToArray());
            }
            catch (Newtonsoft.Json.JsonSerializationException)
            {
                throw new JsonSerializationException();
            }
            catch (Newtonsoft.Json.JsonReaderException)
            {
                throw new JsonSerializationException();
            }
        }
    }
}
