using System;
using System.Runtime.Serialization;

namespace NuPattern.Runtime.Shell.Shortcuts
{
    /// <summary>
    /// An exception for IO problems with shortcuts.
    /// </summary>
    [Serializable]
    internal class ShortcutIOException : Exception
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ShortcutIOException"/> class.
        /// </summary>
        public ShortcutIOException()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ShortcutIOException"/> class.
        /// </summary>
        public ShortcutIOException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ShortcutIOException"/> class.
        /// </summary>
        public ShortcutIOException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ShortcutIOException"/> class.
        /// </summary>
        protected ShortcutIOException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}
