using System.Collections.Generic;

namespace NuPattern.VisualStudio.Solution
{
    /// <summary>
    /// A folder within a project or another folder.
    /// </summary>
    public interface IFolderContainer
    {
        /// <summary>
        /// Creates a nested folder with the given name.
        /// </summary>
        /// <param name="name">Name of the folder to create.</param>
        /// <returns>The newly created folder.</returns>
        IFolder CreateFolder(string name);

        /// <summary>
        /// returns the list of all folders in this container
        /// </summary>
        IEnumerable<IFolder> Folders { get; }
    }
}
