using System;
using System.Collections.Generic;

namespace NuPattern.Runtime
{
	/// <summary>
	/// Provides data when products are added or removed in the <see cref="IPatternManager"/>.
	/// </summary>
	public class ProductCollectionChangedEventArgs : EventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ProductCollectionChangedEventArgs"/> class.
		/// </summary>
		/// <param name="addedItems">The added items.</param>
		/// <param name="removedItems">The removed items.</param>
		public ProductCollectionChangedEventArgs(IEnumerable<IProduct> addedItems, IEnumerable<IProduct> removedItems)
		{
			Guard.NotNull(() => addedItems, addedItems);
			Guard.NotNull(() => removedItems, removedItems);

			this.AddedItems = addedItems;
			this.RemovedItems = removedItems;
		}

		/// <summary>
		/// Gets the added items.
		/// </summary>
		public IEnumerable<IProduct> AddedItems { get; private set; }

		/// <summary>
		/// Gets the removed items.
		/// </summary>
		public IEnumerable<IProduct> RemovedItems { get; private set; }
	}
}