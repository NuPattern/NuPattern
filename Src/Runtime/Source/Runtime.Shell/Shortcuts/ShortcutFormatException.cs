using System;
using System.Runtime.Serialization;

namespace NuPattern.Runtime.Shell.Shortcuts
{
    /// <summary>
    /// An exception for formatting problems with shortcuts.
    /// </summary>
    [Serializable]
    internal class ShortcutFormatException : Exception
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ShortcutFormatException"/> class.
        /// </summary>
        public ShortcutFormatException()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ShortcutFormatException"/> class.
        /// </summary>
        public ShortcutFormatException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ShortcutFormatException"/> class.
        /// </summary>
        public ShortcutFormatException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ShortcutFormatException"/> class.
        /// </summary>
        protected ShortcutFormatException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}
