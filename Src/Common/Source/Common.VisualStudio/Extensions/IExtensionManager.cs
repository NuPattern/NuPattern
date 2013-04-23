using System.Collections.Generic;

namespace NuPattern.VisualStudio.Extensions
{
    /// <summary>
    /// Defines a manager of VSIX extensions
    /// </summary>
    public interface IExtensionManager
    {
        /// <summary>
        /// Creates an in-memory representation of an extension from a folder on disk.
        /// </summary>
        /// <param name="extensionPath">The path to the extension definintion on disk</param>
        /// <returns></returns>
        IExtension CreateExtension(string extensionPath);

        /// <summary>
        /// Gets the enabled extensions
        /// </summary>
        /// <returns></returns>
        IEnumerable<IInstalledExtension> GetEnabledExtensions();

        /// <summary>
        /// Gets the installed extension with given identifier
        /// </summary>
        /// <param name="identifier">Identifier of the extension</param>
        /// <returns></returns>
        IInstalledExtension GetInstalledExtension(string identifier);

        /// <summary>
        /// Gets the installed extensions.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IInstalledExtension> GetInstalledExtensions();
    }
}
