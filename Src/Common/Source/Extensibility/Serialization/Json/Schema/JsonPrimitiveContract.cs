using System;

namespace NuPattern.Extensibility.Serialization.Json
{
    /// <summary>
    /// Contract details for a <see cref="Type"/> used by the <see cref="JsonSerializer"/>.
    /// </summary>
    public class JsonPrimitiveContract : JsonContract
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonPrimitiveContract"/> class.
        /// </summary>
        /// <param name="underlyingType">The underlying type for the contract.</param>
        public JsonPrimitiveContract(Type underlyingType)
            : base(underlyingType)
        {
        }
    }
}