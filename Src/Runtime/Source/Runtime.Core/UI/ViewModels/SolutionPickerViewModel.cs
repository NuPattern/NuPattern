using System.Linq;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using NuPattern.Presentation;

namespace NuPattern.Runtime.UI.ViewModels
{
    /// <summary>
    /// Provides a view model for the solution picker
    /// </summary>
    internal class SolutionPickerViewModel : SolutionPickerBaseViewModel
    {
        /// <summary>
        /// Creates a new instance of the <see cref="SolutionPickerViewModel"/> class.
        /// </summary>
        public SolutionPickerViewModel(FilteredItemContainer root, IPickerFilter filter)
            : base(root, filter)
        {
        }

        /// <summary>
        /// Gets the expandall command.
        /// </summary>
        public System.Windows.Input.ICommand ExpandAllCommand { get; private set; }

        /// <summary>
        /// Gets the collapseall command.
        /// </summary>
        public System.Windows.Input.ICommand CollapseAllCommand { get; private set; }

        /// <summary>
        /// Initializes the commands for the view
        /// </summary>
        protected override void InitializeCommands()
        {
            base.InitializeCommands();
            this.ExpandAllCommand = new RelayCommand(ExpandAllItems);
            this.CollapseAllCommand = new RelayCommand(CollapseAllItems);
        }

        private void ExpandAllItems()
        {
            this.Items.ExpandAll();
        }

        private void CollapseAllItems()
        {
            this.Items.CollapseAll();
        }

        /// <summary>
        /// Whether the dialog can be closed
        /// </summary>
        protected override bool CanCloseDialog(IDialogWindow dialog)
        {
            // Ensure an item is selected
            var selectedItem = GetSelectedItem();
            if (selectedItem == null)
            {
                return false;
            }

            return selectedItem.IsSelectable;
        }

        /// <summary>
        /// Gets the selected item
        /// </summary>
        internal FilteredItemContainer GetSelectedItem()
        {
            // Find first selected item in hierarchy
            return this.Items.Traverse(item => item.Items)
                .FirstOrDefault(subItem => subItem.IsSelected);
        }

        /// <summary>
        /// Sets the selected item
        /// </summary>
        /// <param name="selectedItem">Item to select</param>
        internal void SetSelectedItem(IItemContainer selectedItem)
        {
            if (selectedItem != null)
            {
                // Find the item in the hierarchy
                var filteredItem = this.Items.Traverse(item => item.Items)
                    .FirstOrDefault(subItem => subItem.Item.Equals(selectedItem));
                if (filteredItem != null)
                {
                    filteredItem.IsSelected = true;
                    filteredItem.IsExpanded = true;
                }
            }
        }
    }
}
