using System;
using System.Runtime.Serialization;

namespace NuPattern.Extensibility.Serialization.Json
{
    /// <summary>
    /// The exception thrown when an error occurs while reading Json text.
    /// </summary>
#if (!SILVERLIGHT && !WINDOWS_PHONE)
    [Serializable]
#endif
    internal class JsonReaderException : JsonSerializationException
    {
        /// <summary>
        /// Gets the line number indicating where the error occurred.
        /// </summary>
        /// <value>The line number indicating where the error occurred.</value>
        public int LineNumber { get; private set; }


        /// <summary>
        /// Gets the line position indicating where the error occurred.
        /// </summary>
        /// <value>The line position indicating where the error occurred.</value>
        public int LinePosition { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonReaderException"/> class.
        /// </summary>
        public JsonReaderException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonReaderException"/> class
        /// with a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public JsonReaderException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonReaderException"/> class
        /// with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public JsonReaderException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

#if !(WINDOWS_PHONE || SILVERLIGHT)
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonReaderException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
        public JsonReaderException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif

        internal JsonReaderException(string message, Exception innerException, int lineNumber, int linePosition)
            : base(message, innerException)
        {
            LineNumber = lineNumber;
            LinePosition = linePosition;
        }
    }
}
