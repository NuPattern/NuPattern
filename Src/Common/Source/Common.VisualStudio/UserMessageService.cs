using System;
using System.ComponentModel.Composition;
using System.Windows;
using Microsoft.VisualBasic;
using Microsoft.VisualStudio.Shell;
using NuPattern.VisualStudio.Properties;

namespace NuPattern.VisualStudio
{
    /// <summary>
    /// Provides an implementation to send messages to the user using a message box.
    /// </summary>
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IUserMessageService))]
    internal class UserMessageService : IUserMessageService
    {
        private IServiceProvider serviceProvider;
        private Microsoft.VisualStudio.Shell.Interop.IVsUIShell shell;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserMessageService"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        [ImportingConstructor]
        public UserMessageService([Import(typeof(SVsServiceProvider))] IServiceProvider serviceProvider)
        {
            Guard.NotNull(() => serviceProvider, serviceProvider);

            this.serviceProvider = serviceProvider;
            this.shell = this.serviceProvider.GetService<Microsoft.VisualStudio.Shell.Interop.SVsUIShell, Microsoft.VisualStudio.Shell.Interop.IVsUIShell>();
        }

        /// <summary>
        /// Shows an error to the user.
        /// </summary>
        /// <param name="message">The message to show.</param>
        public void ShowError(string message)
        {
            System.Windows.MessageBox.Show(this.shell.GetMainWindow(), message, Resources.UserMessageService_MessageBoxTitle, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Shows information to the user.
        /// </summary>
        /// <param name="message">The message to show.</param>
        public void ShowInformation(string message)
        {
            this.shell.EnableModeless(0);
            try
            {
                ThreadHelper.Generic.Invoke(() =>
                    System.Windows.MessageBox.Show(this.shell.GetMainWindow(), message, Resources.UserMessageService_MessageBoxTitle, MessageBoxButton.OK, MessageBoxImage.Information));
            }
            finally
            {
                this.shell.EnableModeless(1);
            }
        }

        /// <summary>
        /// Shows a warning to the user.
        /// </summary>
        /// <param name="message">The message to show.</param>
        public void ShowWarning(string message)
        {
            this.shell.EnableModeless(0);
            try
            {
                ThreadHelper.Generic.Invoke(() =>
                    System.Windows.MessageBox.Show(this.shell.GetMainWindow(), message, Resources.UserMessageService_MessageBoxTitle, MessageBoxButton.OK, MessageBoxImage.Warning));
            }
            finally
            {
                this.shell.EnableModeless(1);
            }
        }

        /// <summary>
        /// Shows a warning prompting the user.
        /// </summary>
        /// <param name="message">The message to show.</param>
        public bool PromptWarning(string message)
        {
            this.shell.EnableModeless(0);
            try
            {
                return ThreadHelper.Generic.Invoke(() =>
                    System.Windows.MessageBox.Show(this.shell.GetMainWindow(), message, Resources.UserMessageService_MessageBoxTitle, MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK);
            }
            finally
            {
                this.shell.EnableModeless(1);
            }
        }

        /// <summary>
        /// Shows an input box that retrieves input from a user.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <param name="defaultValue">The default value to display.</param>
        /// <returns></returns>
        public string PromptInput(string message, string defaultValue)
        {
            // TODO: implment a better dialog box
            return (Interaction.InputBox(message, Resources.UserMessageService_MessageBoxTitle, string.IsNullOrEmpty(defaultValue) ? string.Empty : defaultValue));
        }
    }
}