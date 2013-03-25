using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using NuPattern.Presentation;

namespace NuPattern.Runtime.UI.ViewModels
{
    /// <summary>
    /// Provides a view model for the solution selector
    /// </summary>
    internal class SolutionSelectorViewModel : SolutionPickerBaseViewModel
    {
        /// <summary>
        /// Creates a new instance of the <see cref="SolutionSelectorViewModel"/> class.
        /// </summary>
        public SolutionSelectorViewModel(FilteredItemContainer root, IPickerFilter filter)
            : base(root, filter)
        {
        }

        /// <summary>
        /// Gets the check item command.
        /// </summary>
        public System.Windows.Input.ICommand CheckItemCommand { get; private set; }

        /// <summary>
        /// Gets the selectall command.
        /// </summary>
        public System.Windows.Input.ICommand SelectAllCommand { get; private set; }

        /// <summary>
        /// Gets the deselectall command.
        /// </summary>
        public System.Windows.Input.ICommand DeselectAllCommand { get; private set; }

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
            this.CheckItemCommand = new RelayCommand<FilteredItemContainer>(CheckSelectedItem, CanCheckSelectedItem);
            this.SelectAllCommand = new RelayCommand(this.SelectAllItems);
            this.DeselectAllCommand = new RelayCommand(this.DeselectAllItems);
            this.ExpandAllCommand = new RelayCommand(this.ExpandAllItems);
            this.CollapseAllCommand = new RelayCommand(this.CollapseAllItems);
        }

        /// <summary>
        /// Gets or sets an optional message to display to the user of the selector.
        /// </summary>
        public string UserMessage { get; set; }

        /// <summary>
        /// Whether to display the UserMessage or not. 
        /// </summary>
        public bool IsUserMessage
        {
            get { return !String.IsNullOrEmpty(this.UserMessage); }
        }

        private void DeselectAllItems()
        {
            this.Items.CheckAll(false);
        }

        private void SelectAllItems()
        {
            this.Items.CheckAll(true);
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
            // Ensure at least one item is selected
            var selectedItems = GetSelectedItems();
            if (!selectedItems.Any())
            {
                return false;
            }

            return selectedItems.Any(si => si.IsSelectable);
        }

        private static bool CanCheckSelectedItem(FilteredItemContainer item)
        {
            return (item != null && item.IsSelectable);
        }

        private static void CheckSelectedItem(FilteredItemContainer item)
        {
            if (item != null)
            {
                item.ToggleCheck();
            }
        }

        /// <summary>
        /// Gets the selected items.
        /// </summary>
        internal IEnumerable<FilteredItemContainer> GetSelectedItems()
        {
            return this.Items.Traverse(item => item.Items)
                .Where(subItem => subItem.IsChecked && subItem.IsSelectable);
        }

        /// <summary>
        /// Selects the selected items.
        /// </summary>
        /// <param name="selectedItems">The items to select</param>
        internal void SetSelectedItems(IEnumerable<IItemContainer> selectedItems)
        {
            if (selectedItems != null && selectedItems.Any())
            {
                selectedItems.ForEach(si =>
                    {
                        // Find the item in the hierarchy
                        var filteredItem = this.Items.Traverse(item => item.Items)
                            .FirstOrDefault(subItem => subItem.Item.Equals(si));
                        if (filteredItem != null)
                        {
                            filteredItem.IsChecked = true;
                            filteredItem.IsExpanded = true;
                        }
                    });
            }
        }
    }
}
