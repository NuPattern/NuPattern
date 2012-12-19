
namespace NuPattern.Runtime
{
    /// <summary>
    /// Core interface implemented by all schema information elements.
    /// </summary>
    public partial interface INamedElementInfo : ISupportTransaction
    {
        /// <summary>
        /// Gets the parent of the current schema information element, or <see langword="null"/> if 
        /// it's the root pattern schema.
        /// </summary>
        INamedElementInfo Parent { get; }
    }
}
