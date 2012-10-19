using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Microsoft.VisualStudio.Patterning.Common.Presentation
{
	/// <summary>
	/// Allows to define sorting behavior for a <see cref="ListView"/>.
	/// </summary>
	public static class SortBehavior
	{
		/// <summary>
		/// Identifies the <c>CanUserSortColumns</c> dependency property.
		/// </summary>
		public static readonly DependencyProperty CanUserSortColumnsProperty =
			DependencyProperty.RegisterAttached(
				"CanUserSortColumns",
				typeof(bool),
				typeof(SortBehavior),
				new FrameworkPropertyMetadata(OnCanUserSortColumnsChanged));

		/// <summary>
		/// Identifies the <c>CanUseSort</c> dependency property.
		/// </summary>
		public static readonly DependencyProperty CanUseSortProperty =
			DependencyProperty.RegisterAttached(
				"CanUseSort",
				typeof(bool),
				typeof(SortBehavior),
				new FrameworkPropertyMetadata(true));

		/// <summary>
		/// Identifies the <c>SortDirection</c> dependency property.
		/// </summary>
		public static readonly DependencyProperty SortDirectionProperty =
			DependencyProperty.RegisterAttached(
				"SortDirection",
				typeof(ListSortDirection?),
				typeof(SortBehavior));

		/// <summary>
		/// Identifies the <c>SortExpression</c> dependency property.
		/// </summary>
		public static readonly DependencyProperty SortExpressionProperty =
			DependencyProperty.RegisterAttached(
				"SortExpression",
				typeof(string),
				typeof(SortBehavior));

		/// <summary>
		/// Gets whether the user can sort columns or not.
		/// </summary>
		/// <param name="element">The <see cref="ListView"/> element.</param>
		/// <returns>Returns <c>true</c> if the user can sort columns; otherwise <c>false</c>.</returns>
		[AttachedPropertyBrowsableForType(typeof(ListView))]
		[SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Attached Property")]
		[SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Attached Property applicable to ListView")]
		public static bool GetCanUserSortColumns(ListView element)
		{
			return (bool)element.GetValue(CanUserSortColumnsProperty);
		}

		/// <summary>
		/// Sets whether the user can sort columns or not.
		/// </summary>
		/// <param name="element">The <see cref="ListView"/> element.</param>
		/// <param name="value">If set to <c>true</c> the user can sort columns.</param>
		[AttachedPropertyBrowsableForType(typeof(ListView))]
		[SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Attached Property")]
		[SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Attached Property applicable to ListView")]
		public static void SetCanUserSortColumns(ListView element, bool value)
		{
			element.SetValue(CanUserSortColumnsProperty, value);
		}

		/// <summary>
		/// Gets whether the user can sort this column or not.
		/// </summary>
		/// <param name="element">The <see cref="GridViewColumn"/> element.</param>
		/// <returns>Returns <c>true</c> if the user can sort this column; otherwise <c>false</c>.</returns>
		[AttachedPropertyBrowsableForType(typeof(GridViewColumn))]
		[SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Attached Property")]
		[SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Attached Property applicable to GridViewColumn")]
		public static bool GetCanUseSort(GridViewColumn element)
		{
			return (bool)element.GetValue(CanUseSortProperty);
		}

		/// <summary>
		/// Sets  whether the user can sort this column or not.
		/// </summary>
		/// <param name="element">The <see cref="GridViewColumn"/> element.</param>
		/// <param name="value">If set to <c>true</c> the user can sort this column.</param>
		[AttachedPropertyBrowsableForType(typeof(GridViewColumn))]
		[SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Attached Property")]
		[SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Attached Property applicable to GridViewColumn")]
		public static void SetCanUseSort(GridViewColumn element, bool value)
		{
			element.SetValue(CanUseSortProperty, value);
		}

		/// <summary>
		/// Gets the sort direction for the column.
		/// </summary>
		/// <param name="element">The <see cref="GridViewColumn"/> element.</param>
		/// <returns>The direction of the sort.</returns>
		[AttachedPropertyBrowsableForType(typeof(GridViewColumn))]
		[SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Attached Property")]
		[SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Attached Property applicable to GridViewColumn")]
		public static ListSortDirection? GetSortDirection(GridViewColumn element)
		{
			return (ListSortDirection?)element.GetValue(SortDirectionProperty);
		}

		/// <summary>
		/// Sets the sort direction for the column.
		/// </summary>
		/// <param name="element">The <see cref="GridViewColumn"/> element.</param>
		/// <param name="value">The direction of the sort.</param>
		[AttachedPropertyBrowsableForType(typeof(GridViewColumn))]
		[SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Attached Property")]
		[SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Attached Property applicable to GridViewColumn")]
		public static void SetSortDirection(GridViewColumn element, ListSortDirection? value)
		{
			element.SetValue(SortDirectionProperty, value);
		}

		/// <summary>
		/// Gets the sort expression for the column.
		/// </summary>
		/// <param name="element">The <see cref="GridViewColumn"/> element.</param>
		/// <returns>The expression property of the sort.</returns>
		[AttachedPropertyBrowsableForType(typeof(GridViewColumn))]
		[SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Attached Property")]
		[SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Attached Property applicable to GridViewColumn")]
		public static string GetSortExpression(GridViewColumn element)
		{
			return (string)element.GetValue(SortExpressionProperty);
		}

		/// <summary>
		/// Sets the sort direction for the column.
		/// </summary>
		/// <param name="element">The <see cref="GridViewColumn"/> element.</param>
		/// <param name="value">The expression property of the sort.</param>
		[AttachedPropertyBrowsableForType(typeof(GridViewColumn))]
		[SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Attached Property")]
		[SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Attached Property applicable to GridViewColumn")]
		public static void SetSortExpression(GridViewColumn element, string value)
		{
			element.SetValue(SortExpressionProperty, value);
		}

		private static void OnCanUserSortColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var listView = (ListView)d;
			if ((bool)e.NewValue)
			{
				listView.AddHandler(GridViewColumnHeader.ClickEvent, (RoutedEventHandler)OnColumnHeaderClick);
				if (listView.IsLoaded)
				{
					DoInitialSort(listView);
				}
				else
				{
					listView.Loaded += OnLoaded;
				}
			}
			else
			{
				listView.RemoveHandler(GridViewColumnHeader.ClickEvent, (RoutedEventHandler)OnColumnHeaderClick);
			}
		}

		private static void OnLoaded(object sender, RoutedEventArgs e)
		{
			var listView = (ListView)e.Source;
			listView.Loaded -= OnLoaded;
			DoInitialSort(listView);
		}

		private static void DoInitialSort(ListView listView)
		{
			var gridView = (GridView)listView.View;
			var column = gridView.Columns.FirstOrDefault(c => GetSortDirection(c) != null);
			if (column != null)
			{
				DoSort(listView, column);
			}
		}

		private static void OnColumnHeaderClick(object sender, RoutedEventArgs e)
		{
			var columnHeader = e.OriginalSource as GridViewColumnHeader;
			if (columnHeader != null && GetCanUseSort(columnHeader.Column))
			{
				DoSort((ListView)e.Source, columnHeader.Column);
			}
		}

		private static void DoSort(ListView listView, GridViewColumn newColumn)
		{
			var sortDescriptions = listView.Items.SortDescriptions;
			var newDirection = ListSortDirection.Ascending;

			var propertyPath = ResolveSortExpression(newColumn);
			if (propertyPath != null)
			{
				if (sortDescriptions.Count > 0)
				{
					if (sortDescriptions[0].PropertyName == propertyPath)
					{
						newDirection = GetSortDirection(newColumn) == ListSortDirection.Ascending ?
							ListSortDirection.Descending :
							ListSortDirection.Ascending;
					}
					else
					{
						var gridView = (GridView)listView.View;
						foreach (var column in gridView.Columns.Where(c => GetSortDirection(c) != null))
						{
							SetSortDirection(column, null);
						}
					}

					sortDescriptions.Clear();
				}

				sortDescriptions.Add(new SortDescription(propertyPath, newDirection));
				SetSortDirection(newColumn, newDirection);
			}
		}

		private static string ResolveSortExpression(GridViewColumn column)
		{
			var propertyPath = GetSortExpression(column);
			if (propertyPath == null)
			{
				var binding = column.DisplayMemberBinding as Binding;
				return binding != null ? binding.Path.Path : null;
			}

			return propertyPath;
		}
	}
}