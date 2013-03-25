namespace NuPattern.VisualStudio.Shell
{
    /// <summary>
    /// Provides a contract to send messages to the user.
    /// </summary>
    public interface IUserMessageService
    {
        /// <summary>
        /// Shows an error to the user.
        /// </summary>
        /// <param name="message">The message to show.</param>
        void ShowError(string message);

        /// <summary>
        /// Shows information to the user.
        /// </summary>
        /// <param name="message">The message to show.</param>
        void ShowInformation(string message);

        /// <summary>
        /// Shows a warning to the user.
        /// </summary>
        /// <param name="message">The message to show.</param>
        void ShowWarning(string message);

        /// <summary>
        /// Prompts the user with a warning.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        bool PromptWarning(string message);
    }
}