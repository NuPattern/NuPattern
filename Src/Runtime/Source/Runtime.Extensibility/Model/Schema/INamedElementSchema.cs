
namespace NuPattern.Runtime
{
    /// <summary>
    /// Core interface implemented by all schema elements.
    /// </summary>
    public partial interface INamedElementSchema : ISupportTransaction
    {
        /// <summary>
        /// Gets the parent of the current schema element, or <see langword="null"/> if 
        /// it's the root pattern schema.
        /// </summary>
        INamedElementSchema Parent
        {
            get;
        }

        /// <summary>
        /// Returns the path of the item in teh current schema.
        /// </summary>
        /// <returns></returns>
        string GetSchemaPathValue();

		/// <summary>
		/// Gets the root pattern ancestor for this instance. Note that for a pattern, 
		/// this may be an ancestor pattern if it has been instantiated as an 
		/// extension point.
		/// </summary>
		/// <remarks>The returned value may be null if the element is not rooted in any pattern.</remarks>
		IPatternSchema Root { get; }
    }
}
