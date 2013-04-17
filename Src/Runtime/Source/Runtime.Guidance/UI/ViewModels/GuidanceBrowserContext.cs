using NuPattern;
using NuPattern.VisualStudio;

namespace Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.UI.ViewModels
{
    /// <summary>
    /// Defines the needed classes for the <see cref="GuidanceBrowserViewModel"/>.
    /// </summary>
    internal class GuidanceBrowserContext
    {
        /// <summary>
        /// Gets or sets the view model.
        /// </summary>
        public GuidanceBrowserViewModel ViewModel { get; internal set; }

        /// <summary>
        /// Gets the feature manager.
        /// </summary>
        public IFeatureManager FeatureManager { get; internal set; }

        /// <summary>
        /// Gets the URI reference service
        /// </summary>
        public IUriReferenceService UriReferenceService { get; internal set; }

        /// <summary>
        /// Gets the user message service.
        /// </summary>
        public IUserMessageService UserMessageService { get; internal set; }
    }
}