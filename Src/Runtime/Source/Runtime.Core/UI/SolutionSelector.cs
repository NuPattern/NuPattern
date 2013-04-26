using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using NuPattern.Diagnostics;
using NuPattern.Runtime.Properties;
using NuPattern.Runtime.UI.ViewModels;
using NuPattern.Runtime.UI.Views;
using NuPattern.VisualStudio;
using NuPattern.VisualStudio.Solution;

namespace NuPattern.Runtime.UI
{
    /// <summary>
    /// A selector that displays solution items
    /// </summary>
    internal class SolutionSelector : ISolutionSelector
    {
        private static readonly ITracer tracer = Tracer.Get<SolutionSelector>();

        /// <summary>
        /// Creates a new instance of the <see cref="SolutionSelector"/> class.
        /// </summary>
        public SolutionSelector()
        {
            this.Filter = new PickerFilter();
            this.ShowAllExpanded = false;
        }

        /// <summary>
        /// Gets or sets the owner window of the selector.
        /// </summary>
        public Window Owner { get; set; }

        /// <summary>
        /// Gets or sets the root item to select from.
        /// </summary>
        public IItemContainer RootItem { get; set; }

        /// <summary>
        /// Gets or sets the title of the selector.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets an optional message to display to the user of the selector.
        /// </summary>
        public string UserMessage { get; set; }

        /// <summary>
        /// Gets or Sets the message for when there are no filtered items found in solution.
        /// </summary>
        public string EmptyItemsMessage { get; set; }

        /// <summary>
        /// Gets or sets the filter for the items in the selector.
        /// </summary>
        public IPickerFilter Filter { get; private set; }

        /// <summary>
        /// Displays the picker.
        /// </summary>
        public bool ShowDialog()
        {
            if (this.RootItem == null)
            {
                throw new InvalidOperationException(Resources.SolutionSelector_ErrorNoRootItem);
            }

            // Initialize the view
            var selector = new SolutionSelectorView();
            selector.Owner = this.Owner;
            if (!string.IsNullOrEmpty(this.Title))
            {
                selector.Title = this.Title;
            }

            // Initialize the model
            var root = new FilteredItemContainer(this.RootItem, this.Filter);
            if (this.ShowAllExpanded)
            {
                root.ExpandAll();
            }
            var model = new SolutionSelectorViewModel(root, this.Filter);
            model.UserMessage = this.UserMessage;
            model.EmptyItemsMessage = this.EmptyItemsMessage;

            //Bind the model and view
            selector.DataContext = model;

            // Display the view
            var result = false;
            tracer.ShieldUI(() =>
                {
                    result = selector.ShowDialog().GetValueOrDefault();
                    if (result)
                    {
                        if (model.GetSelectedItems() == null || !model.GetSelectedItems().Any())
                        {
                            result = false;
                        }
                        else
                        {
                            this.SelectedItems = model.GetSelectedItems().Select(i => i.Item);
                        }
                    }

                }, Resources.SolutionSelector_ErrorFailedDialog);

            return result;
        }

        /// <summary>
        /// The selected items from the selector.
        /// </summary>
        public IEnumerable<IItemContainer> SelectedItems { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show all items expanded.
        /// </summary>
        public bool ShowAllExpanded { get; set; }
    }
}
