using System;
using System.Runtime.Serialization;

namespace NuPattern.Runtime.Shell.Shortcuts
{
    /// <summary>
    /// An exception for file formatting problems with shortcuts.
    /// </summary>
    [Serializable]
    internal class ShortcutFileFormatException : Exception
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ShortcutFileFormatException"/> class.
        /// </summary>
        public ShortcutFileFormatException()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ShortcutFileFormatException"/> class.
        /// </summary>
        public ShortcutFileFormatException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ShortcutFileFormatException"/> class.
        /// </summary>
        public ShortcutFileFormatException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ShortcutFileFormatException"/> class.
        /// </summary>
        protected ShortcutFileFormatException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}
