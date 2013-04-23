namespace NuPattern.VisualStudio.Solution
{
    /// <summary>
    /// An item that has a data collection.
    /// </summary>
    public interface IDataContainer
    {
        /// <summary>
        /// Arbitrary data associated with the item, using dynamic property syntax 
        /// (i.e. solution.Data.MyValue = "Hello").
        /// </summary>
        dynamic Data { get; }
    }
}
