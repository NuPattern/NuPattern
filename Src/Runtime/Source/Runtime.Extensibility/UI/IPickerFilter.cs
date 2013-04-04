using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;

namespace NuPattern.Runtime.UI
{
    /// <summary>
    /// Defines a filter for the picker controls.
    /// </summary>
    /// <remarks>
    /// To pass through the filter, items must match the Kind.
    /// If any match criteria is defined (i.e. MatchFileExtensions or MatchItems have a value) then items must match that criteria to pass through the filter.
    /// </remarks>
    [CLSCompliant(false)]
    public interface IPickerFilter
    {
        /// <summary>
        /// Determines whether the item is allowed through the filter.
        /// </summary>
        bool ApplyFilter(IItemContainer item);

        /// <summary>
        /// Determines whether the item matches the filter.
        /// </summary>
        bool MatchesFilter(IItemContainer item);

        /// <summary>
        /// The kinds of items to allow through the filter.
        /// </summary>
        ItemKind Kind { get; set; }

        /// <summary>
        /// Gets or sets the file extensions to match.
        /// </summary>
        string MatchFileExtensions { get; set; }

        /// <summary>
        /// Gets or sets specific items to match.
        /// </summary>
        IEnumerable<IItemContainer> MatchItems { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to include empty containers that do not contain any matching items.
        /// </summary>
        bool IncludeEmptyContainers { get; set; }
    }
}