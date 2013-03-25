using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using NuPattern.Extensibility;
using NuPattern.Runtime.UI;

namespace NuPattern.Runtime
{
    /// <summary>
    /// Extensions to the <see cref="FilteredItemContainer"/> class.
    /// </summary>
    internal static class FilteredItemContainerExtensions
    {
        /// <summary>
        /// Expands all items in the hierarchy
        /// </summary>
        public static void ExpandAll(this FilteredItemContainer container)
        {
            new[] { container }.ExpandAll();
        }

        /// <summary>
        /// Expands all items in the hierarchy
        /// </summary>
        public static void ExpandAll(this IEnumerable<FilteredItemContainer> containers)
        {
            Guard.NotNull(() => containers, containers);

            // Need to traverse to leaves of tree only (IsExpanded cascades up)
            containers.Traverse(item => item.Items)
                .ForEach(item =>
                {
                    if (!item.Items.Any())
                    {
                        item.IsExpanded = true;
                    }
                });

        }

        /// <summary>
        /// Collapses all items in the hierarchy
        /// </summary>
        public static void CollapseAll(this IEnumerable<FilteredItemContainer> containers)
        {
            Guard.NotNull(() => containers, containers);

            // Need to traverse to expand all items
            containers.Traverse(item => item.Items).ForEach(item => item.IsExpanded = false);
        }

        /// <summary>
        /// Selects/deselects all items in the hierarchy.
        /// </summary>
        public static void CheckAll(this IEnumerable<FilteredItemContainer> containers, bool isSelected)
        {
            Guard.NotNull(() => containers, containers);

            // Only need to do top level items (IsChecked cascades down)
            containers.ForEach(it => it.IsChecked = isSelected);
        }
    }
}
