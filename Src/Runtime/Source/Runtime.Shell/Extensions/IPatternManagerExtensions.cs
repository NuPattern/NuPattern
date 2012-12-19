using System;
using System.Linq;

namespace NuPattern.Runtime.Shell
{
    /// <summary>
    /// Define extension methods related to <see cref="IPatternManagerExtensions"/>.
    /// </summary>
    [CLSCompliant(false)]
    public static class IPatternManagerExtensions
    {
        /// <summary>
        /// Deletes all products.
        /// </summary>
        /// <param name="patternManager">The pattern manager.</param>
        public static void DeleteAll(this IPatternManager patternManager)
        {
            Guard.NotNull(() => patternManager, patternManager);

            var productsToDelete = patternManager.Products.ToList();

            foreach (var product in productsToDelete)
            {
                patternManager.DeleteProduct(product);
            }
        }
    }
}