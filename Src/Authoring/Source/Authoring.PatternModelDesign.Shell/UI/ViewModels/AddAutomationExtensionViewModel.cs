using System.Collections.Generic;
using System.Linq;
using NuPattern.Presentation;
using NuPattern.Runtime.Automation;

namespace NuPattern.Runtime.Schema.UI.ViewModels
{
    /// <summary>
    /// View model for add automation extension
    /// </summary>
    internal class AddAutomationExtensionViewModel : ViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddAutomationExtensionViewModel"/> class.
        /// </summary>
        /// <param name="exportedAutomations">The exported automations.</param>
        public AddAutomationExtensionViewModel(IEnumerable<IExportedAutomationMetadata> exportedAutomations)
        {
            Guard.NotNull(() => exportedAutomations, exportedAutomations);

            this.ExportedAutomations = exportedAutomations.OrderBy(a => a.DisplayName).ToArray();
            this.InitializeCommands();
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
        public System.Windows.Input.ICommand SelectAutomationExtensionCommand
        {
            get;
            private set;
        }

        private void InitializeCommands()
        {
            this.SelectAutomationExtensionCommand = new RelayCommand<IDialogWindow>(dialog => CloseDialog(dialog), dialog => CanCloseDialog());
        }

        private bool CanCloseDialog()
        {
            return (this.CurrentExportedAutomation != null);
        }

        private static void CloseDialog(IDialogWindow dialog)
        {
            dialog.DialogResult = true;
            dialog.Close();
        }
    }
}