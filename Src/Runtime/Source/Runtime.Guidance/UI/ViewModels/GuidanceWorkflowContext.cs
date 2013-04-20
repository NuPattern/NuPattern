using NuPattern.VisualStudio;

namespace NuPattern.Runtime.Guidance.UI.ViewModels
{
    /// <summary>
    /// Defines the needed classes for the <see cref="GuidanceWorkflowViewModel"/>.
    /// </summary>
    internal class GuidanceWorkflowContext
    {
        /// <summary>
        /// Gets or sets the view model.
        /// </summary>
        public GuidanceWorkflowViewModel ViewModel { get; internal set; }

        /// <summary>
        /// Gets the current guidance extension
        /// </summary>
        public IGuidanceExtension Extension { get; internal set; }

        /// <summary>
        /// Gets the current guidance manager
        /// </summary>
        public IGuidanceManager GuidanceManager { get; internal set; }

        /// <summary>
        /// Gets the user message service.
        /// </summary>
        public IUserMessageService UserMessageService { get; internal set; }
    }
}