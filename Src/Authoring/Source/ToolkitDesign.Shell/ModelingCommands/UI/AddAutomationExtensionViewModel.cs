using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.Patterning.Runtime;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Input = System.Windows.Input;

namespace Microsoft.VisualStudio.Patterning.Runtime.Schema
{
	/// <summary>
	/// View model for add automation extension
	/// </summary>
	[CLSCompliant(false)]
	public class AddAutomationExtensionViewModel : Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.ViewModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AddAutomationExtensionViewModel"/> class.
		/// </summary>
		/// <param name="exportedAutomations">The exported automations.</param>
		public AddAutomationExtensionViewModel(IEnumerable<IExportedAutomationMetadata> exportedAutomations)
		{
			Guard.NotNull(() => exportedAutomations, exportedAutomations);

			this.ExportedAutomations = exportedAutomations.OrderBy(a => a.DisplayName).ToArray();
			this.RegisterCommands();
		}

		/// <summary>
		/// Gets or sets the current exported automation.
		/// </summary>
		/// <value>The current exported automation.</value>
		public IExportedAutomationMetadata CurrentExportedAutomation
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the exported automations.
		/// </summary>
		/// <value>The exported automations.</value>
		public IEnumerable<IExportedAutomationMetadata> ExportedAutomations
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets or sets the select automation extension command.
		/// </summary>
		/// <value>The select automation extension command.</value>
		public Input.ICommand SelectAutomationExtensionCommand
		{
			get;
			private set;
		}

		private void RegisterCommands()
		{
			this.SelectAutomationExtensionCommand = new RelayCommand<IDialogWindow>(w => CloseDialog(w), w => this.CurrentExportedAutomation != null);
		}

		private static void CloseDialog(IDialogWindow dialog)
		{
			dialog.DialogResult = true;
			dialog.Close();
		}
	}
}