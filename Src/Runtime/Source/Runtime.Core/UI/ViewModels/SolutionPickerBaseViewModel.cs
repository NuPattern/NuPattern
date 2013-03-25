using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using NuPattern.Presentation;

namespace NuPattern.Runtime.UI.ViewModels
{
    /// <summary>
    /// Provides a view model for the solution pickers
    /// </summary>
    internal abstract class SolutionPickerBaseViewModel : ViewModel
    {
        private string emptyItemsMessage;

        /// <summary>
        /// Creates a new instance of the <see cref="SolutionPickerViewModel"/> class.
        /// </summary>
        public SolutionPickerBaseViewModel(FilteredItemContainer root, IPickerFilter filter)
        {
            Guard.NotNull(() => root, root);
            Guard.NotNull(() => filter, filter);

            this.Filter = filter;
            this.Items = (this.Filter.ApplyFilter(root.Item)) ? new[] { root } : Enumerable.Empty<FilteredItemContainer>();

            this.InitializeCommands();
        }

        /// <summary>
        /// Gets the submit command.
        /// </summary>
        public System.Windows.Input.ICommand SubmitCommand { get; private set; }

        /// <summary>
        /// Gets or sets the empty items message.
        /// </summary>
        public string EmptyItemsMessage
        {
            get
            {
                if (string.IsNullOrEmpty(this.emptyItemsMessage))
                {
                    this.EmptyItemsMessage = Properties.Resources.SolutionPickerBaseViewModel_EmptyItemsMessage;
                    if (!String.IsNullOrEmpty(this.Filter.MatchFileExtensions))
                    {
                        this.EmptyItemsMessage = string.Format(CultureInfo.CurrentCulture, Properties.Resources.SolutionPickerBaseViewModel_EmptyItemsMessageFileType,
                                                               this.Filter.MatchFileExtensions);
                    }
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

        /// <summary>
        /// Initializes the commands for the dialog
        /// </summary>
        protected virtual void InitializeCommands()
        {
            this.SubmitCommand = new RelayCommand<IDialogWindow>(CloseDialog, this.CanCloseDialog);
        }

        /// <summary>
        /// Whether the dialog can be closed
        /// </summary>
        protected abstract bool CanCloseDialog(IDialogWindow dialog);

        private static void CloseDialog(IDialogWindow dialog)
        {
            // For a TreeViewItem the double-click event bubbles up and fires for every ancestor TreeViewItem as well
            // We only want to handle it once to close the dialog (ignore repeated attempts to close dialog)
            if (dialog != null)
            {
                if (dialog.DialogResult != true)
                {
                    dialog.DialogResult = true;
                    dialog.Close();
                }
            }
        }
    }
}
