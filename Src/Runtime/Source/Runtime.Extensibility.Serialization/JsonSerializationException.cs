using System;
using System.Runtime.Serialization;

namespace NuPattern.Runtime.Serialization
{
    /// <summary>
    /// A Json serialization exception
    /// </summary>
    public class JsonSerializationException : Exception
    {
        /// <summary>
        /// Creates a new instance of the <see cref="JsonSerializationException"/> exception.
        /// </summary>
        public JsonSerializationException()
            : base()
        {

        }
        /// <summary>
        /// Creates a new instance of the <see cref="JsonSerializationException"/> exception.
        /// </summary>
        public JsonSerializationException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
        /// <summary>
        /// Creates a new instance of the <see cref="JsonSerializationException"/> exception.
        /// </summary>
        public JsonSerializationException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// Creates a new instance of the <see cref="JsonSerializationException"/> exception.
        /// </summary>
        protected JsonSerializationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
