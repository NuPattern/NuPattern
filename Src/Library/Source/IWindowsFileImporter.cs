using System;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;

namespace NuPattern.Library
{
    /// <summary>
    /// Defines an interface fo a file importer.
    /// </summary>
    [CLSCompliant(false)]
    public interface IWindowsFileImporter
    {
        /// <summary>
        /// Initializes the importer.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Cleans up the importer.
        /// </summary>
        void Cleanup();

        /// <summary>
        /// Adds the given file to the solution.
        /// </summary>
        bool ImportFileToSolution(string filePath);

        /// <summary>
        /// Gets the added item in the solution.
        /// </summary>
        IItemContainer GetItemInSolution(string filePath);

        /// <summary>
        /// Gets the target container for importing files.
        /// </summary>
        IItemContainer TargetContainer { get; }
    }
}
