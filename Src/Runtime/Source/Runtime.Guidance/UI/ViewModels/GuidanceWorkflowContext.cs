using NuPattern.VisualStudio;

namespace Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.UI.ViewModels
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
        /// Gets the current feature extension
        /// </summary>
        public IFeatureExtension FeatureExtension { get; internal set; }

        /// <summary>
        /// Gets the user message service.
        /// </summary>
        public IUserMessageService UserMessageService { get; internal set; }
    }
}