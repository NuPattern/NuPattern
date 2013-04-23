using System.Collections.Generic;

namespace NuPattern.VisualStudio.Solution
{
    /// <summary>
    /// A solution folder in the solution.
    /// </summary>
    public interface ISolutionFolderContainer
    {
        /// <summary>
        /// Creates a nested solution folder with the given name.
        /// </summary>
        /// <param name="name">Name of the solution folder to create.</param>
        /// <returns>The created solution folder.</returns>
        ISolutionFolder CreateSolutionFolder(string name);

        /// <summary>
        /// returns the list of all solution folders in this container
        /// </summary>
        IEnumerable<ISolutionFolder> SolutionFolders { get; }
    }
}
