using System;
using System.Windows;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;

namespace NuPattern.Runtime
{
	/// <summary>
	/// Defines a picker used for selecting filtered solution artifacts.
	/// </summary>
	[CLSCompliant(false)]
	public interface ISolutionPicker : IDialogWindow
	{
		/// <summary>
		/// The currently selected item.
		/// </summary>
		IItemContainer SelectedItem { get; set; }

		/// <summary>
		/// The current fileter to apply to items in the picker.
		/// </summary>
		IPickerFilter Filter
		{
			get;
		}

		/// <summary>
		/// The title of the picker.
		/// </summary>
		string Title { get; set; }

		/// <summary>
		/// The message to show when no items can be found to pick.
		/// </summary>
		string EmptyItemsMessage { get; set; }

		/// <summary>
		/// The root item to start teh select at.
		/// </summary>
		IItemContainer RootItem { get; set; }

		/// <summary>
		/// The window owner of the picker dialog.
		/// </summary>
		Window Owner { get; set; }
	}
}