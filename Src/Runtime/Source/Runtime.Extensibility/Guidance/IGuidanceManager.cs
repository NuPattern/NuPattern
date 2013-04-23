using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NuPattern.Runtime.Guidance
{
    /// <summary>
    /// Manages guidance extensions and their lifetime.
    /// </summary>
    public interface IGuidanceManager : INotifyPropertyChanged
    {
        /// <summary>
        /// Event raised when the list of instantiated guidance extensions changed.
        /// </summary>
        event EventHandler InstantiatedExtensionsChanged;

        /// <summary>
        /// Event raised when a solution is opened or closed.
        /// </summary>
        event EventHandler IsOpenedChanged;

        /// <summary>
        /// Event raised when the active guidance extension is changed.
        /// </summary>
        event EventHandler ActiveExtensionChanged;

        /// <summary>
        /// Installed guidance extensions.
        /// </summary>
        IEnumerable<IGuidanceExtensionRegistration> InstalledGuidanceExtensions { get; }

        /// <summary>
        /// Instantiated guidance extensions in the initialized solution, if any.
        /// </summary>
        IEnumerable<IGuidanceExtension> InstantiatedGuidanceExtensions { get; }

        /// <summary>
        /// Whether the manager has been initialized for a solution. 
        /// </summary>
        bool IsOpened { get; }

        /// <summary>
        /// Gets or sets the active guidance extension.
        /// </summary>
        IGuidanceExtension ActiveGuidanceExtension { get; set; }

        /// <summary>
        /// Instantiates the given guidance extension in the current solution.
        /// </summary>
        /// <param name="extensionId">Identifier for the guidance extension.</param>
        /// <param name="instanceName">Name to assign to the newly created guidance extension instance.</param>
        /// <returns>The instantiated guidance extension.</returns>
        IGuidanceExtension Instantiate(string extensionId, string instanceName);

        /// <summary>
        /// Initializes the manager with a solution state.
        /// </summary>
        void Open(ISolutionState solutionState);

        /// <summary>
        /// Release the manager for the current solution.
        /// </summary>
        void Close();

        /// <summary>
        /// Finishes the given guidance extension.
        /// </summary>
        /// <param name="instanceName">Name of the guidance extension instance to finish.</param>
        void Finish(string instanceName);

        /// <summary>
        /// Finds the installed guidance extension associated with the type, if any.
        /// </summary>
        IGuidanceExtensionRegistration FindGuidanceExtension(Type type);

        /// <summary>
        /// Finds the installed guidance extension associated with the file, if any.
        /// </summary>
        IGuidanceExtensionRegistration FindGuidanceExtension(string filename);

        /// <summary>
        /// Finds the installed guidance extension associated with the type, if any.
        /// </summary>
        IGuidanceExtensionRegistration FindGuidanceExtension(Func<IGuidanceExtensionRegistration, bool> predicate);
    }
}