using System;
using System.Windows;
using NuPattern.VisualStudio.Solution;

namespace NuPattern.Runtime.UI
{
    /// <summary>
    /// Defines a picker used for selecting a single solution artifact from a filtered subset of solution artifacts.
    /// </summary>
    [CLSCompliant(false)]
    public interface ISolutionPicker
    {
        /// <summary>
        /// Gets or sets the owner window of the picker.
        /// </summary>
        Window Owner { get; set; }

        /// <summary>
        /// Gets or sets the root item to pick from.
        /// </summary>
        ISolution RootItem { get; set; }

        /// <summary>
        /// The title of the picker.
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// The message to show when no items can be found to pick.
        /// </summary>
        string EmptyItemsMessage { get; set; }

        /// <summary>
        /// Gets or sets the filter for the picker.
        /// </summary>
        IPickerFilter Filter { get; }

        /// <summary>
        /// Displays the dialog.
        /// </summary>
        /// <returns></returns>
        bool ShowDialog();

        /// <summary>
        /// Gets or sets the currently selected item.
        /// </summary>
        IItemContainer SelectedItem { get; set; }
    }
}