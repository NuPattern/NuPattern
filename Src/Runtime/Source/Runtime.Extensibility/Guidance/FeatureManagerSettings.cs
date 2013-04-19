using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;

namespace NuPattern.Runtime.Guidance
{
    /// <summary>
    /// Provides settings for working with the <see cref="IFeatureManager"/>.
    /// </summary>
    public static class FeatureManagerSettings
    {
        /// <summary>
        /// Gets or sets whether to have verbose tracing for bindings
        /// </summary>
        public static bool VerboseBindingTracing { get; set; }

        /// <summary>
        /// Gets or sets whether to have verbose tracing.
        /// </summary>
        public static bool VerboseTracing { get; set; }
    }
}
