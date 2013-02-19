using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;

namespace NuPattern.Runtime.UI
{
	/// <summary>
	/// Provides a view model for the solution picker
	/// </summary>
	[CLSCompliant(false)]
	public partial class SolutionPickerViewModel : ViewModel
	{
		private static readonly ITraceSource tracer = Tracer.GetSourceFor<SolutionPickerViewModel>();
		private string emptyItemsMessage;

		/// <summary>
		/// Creates a new instance of the <see cref="SolutionPickerViewModel"/> class.
		/// </summary>
		public SolutionPickerViewModel(FilteredItemContainer root, IPickerFilter filter)
		{
			Guard.NotNull(() => root, root);
			Guard.NotNull(() => filter, filter);

			this.Filter = filter;
			this.Items = (this.Filter.ApplyFilter(root.Item)) ? new[] { root } : Enumerable.Empty<FilteredItemContainer>();

			this.InitializeCommands();
		}

		/// <summary>
		/// Gets the activate command.
		/// </summary>
		public System.Windows.Input.ICommand SelectItemCommand { get; private set; }

		/// <summary>
		/// Gets or sets the empty items message.
		/// </summary>
		public string EmptyItemsMessage
		{
			get
			{
				if (string.IsNullOrEmpty(this.emptyItemsMessage))
				{
					this.EmptyItemsMessage = string.Format(CultureInfo.CurrentCulture,
						Properties.Resources.SolutionPickerViewModel_EmptyItemsMessage, this.Filter.IncludeFileExtensions);
				}

				return this.emptyItemsMessage;
			}
			set
			{
				this.emptyItemsMessage = value;
				OnPropertyChanged(() => this.EmptyItemsMessage);
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
		/// Gets or sets the items for display.
		/// </summary>
		public IEnumerable<FilteredItemContainer> Items
		{
			get;
			private set;
		}

		private void InitializeCommands()
		{
			this.SelectItemCommand = new RelayCommand<IDialogWindow>(dialog => this.SelectItem(dialog), dialog => this.CanSelectItem(dialog));
		}

		private bool CanSelectItem(IDialogWindow dialog)
		{
			// Ensure an item is selected
			var selectedItem = GetSelectedItem();
			if (selectedItem == null)
			{
				return false;
			}

			// Are we filtering only files?
			if (!string.IsNullOrEmpty(this.Filter.IncludeFileExtensions))
			{
				return (selectedItem.Item.Kind == ItemKind.Item);
			}
			else
			{
				return true;
			}
		}

		private void SelectItem(IDialogWindow dialog)
		{
			// For a TreeViewItem the double-click event bubbles up and fires for every ancestor TreeViewItem as well
			// We only want to handle it once to close the dialog (ignore repeated attempts to close dialog)
			if (dialog != null
				&& dialog.DialogResult == true)
			{
				return;
			}

			dialog.DialogResult = true;
			dialog.Close();
		}

		internal FilteredItemContainer GetSelectedItem()
		{
			// Find first selected item in hierarchy
			return this.Items.Traverse(item => item.Items)
				.Where(subItem => subItem.IsSelected)
				.FirstOrDefault();
		}

		internal void SetSelectedItem(IItemContainer selectedItem)
		{
			if (selectedItem != null)
			{
				// Find the item in the hierarchy
				var filteredItem = this.Items.Traverse(item => item.Items)
					.Where(subItem => subItem.Item.Equals(selectedItem))
					.FirstOrDefault();
				if (filteredItem != null)
				{
					filteredItem.IsSelected = true;
					filteredItem.IsExpanded = true;
				}
			}
		}
	}
}
