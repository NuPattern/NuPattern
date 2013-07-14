using System;
using System.Runtime.Serialization;

namespace NuPattern.Runtime.Shell.Shortcuts
{
    /// <summary>
    /// An exception for IO access problems with shortcuts.
    /// </summary>
    [Serializable]
    internal class ShortcutFileAccessException : Exception
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ShortcutFileAccessException"/> class.
        /// </summary>
        public ShortcutFileAccessException()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ShortcutFileAccessException"/> class.
        /// </summary>
        public ShortcutFileAccessException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ShortcutFileAccessException"/> class.
        /// </summary>
        public ShortcutFileAccessException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ShortcutFileAccessException"/> class.
        /// </summary>
        protected ShortcutFileAccessException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}
