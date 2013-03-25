#if !(NET35 || NET20 || WINDOWS_PHONE)
using System;

namespace NuPattern.Extensibility.Serialization.Json
{
    /// <summary>
    /// Contract details for a <see cref="Type"/> used by the <see cref="JsonSerializer"/>.
    /// </summary>
    public class JsonDynamicContract : JsonContract
    {
        /// <summary>
        /// Gets the object's properties.
        /// </summary>
        /// <value>The object's properties.</value>
        public JsonPropertyCollection Properties { get; private set; }

        /// <summary>
        /// Gets or sets the property name resolver.
        /// </summary>
        /// <value>The property name resolver.</value>
        public Func<string, string> PropertyNameResolver { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonDynamicContract"/> class.
        /// </summary>
        /// <param name="underlyingType">The underlying type for the contract.</param>
        public JsonDynamicContract(Type underlyingType)
            : base(underlyingType)
        {
            Properties = new JsonPropertyCollection(UnderlyingType);
        }
    }
}
#endif