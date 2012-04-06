
using System;
namespace Microsoft.VisualStudio.Patterning.Extensibility.Serialization.Json
{
    /// <summary>
    /// Json serialization converter class.
    /// </summary>
    public abstract class JsonConverter
    {
        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public abstract void WriteJson(JsonWriter writer, object value, JsonSerializer serializer);

        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="JsonReader"/> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>The object value.</returns>
        public abstract object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer);

        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>
        /// 	<c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
        /// </returns>
        public abstract bool CanConvert(Type objectType);

        ///// <summary>
        ///// Gets the <see cref="JsonSchema"/> of the JSON produced by the JsonConverter.
        ///// </summary>
        ///// <returns>The <see cref="JsonSchema"/> of the JSON produced by the JsonConverter.</returns>
        //public virtual JsonSchema GetSchema()
        //{
        //    return null;
        //}

        /// <summary>
        /// Gets a value indicating whether this <see cref="JsonConverter"/> can read JSON.
        /// </summary>
        /// <value><c>true</c> if this <see cref="JsonConverter"/> can read JSON; otherwise, <c>false</c>.</value>
        public virtual bool CanRead
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="JsonConverter"/> can write JSON.
        /// </summary>
        /// <value><c>true</c> if this <see cref="JsonConverter"/> can write JSON; otherwise, <c>false</c>.</value>
        public virtual bool CanWrite
        {
            get { return true; }
        }
    }
}
