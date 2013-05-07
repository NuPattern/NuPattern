
using System.Collections.ObjectModel;

namespace NuPattern.Runtime.UI.ViewModels
{
    /// <summary>
    /// Defines a view model for a context menu
    /// </summary>
    public interface IMenuOptionViewModel
    {
        /// <summary>
        /// Gets the caption of the menu
        /// </summary>
        string Caption { get; }

        /// <summary>
        /// Gets the menu options
        /// </summary>
        ObservableCollection<IMenuOptionViewModel> MenuOptions { get; }

        /// <summary>
        /// Gets a value indicating whether the menu is selected.
        /// </summary>
        bool IsSelected { get; set; }

        /// <summary>
        /// Gets a value indicating whether the menu is visible.
        /// </summary>
        bool IsVisible { get; set; }

        /// <summary>
        /// Gets a value indicating whether the menu is enabled.
        /// </summary>
        bool IsEnabled { get; set; }

        /// <summary>
        /// Gets the data for the menu
        /// </summary>
        object Data { get; set; }
    }
}
