using System;
using System.Collections.Generic;
using NuPattern.Runtime.Guidance.Workflow;

namespace NuPattern.Runtime.Guidance
{
    /// <summary>
    /// Main feature interface, which provides feature lifecycle members.
    /// </summary>
    public interface IFeatureExtension
    {
        /// <summary>
        /// Gets the singleton instance of the FeatureManager
        /// </summary>
        IFeatureManager FeatureManager { get; }

        /// <summary>
        /// Gets the commands included in this feature.
        /// </summary>
        IEnumerable<ICommandBinding> Commands { get; }

        /// <summary>
        /// Gets the feature type identifier.
        /// </summary>
        string FeatureId { get; }

        /// <summary>
        /// Gets the process guidance workflow instance.
        /// </summary>
        IGuidanceWorkflow GuidanceWorkflow { get; }

        /// <summary>
        /// Gets the name of a concrete instance of a feature.
        /// </summary>
        string InstanceName { get; }

        /// <summary>
        /// Gets the flag which indicates if the active state
        /// state is persisted
        /// </summary>
        bool PersistStateInSolution { get; }

        /// <summary>
        /// Gets the flag which indicates if the instantiation of
        /// this feature is stored in the Solution and thus is
        /// re-initialize when the solution is re-opened.
        /// </summary>
        bool PersistInstanceInSolution { get; }

        /// <summary>
        /// Instantiates the feature in a solution. 
        /// </summary>
        /// <param name="registration">The feature registration information, which includes its identifier and manifest information..</param>
        /// <param name="instanceName">Name of the instance.</param>
        /// <param name="featureManager">Reference to the FeatureManager</param>
        /// <remarks>
        /// This method is called when a feature is 
        /// first instantiated in a solution.
        /// </remarks>
        void Instantiate(IFeatureRegistration registration, string instanceName, IFeatureManager featureManager);

        /// <summary>
        /// Initializes the feature.
        /// </summary>
        /// <param name="registration">The feature registration information, which includes its identifier and manifest information..</param>
        /// <param name="instanceName">Name of the instance.</param>
        /// <param name="persistedVersion">The version of the feature.</param>
        /// <param name="featureManager">Reference to the FeatureManager</param>
        /// <remarks>
        /// This method is called after <see cref="Instantiate"/>
        /// when a feature is first initialized in a solution,
        /// or after reopening a solution where the feature
        /// had been previously instantiated.
        /// </remarks>
        void Initialize(IFeatureRegistration registration, string instanceName, Version persistedVersion, IFeatureManager featureManager);

        /// <summary>
        /// Called so that features who desire a persistent blackboard
        /// can call BlackboardManager.Current.IsPersisted = true;
        /// </summary>
        /// <remarks>
        /// Called during RunFinished of the template wizard.
        /// If instantiating 
        /// </remarks>
        void PostInitialize();

        /// <summary>
        /// Finishes the feature.
        /// </summary>
        void Finish();

        ///// <summary>
        ///// Invoked when the feature is being deleted from the solution. 
        ///// Feature author can cancel the deletion by setting 
        ///// the <see cref="CancelEventArgs.Cancel"/> flag.
        ///// </summary>
        ///// <param name="cancel">Allows specifying whether the deletion must be canceled.</param>
        //void Deleting(CancelEventArgs cancel);
    }
}