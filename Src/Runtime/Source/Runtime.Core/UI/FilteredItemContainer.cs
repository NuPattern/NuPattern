using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Media.Imaging;
using NuPattern.Presentation.Data;
using NuPattern.Reflection;
using NuPattern.VisualStudio.Solution;

namespace NuPattern.Runtime.UI
{
    /// <summary>
    /// Wraps an <see cref="IItemContainer"/> to provide filtering.
    /// </summary>
    internal class FilteredItemContainer : INotifyPropertyChanged
    {
        private IPickerFilter filter;
        private BitmapSource icon;
        private bool isSelected;
        private bool isChecked;
        private bool isExpanded;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilteredItemContainer"/> class.
        /// </summary>
        /// <param name="item">The wrapped item.</param>
        /// <param name="filter">The filter to apply to the children of the item.</param>
        public FilteredItemContainer(IItemContainer item, IPickerFilter filter) : this(null, item, filter) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilteredItemContainer"/> class.
        /// </summary>
        /// <param name="parent">The parent item.</param>
        /// <param name="item">The wrapped item.</param>
        /// <param name="filter">The filter to apply to the children of the item.</param>
        private FilteredItemContainer(FilteredItemContainer parent, IItemContainer item, IPickerFilter filter)
        {
            Guard.NotNull(() => item, item);
            Guard.NotNull(() => filter, filter);

            this.filter = filter;
            this.Item = item;
            this.Parent = parent;
            this.Items = item.Items.Where(filter.ApplyFilter).Select(i => new FilteredItemContainer(this, i, filter)).ToArray();
        }

        /// <summary>
        /// Notifies that a property value has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the parent element of this element.
        /// </summary>
        public FilteredItemContainer Parent { get; private set; }

        /// <summary>
        /// The icon for the item.
        /// </summary>
        public BitmapSource Icon
        {
            get
            {
                if (this.icon == null)
                {
                    this.icon = new IconToBitmapSourceConverter().Convert(this.Item.Icon, typeof(BitmapSource), null, CultureInfo.CurrentCulture) as BitmapSource;
                }

                return this.icon;
            }
        }

        /// <summary>
        /// Gets the item.
        /// </summary>
        public IItemContainer Item { get; private set; }

        /// <summary>
        /// Gets the filtered children of this item.
        /// </summary>
        public IEnumerable<FilteredItemContainer> Items { get; private set; }

        /// <summary>
        /// Gets or sets whether the item is selected.
        /// </summary>
        public bool IsSelected
        {
            get { return this.isSelected; }
            set
            {
                if (value != this.isSelected)
                {
                    this.isSelected = value;
                    this.OnPropertyChanged(Reflector<FilteredItemContainer>.GetPropertyName(p => p.IsSelected));
                }
            }
        }

        /// <summary>
        /// Gets or sets whether the item is expanded.
        /// </summary>
        public bool IsExpanded
        {
            get { return this.isExpanded; }
            set
            {
                if (value != this.isExpanded)
                {
                    this.isExpanded = value;
                    this.OnPropertyChanged(Reflector<FilteredItemContainer>.GetPropertyName(p => p.IsExpanded));
                }

                // Expand ancestory
                if (this.isExpanded)
                {
                    if (this.Parent != null)
                    {
                        this.Parent.IsExpanded = true;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets whether the item is selectable
        /// </summary>
        /// <remarks>
        /// Whether the item is filtered because it matches the filter criteria, or whether it is a ancestor of a filtered item.
        /// </remarks>
        public bool IsSelectable
        {
            get
            {
                return this.filter.MatchesFilter(this.Item);
            }
        }

        /// <summary>
        /// Gets or sets whether the item is checked.
        /// </summary>
        public bool IsChecked
        {
            get { return this.isChecked; }
            set
            {
                if (value != this.isChecked)
                {
                    this.isChecked = value;
                    this.OnPropertyChanged(Reflector<FilteredItemContainer>.GetPropertyName(p => p.IsChecked));
                }

                // Check children
                if (this.Items != null)
                {
                    this.Items.ForEach(it => it.IsChecked = value);
                }
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Toggles the items check state
        /// </summary>
        public void ToggleCheck()
        {
            this.IsChecked = !this.IsChecked;
        }
    }
}