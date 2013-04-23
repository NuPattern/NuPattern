namespace NuPattern.VisualStudio.Solution
{
    /// <summary>
    /// A folder within a project or another folder.
    /// </summary>
    public interface IFolder : IItemContainer, IFolderContainer
    {
        // intentionally left blank, all methods come from the inherited interfaces. 
        // This interface is a perfect candidate for extension methods.               
    }
}
