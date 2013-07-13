
using System.Collections.Generic;

namespace NuPattern.Runtime.Shortcuts
{
    /// <summary>
    /// Defines a shortcut.
    /// </summary>
    public interface IShortcut
    {
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
