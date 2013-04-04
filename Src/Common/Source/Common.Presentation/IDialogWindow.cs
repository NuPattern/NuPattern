namespace NuPattern.Presentation
{
    /// <summary>
    /// Represents a dialog window.
    /// </summary>
    public interface IDialogWindow
    {
        /// <summary>
        /// Gets or sets the data context.
        /// </summary>
        /// <value>The data context.</value>
        object DataContext { get; set; }

        /// <summary>
        /// Gets or sets the dialog result.
        /// </summary>
        /// <value>The dialog result.</value>
        bool? DialogResult { get; set; }

        /// <summary>
        /// Opens a dialog and returns only when the newly opened dialog is closed.
        /// </summary>
        /// <returns>A value that signifies how a dialog was closed by the user.</returns>
        bool? ShowDialog();

        /// <summary>
        /// Closes the dialog.
        /// </summary>
        void Close();
    }
}