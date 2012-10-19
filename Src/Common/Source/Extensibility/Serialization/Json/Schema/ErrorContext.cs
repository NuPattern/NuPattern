using System;

namespace Microsoft.VisualStudio.Patterning.Extensibility.Serialization.Json
{
    /// <summary>
    /// Provides information surrounding an error.
    /// </summary>
    public class ErrorContext
    {
        internal ErrorContext(object originalObject, object member, Exception error)
        {
            OriginalObject = originalObject;
            Member = member;
            Error = error;
        }

        /// <summary>
        /// Gets or sets the error.
        /// </summary>
        /// <value>The error.</value>
        public Exception Error { get; private set; }
        /// <summary>
        /// Gets the original object that caused the error.
        /// </summary>
        /// <value>The original object that caused the error.</value>
        public object OriginalObject { get; private set; }
        /// <summary>
        /// Gets the member that caused the error.
        /// </summary>
        /// <value>The member that caused the error.</value>
        public object Member { get; private set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ErrorContext"/> is handled.
        /// </summary>
        /// <value><c>true</c> if handled; otherwise, <c>false</c>.</value>
        public bool Handled { get; set; }
    }
}