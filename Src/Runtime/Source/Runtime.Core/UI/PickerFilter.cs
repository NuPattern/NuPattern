using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;

namespace NuPattern.Runtime.UI
{
    /// <summary>
    /// Represents a filter to be applied to the elements in hierarchy items.
    /// </summary>
    /// <remarks>
    /// Only items that match the Kind pass through the filter.
    /// If any match criteria defined (i.e. MatchFileExtensions or MatchItems have any value) then only items matching that criteria pass through the filter.
    /// Since this filter is applied to a hierarchy, non-matching items are only permitted if they contain descendant items that match.
    /// Empty containers are only permitted when IncludeEmptyContainers=true.
    /// </remarks>
    internal class PickerFilter : IPickerFilter
    {
        internal const ItemKind AllKinds = ItemKind.Folder | ItemKind.Item | ItemKind.Project | ItemKind.Reference |
                ItemKind.ReferencesFolder | ItemKind.Solution | ItemKind.SolutionFolder | ItemKind.Unknown;

        /// <summary>
        /// Initializes a new instance of the <see cref="PickerFilter"/> class.
        /// </summary>
        public PickerFilter()
        {
            this.Kind = AllKinds;
            this.IncludeEmptyContainers = false;
            this.MatchFileExtensions = string.Empty;
            this.MatchItems = Enumerable.Empty<IItemContainer>();
        }

        /// <summary>
        /// Gets or sets the kind of items to match.
        /// </summary>
        public ItemKind Kind { get; set; }

        /// <summary>
        /// Gets or sets the file extensions to match.
        /// </summary>
        /// <value>The list of included file extensions.</value>
        public string MatchFileExtensions { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether include empty containers.
        /// </summary>
        public bool IncludeEmptyContainers { get; set; }

        /// <summary>
        /// Gets or sets specific items to match.
        /// </summary>
        public IEnumerable<IItemContainer> MatchItems { get; set; }

        /// <summary>
        /// Determines whether the item is to be filtered or not.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>Returns <c>true</c> when the item should be included; otherwise <c>false</c>.</returns>
        public bool ApplyFilter(IItemContainer item)
        {
            Guard.NotNull(() => item, item);

            // Permit anything that matches the filters
            if (this.MatchesFilter(item))
            {
                return true;
            }
            else
            {
                // Only permit non-matches if their Kind allowed
                if (IsAllowedKind(item.Kind))
                {
                    // Only permit containing items that have children that match the filter
                    if (HasChildrenItems(item))
                    {
                        return HasAnyMatchingChildren(item);
                    }
                    else
                    {
                        // Permit empty (notional) containers if flag allowed
                        if (IsNotionalContainerKind(item) && this.IncludeEmptyContainers)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether the item matches the filter.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>Returns <c>true</c> when the item matches the filter criteria; otherwise <c>false</c>.</returns>
        public bool MatchesFilter(IItemContainer item)
        {
            Guard.NotNull(() => item, item);

            // Determine if any filters are defined
            if (IsIncludedFilterApplied() || IsFileExtensionFilterApplied())
            {
                // Does item belong in included collection
                if (IsIncludedItem(item))
                {
                    return true;
                }

                // Whether this is an item that matches file extension filter
                if (IsFileExtensionFilterApplied())
                {
                    if (IsItemKind(item))
                    {
                        var physicalPath = item.PhysicalPath;
                        if (!String.IsNullOrEmpty(physicalPath) &&
                            this.MatchFileExtensions.ToUpperInvariant()
                                .Contains(Path.GetExtension(physicalPath).ToUpperInvariant()))
                        {
                            return true;
                        }
                    }
                }
            }
            else
            {
                // Whether this item is an allowable Kind
                return IsAllowedKind(item.Kind);
            }

            return false;
        }

        private bool IsIncludedFilterApplied()
        {
            return (this.MatchItems != null && this.MatchItems.Any());
        }

        private bool IsFileExtensionFilterApplied()
        {
            return (!string.IsNullOrEmpty(this.MatchFileExtensions));
        }

        private static bool IsItemKind(IItemContainer item)
        {
            return item.Kind == ItemKind.Item;
        }

        private static bool IsNotionalContainerKind(IItemContainer item)
        {
            // TODO: Could subtract ItemKind.Unknown, ItemKind.Item from AllKinds
            return (item.Kind != ItemKind.Unknown && item.Kind != ItemKind.Item);
        }

        private bool IsAllowedKind(ItemKind kind)
        {
            return ((kind & this.Kind) == kind);
        }

        private static bool HasChildrenItems(IItemContainer item)
        {
            return item.Items.Any();
        }

        private bool IsIncludedItem(IItemContainer item)
        {
            return IsIncludedFilterApplied() && this.MatchItems.Contains(item);
        }

        private bool HasAnyMatchingChildren(IItemContainer item)
        {
            return item.Items.Any(this.ApplyFilter);
        }
    }
}