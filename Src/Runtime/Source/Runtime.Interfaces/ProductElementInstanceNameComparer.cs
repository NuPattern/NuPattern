
namespace Microsoft.VisualStudio.Patterning.Runtime
{
    /// <summary>
    /// Compares the <see cref="IProductElement.InstanceName"/> of two elements alphabetically.
    /// </summary>
    internal class ProductElementInstanceNameComparer : ProductElementComparer
    {
        /// <summary>
        /// Compares the <see cref="IProductElement.InstanceName"/> of two elements.
        /// </summary>
        /// <param name="x">The first element</param>
        /// <param name="y">The second element</param>
        /// <returns>A value that determines if the first element is less than, greater than or equal to the second element</returns>
        public override int Compare(IProductElement x, IProductElement y)
        {
            return string.Compare(x.InstanceName, y.InstanceName, System.StringComparison.InvariantCulture);
        }
    }
}
