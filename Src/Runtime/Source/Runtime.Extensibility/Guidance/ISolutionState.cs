using System;
using System.Collections.Generic;

namespace NuPattern.Runtime.Guidance
{
    /// <summary>
    /// Keeps the list of instantiated guidance extension in a persisted storage.
    /// </summary>
    public interface ISolutionState
    {
        /// <summary>
        /// Gets the states of the instantiated guidance extension.
        /// </summary>
        IEnumerable<IGuidanceExtensionPersistState> InstantiatedGuidanceExtensions { get; }

        /// <summary>
        /// Adds a new guidance extension to the solution
        /// </summary>
        /// <param name="extensionId"></param>
        /// <param name="instanceName"></param>
        /// <param name="version"></param>
        void AddExtension(string extensionId, string instanceName, Version version);

        /// <summary>
        /// Removes a guidance extension from the solution.
        /// </summary>
        /// <param name="extensionId"></param>
        /// <param name="instanceName"></param>
        void RemoveExtension(string extensionId, string instanceName);

        /// <summary>
        /// Updates the guidance extension state with the given id and name with the 
        /// specified version.
        /// </summary>
        void Update(string extensionId, string instanceName, Version newVersion);
    }
}