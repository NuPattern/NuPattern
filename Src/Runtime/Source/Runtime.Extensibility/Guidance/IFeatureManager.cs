using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.VisualStudio.TeamArchitect.PowerTools.Features
{
    /// <summary>
    /// Manages features and their lifetime.
    /// </summary>
    [CLSCompliant(false)]
    public interface IFeatureManager : INotifyPropertyChanged
    {
        /// <summary>
        /// Event raised when the list of instantiated features changed.
        /// </summary>
        event EventHandler InstantiatedFeaturesChanged;

        /// <summary>
        /// Event raised when a solution is opened or closed.
        /// </summary>
        event EventHandler IsOpenedChanged;

        /// <summary>
        /// Event raised when the active feature is changed.
        /// </summary>
        event EventHandler ActiveFeatureChanged;

        /// <summary>
        /// Installed feature extensions.
        /// </summary>
        IEnumerable<IFeatureRegistration> InstalledFeatures { get; }

        /// <summary>
        /// Instantiated features in the initialized solution, if any.
        /// </summary>
        IEnumerable<IFeatureExtension> InstantiatedFeatures { get; }

        /// <summary>
        /// Whether the manager has been initialized for a solution. 
        /// </summary>
        bool IsOpened { get; }

        /// <summary>
        /// Gets or sets the active feature.
        /// </summary>
        IFeatureExtension ActiveFeature { get; set; }

        /// <summary>
        /// Instantiates the given feature in the current solution.
        /// </summary>
        /// <param name="featureId">Identifier for the feature type.</param>
        /// <param name="instanceName">Name to assign to the newly created feature instance.</param>
        /// <returns>The instantiated feature.</returns>
        IFeatureExtension Instantiate(string featureId, string instanceName);

        /// <summary>
        /// Initializes the manager with a solution state.
        /// </summary>
        void Open(ISolutionState solutionState);

        /// <summary>
        /// Release the manager for the current solution.
        /// </summary>
        void Close();

        /// <summary>
        /// Finishes the given feature.
        /// </summary>
        /// <param name="instanceName">Name of the feature instance to finish.</param>
        void Finish(string instanceName);

        /// <summary>
        /// Finds the installed feature associated with the type, if any.
        /// </summary>
        IFeatureRegistration FindFeature(Type type);

        /// <summary>
        /// Finds the installed feature associated with the file, if any.
        /// </summary>
        IFeatureRegistration FindFeature(string filename);

        /// <summary>
        /// Finds the installed feature associated with the type, if any.
        /// </summary>
        IFeatureRegistration FindFeature(Func<IFeatureRegistration, bool> predicate);
    }
}