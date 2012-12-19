using System;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using NuPattern.Common.Presentation;

namespace NuPattern.Runtime.UI
{
    /// <summary>
    /// Interaction logic for SolutionPickerView.xaml.
    /// </summary>
    [CLSCompliant(false)]
    public partial class SolutionPicker : CommonDialogWindow, IDialogWindow, ISolutionPicker
    {
        /// <summary>
        /// Identifies the <see cref="EmptyItemsMessage"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty EmptyItemsMessageProperty = DependencyProperty.Register(
            "EmptyItemsMessage",
            typeof(string),
            typeof(SolutionPicker),
            new UIPropertyMetadata(null));

        /// <summary>
        /// Initializes a new instance of the <see cref="SolutionPicker"/> class.
        /// </summary>
        public SolutionPicker()
            : base()
        {
            this.InitializeComponent();

            this.Filter = new PickerFilter();
            this.ok.Command = new RelayCommand(this.SelectItem, this.IsCurrentSelectable);
            this.Activated += this.OnActivated;
        }

        /// <summary>
        /// Gets or sets the empty items message.
        /// </summary>
        public string EmptyItemsMessage
        {
            get
            {
                return (string)this.GetValue(EmptyItemsMessageProperty);
            }
            set
            {
                this.SetValue(EmptyItemsMessageProperty, value);
            }
        }

        /// <summary>
        /// Gets the filter used in the items tree.
        /// </summary>
        public IPickerFilter Filter
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the selected item picked from the items tree.
        /// </summary>
        public IItemContainer SelectedItem
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the root item for traversal.
        /// </summary>
        public IItemContainer RootItem
        {
            get;
            set;
        }

        private void OnActivated(object sender, EventArgs e)
        {
            this.Activated -= this.OnActivated;

            if (string.IsNullOrEmpty(this.EmptyItemsMessage))
            {
                this.EmptyItemsMessage = string.Format(CultureInfo.CurrentCulture, Properties.Resources.SolutionPicker_EmptyItemsMessage, this.Filter.IncludeFileExtensions);
            }

            if (this.RootItem != null && this.Filter.ApplyFilter(this.RootItem))
            {
                var root = new FilteredItemContainer(this.RootItem, this.Filter);
                this.solutionItems.ItemsSource = new[] { root };
                this.SetSelectedItem(root);
            }
        }

        private bool IsCurrentSelectable()
        {
            if (!string.IsNullOrEmpty(this.Filter.IncludeFileExtensions))
            {
                return this.solutionItems.SelectedItem != null && ((FilteredItemContainer)this.solutionItems.SelectedItem).Item.Kind == ItemKind.Item;
            }

            return true;
        }

        private void OnItemMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!this.DialogResult.HasValue && this.IsCurrentSelectable())
            {
                this.SelectItem();
            }

            e.Handled = true;
        }

        private void SelectItem()
        {
            this.SelectedItem = ((FilteredItemContainer)this.solutionItems.SelectedItem).Item;
            this.DialogResult = true;
            this.Close();
        }

        private void SetSelectedItem(FilteredItemContainer root)
        {
            //if (this.SelectedItem != null)
            //{
            //    // Search the item in the hierarchy
            //    var selectedItem = root.Traverse(item => item.Items, item => item.Item == this.SelectedItem);
            //    if (selectedItem != null)
            //    {
            //        // TODO check this, maiby we have to search the item hierarchically to preselect the item
            //        var node = (TreeViewItem)this.solutionItems.ItemContainerGenerator.ContainerFromItem(selectedItem);
            //        if (node != null)
            //        {
            //            node.IsSelected = true;
            //        }
            //    }
            //}
        }
    }
}