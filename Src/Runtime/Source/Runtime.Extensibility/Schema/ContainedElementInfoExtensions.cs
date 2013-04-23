using System;
using NuPattern.Runtime.Comparers;

namespace NuPattern.Runtime
{
    /// <summary>
    /// Extensions to the <see cref="IContainedElementInfo"/> interface.
    /// </summary>
    public static class ContainedElementInfoExtensions
    {
        /// <summary>
        /// Determines if the default order group comparer is configured for the info.
        /// </summary>
        /// <remarks>
        /// If no comparer is configured, then the default comparer <see cref="ProductElementInstanceNameComparer"/> will be used.
        /// </remarks>
        public static bool IsDefaultOrderComparer(this IContainedElementInfo info)
        {
            return (String.IsNullOrEmpty(info.OrderGroupComparerTypeName)
                || String.Equals(info.OrderGroupComparerTypeName, typeof(ProductElementInstanceNameComparer).FullName, StringComparison.OrdinalIgnoreCase));
        }
    }
}
