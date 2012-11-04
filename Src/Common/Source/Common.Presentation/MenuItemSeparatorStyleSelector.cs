using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Microsoft.VisualStudio.Patterning.Common.Presentation
{
	/// <summary>
	/// Defines a separators for each group in a bound <see cref="ContextMenu"/>
	/// </summary>
	public class MenuItemSeparatorStyleSelector : DataTemplateSelector
	{
		/// <summary>
		/// Gets or sets the empty template.
		/// </summary>
		/// <value>The empty template.</value>
		public DataTemplate EmptyTemplate { get; set; }

		/// <summary>
		/// Gets or sets the separator template.
		/// </summary>
		/// <value>The separator template.</value>
		public DataTemplate SeparatorTemplate { get; set; }

		/// <summary>
		/// When overridden in a derived class, returns a <see cref="DataTemplate"/> based on custom logic.
		/// </summary>
		/// <param name="item">The data object for which to select the template.</param>
		/// <param name="container">The data-bound object.</param>
		/// <returns>
		/// Returns a <see cref="DataTemplate"/> or null. The default value is null.
		/// </returns>
		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			var group = item as CollectionViewGroup;
			if (group != null && group.IsBottomLevel)
			{
				var itemsControl = container.FindAncestor<ItemsControl>();
				if (itemsControl != null)
				{
					var collectionView = itemsControl.Items as ICollectionView;
					if (collectionView != null && collectionView.Groups.Count > 0 && !group.Equals(collectionView.Groups[0]))
					{
						return this.SeparatorTemplate;
					}
				}
			}

			return this.EmptyTemplate;
		}
	}
}