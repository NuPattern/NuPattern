using System;
using System.Runtime.Serialization;

namespace NuPattern.Runtime.Shell.Shortcuts
{
    /// <summary>
    /// An exception for missing providers of shortcuts.
    /// </summary>
    [Serializable]
    internal class ShortcutProviderNotExistException : Exception
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ShortcutProviderNotExistException"/> class.
        /// </summary>
        public ShortcutProviderNotExistException()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ShortcutProviderNotExistException"/> class.
        /// </summary>
        public ShortcutProviderNotExistException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ShortcutProviderNotExistException"/> class.
        /// </summary>
        public ShortcutProviderNotExistException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ShortcutProviderNotExistException"/> class.
        /// </summary>
        protected ShortcutProviderNotExistException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}
