using System;
using System.Collections.Generic;
using System.Windows;
using NuPattern.VisualStudio.Solution;

namespace NuPattern.Runtime.UI
{
    /// <summary>
    /// Defines a selector used for selecting multiple solution artifacts from a filtered subset of solution artifacts.
    /// </summary>
    [CLSCompliant(false)]
    public interface ISolutionSelector
    {
        /// <summary>
        /// Gets or sets the owner window of the selector.
        /// </summary>
        Window Owner { get; set; }

        /// <summary>
        /// Gets or sets the root item to select from.
        /// </summary>
        IItemContainer RootItem { get; set; }

        /// <summary>
        /// The title of the selector.
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Gets or sets an optional message to display to the user of the selector.
        /// </summary>
        string UserMessage { get; set; }

        /// <summary>
        /// The message to show when no items can be found to select.
        /// </summary>
        string EmptyItemsMessage { get; set; }

        /// <summary>
        /// Gets or sets the filter for the selector.
        /// </summary>
        IPickerFilter Filter { get; }

        /// <summary>
        /// Displays the dialog.
        /// </summary>
        /// <returns></returns>
        bool ShowDialog();

        /// <summary>
        /// Gets or sets the currently selected items
        /// </summary>
        IEnumerable<IItemContainer> SelectedItems { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to expand all items when shown.
        /// </summary>
        bool ShowAllExpanded { get; set; }
    }
}