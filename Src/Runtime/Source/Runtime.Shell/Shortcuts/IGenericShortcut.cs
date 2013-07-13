
using System.Collections.Generic;

namespace NuPattern.Runtime.Shell.Shortcuts
{
    /// <summary>
    /// Defines a generic (all purpose) shortcut.
    /// </summary>
    internal interface IGenericShortcut
    {
        /// <summary>
        /// Executes the shortcut
        /// </summary>
        void Execute();

        /// <summary>
        /// Gets a value which determines whether the shortcut has been updated after being executed.
        /// </summary>
        bool Upated { get; }

        /// <summary>
        /// Gets the type of the shortcut.
        /// </summary>
        string Type { get; }

        /// <summary>
        /// Gets the description of the shortcut.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets the parameters of the shortcut
        /// </summary>
        IDictionary<string, string> Parameters { get; }
    }
}
