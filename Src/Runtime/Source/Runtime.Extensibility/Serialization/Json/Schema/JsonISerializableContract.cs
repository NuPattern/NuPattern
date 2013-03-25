#if !SILVERLIGHT && !PocketPC
using System;

namespace NuPattern.Extensibility.Serialization.Json
{
    /// <summary>
    /// Contract details for a <see cref="Type"/> used by the <see cref="JsonSerializer"/>.
    /// </summary>
    public class JsonISerializableContract : JsonContract
    {
        /// <summary>
        /// Gets or sets the ISerializable object constructor.
        /// </summary>
        /// <value>The ISerializable object constructor.</value>
        public ObjectConstructor<object> ISerializableCreator { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonISerializableContract"/> class.
        /// </summary>
        /// <param name="underlyingType">The underlying type for the contract.</param>
        public JsonISerializableContract(Type underlyingType)
            : base(underlyingType)
        {
        }
    }
}
#endif