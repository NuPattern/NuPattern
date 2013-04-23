namespace NuPattern.VisualStudio.Solution
{
    /// <summary>
    /// A solution folder in the solution.
    /// </summary>
    public interface ISolutionFolder : IItemContainer, ISolutionFolderContainer
    {
        // intentionally left blank, all methods come from the inherited interfaces. 
        // This interface is a perfect candidate for extension methods.               
    }
}
