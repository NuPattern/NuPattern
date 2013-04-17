using System.Runtime.InteropServices;

namespace NuPattern.VisualStudio.Solution
{
    /// <summary>
    /// A Visual Studio solution.
    /// </summary>
    [Guid("3635AA23-6A99-4892-A906-B2AC610CDE51")]
    public interface ISolution : ISolutionFolderContainer, IItemContainer, IDataContainer
    {
        /// <summary>
        /// Whether the solution is opened. In Visual Studio, 
        /// there is always a solution object, but it may 
        /// not be open, meaning there's no solution, really.
        /// </summary>
        bool IsOpen { get; }
    }
}
