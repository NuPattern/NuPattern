namespace NuPattern.VisualStudio.Solution
{
    /// <summary>
    /// A project within a solution.
    /// </summary>
    public interface IProject : IItemContainer, IFolderContainer, IDataContainer
    {
        /// <summary>
        /// Arbitrary user data associated with the project, using dynamic property syntax 
        /// (i.e. solution.Data.MyValue = "Hello"). If no specific 
        /// <see cref="ProjectConfiguration"/> or <see cref="ProjectPlatform"/> is provided, 
        /// retrieves the first value found with the given name in any Configuration|Platform found.
        /// </summary>
        dynamic UserData { get; }
    }
}
