using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.TeamArchitect.PowerTools.Features
{
    /// <summary>
    /// Keeps the list of instantiated features in a solution in a permanent storage.
    /// </summary>
    public interface ISolutionState
    {
        /// <summary>
        /// Gets the sttes of the instantiated features.
        /// </summary>
        IEnumerable<ISolutionFeatureState> InstantiatedFeatures { get; }

        /// <summary>
        /// Adds a new feature to the solution
        /// </summary>
        /// <param name="featureId"></param>
        /// <param name="instanceName"></param>
        /// <param name="version"></param>
        void AddFeature(string featureId, string instanceName, Version version);

        /// <summary>
        /// Removes a feature from the solution.
        /// </summary>
        /// <param name="featureId"></param>
        /// <param name="instanceName"></param>
        void RemoveFeature(string featureId, string instanceName);

        /// <summary>
        /// Updates the feature state with the given id and name with the 
        /// specified version.
        /// </summary>
        void Update(string featureId, string instanceName, Version newVersion);
    }
}