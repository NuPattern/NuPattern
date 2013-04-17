using System;

namespace Microsoft.VisualStudio.TeamArchitect.PowerTools.Features
{
    /// <summary>
    /// Defines the persisted state of a feature in the solution
    /// </summary>
    public interface ISolutionFeatureState
    {
        /// <summary>
        /// Gets the identifier fo the feature
        /// </summary>
        string FeatureId { get; set; }

        /// <summary>
        /// Gets the name of the feature
        /// </summary>
        string InstanceName { get; set; }

        /// <summary>
        /// Gets the version of the persisted feature.
        /// </summary>
        Version Version { get; set; }
    }
}