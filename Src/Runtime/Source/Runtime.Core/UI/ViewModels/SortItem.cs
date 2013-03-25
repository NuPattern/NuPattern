using System.ComponentModel;

namespace NuPattern.Runtime.UI
{
    /// <summary>
    /// Represents an item used for setting a sorting.
    /// </summary>
    internal class SortItem
    {
        /// <summary>
        /// Creates a new instance of the <see cref="SortItem"/> class.
        /// </summary>
        public SortItem(string displayText, string sortPropertyExpression, ListSortDirection direction)
        {
            Guard.NotNullOrEmpty(() => displayText, displayText);
            Guard.NotNullOrEmpty(() => sortPropertyExpression, sortPropertyExpression);
            Guard.NotNull(() => direction, direction);

            this.DisplayText = displayText;
            this.SortPropertyExpression = sortPropertyExpression;
            this.Direction = direction;
        }

        /// <summary>
        /// The text to display
        /// </summary>
        public string DisplayText { get; set; }

        /// <summary>
        /// The expression to sort by.
        /// </summary>
        public string SortPropertyExpression { get; set; }

        /// <summary>
        /// The directionof the sort.
        /// </summary>
        public ListSortDirection Direction { get; set; }

        /// <summary>
        /// The sring representation of the item.
        /// </summary>
        public override string ToString()
        {
            return this.DisplayText;
        }
    }

}
