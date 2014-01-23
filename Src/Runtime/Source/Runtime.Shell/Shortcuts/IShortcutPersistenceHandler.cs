
using NuPattern.Runtime.Shortcuts;

namespace NuPattern.Runtime.Shell.Shortcuts
{
    /// <summary>
    /// Defines a handler for reading and writing shortcurt to a persistence store
    /// </summary>
    internal interface IShortcutPersistenceHandler
    {
        /// <summary>
        /// Returns the shortcut from the persistence store
        /// </summary>
        IShortcut ReadShortcut();

        /// <summary>
        /// Writes the shortcut to the persistence store.
        /// </summary>
        /// <param name="shortcut">The shortcut to write to the store</param>
        void WriteShortcut(IShortcut shortcut);
    }
}
