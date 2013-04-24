using System;
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
    /// A picker that displays solution items
    /// </summary>
    internal class SolutionPicker : ISolutionPicker
    {
        private static readonly ITracer tracer = Tracer.Get<SolutionPicker>();

        /// <summary>
        /// Creates a new instance of the <see cref="SolutionPicker"/> class.
        /// </summary>
        public SolutionPicker()
        {
            this.Filter = new PickerFilter();
        }

        /// <summary>
        /// Gets or sets the owner window of the picker.
        /// </summary>
        public Window Owner { get; set; }

        /// <summary>
        /// Gets or sets the root item to pick from.
        /// </summary>
        public ISolution RootItem { get; set; }

        /// <summary>
        /// Gets or sets the title of the picker.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the filter for the items in the picker.
        /// </summary>
        public IPickerFilter Filter { get; private set; }

        /// <summary>
        /// Gets or Sets the message for when there are no filtered items found in solution.
        /// </summary>
        public string EmptyItemsMessage { get; set; }

        /// <summary>
        /// Displays the picker.
        /// </summary>
        public bool ShowDialog()
        {
            if (this.RootItem == null)
            {
                throw new InvalidOperationException(Resources.SolutionPicker_ErrorNoRootItem);
            }

            // Initialize the view
            var picker = new SolutionPickerView();
            picker.Owner = this.Owner;
            if (!string.IsNullOrEmpty(this.Title))
            {
                picker.Title = this.Title;
            }

            // Initialize the model
            var root = new FilteredItemContainer(this.RootItem, this.Filter);
            var model = new SolutionPickerViewModel(root, this.Filter);
            model.EmptyItemsMessage = this.EmptyItemsMessage;
            model.SetSelectedItem(this.SelectedItem);

            //Bind the model and view
            picker.DataContext = model;

            var result = false;
            tracer.ShieldUI(() =>
                {
                    // Display the view
                    result = picker.ShowDialog().GetValueOrDefault();
                    if (result)
                    {
                        if (model.GetSelectedItem() == null)
                        {
                            result = false;
                        }
                        else
                        {
                            this.SelectedItem = model.GetSelectedItem().Item;
                        }
                    }

                }, Resources.SolutionPicker_ErrorFailedDialog);

            return result;
        }

        /// <summary>
        /// The selected item from the picker.
        /// </summary>
        public IItemContainer SelectedItem { get; set; }
    }
}
