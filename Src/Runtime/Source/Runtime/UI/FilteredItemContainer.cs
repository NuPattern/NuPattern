using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Media.Imaging;
using Microsoft.VisualStudio.Patterning.Common.Presentation.Data;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;

namespace Microsoft.VisualStudio.Patterning.Runtime.UI
{
	/// <summary>
	/// Wraps an <see cref="IItemContainer"/> to provide filtering.
	/// </summary>
	[CLSCompliant(false)]
	public class FilteredItemContainer
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FilteredItemContainer"/> class.
		/// </summary>
		/// <param name="item">The wraped item.</param>
		/// <param name="filter">The filter to apply to this item children.</param>
		public FilteredItemContainer(IItemContainer item, IPickerFilter filter)
		{
			Guard.NotNull(() => item, item);
			Guard.NotNull(() => filter, filter);

			this.Item = item;
			this.Items = item.Items.Where(i => filter.ApplyFilter(i)).Select(i => new FilteredItemContainer(i, filter)).ToArray();
		}

		private BitmapSource icon;

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
	}
}