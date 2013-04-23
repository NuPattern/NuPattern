using System;
using System.Collections.Generic;
using NuPattern.Runtime.Guidance.Workflow;

namespace NuPattern.Runtime.Guidance
{
    /// <summary>
    /// Defines a guidance extension.
    /// </summary>
    public interface IGuidanceExtension
    {
        /// <summary>
        /// Gets the singleton instance of the <see cref="GuidanceManager"/>
        /// </summary>
        IGuidanceManager GuidanceManager { get; }

        /// <summary>
        /// Gets the commands included in this guidance extension.
        /// </summary>
        IEnumerable<ICommandBinding> Commands { get; }

        /// <summary>
        /// Gets the guidance extension identifier.
        /// </summary>
        string ExtensionId { get; }

        /// <summary>
        /// Gets the process guidance workflow instance.
        /// </summary>
        IGuidanceWorkflow GuidanceWorkflow { get; }

        /// <summary>
        /// Gets the name of a concrete instance of a guidance extension.
        /// </summary>
        string InstanceName { get; }

        /// <summary>
        /// Gets the flag which indicates if the active state
        /// state is persisted
        /// </summary>
        bool PersistStateInSolution { get; }

        /// <summary>
        /// Gets the flag which indicates if the instantiation of
        /// this guidance extension is stored in the Solution and thus is
        /// re-initialize when the solution is re-opened.
        /// </summary>
        bool PersistInstanceInSolution { get; }

        /// <summary>
        /// Instantiates the guidance extension in a solution. 
        /// </summary>
        /// <param name="registration">The guidance extension registration information, which includes its identifier and manifest information..</param>
        /// <param name="instanceName">Name of the instance.</param>
        /// <param name="guidanceManager">Reference to the <see cref="GuidanceManager"/></param>
        /// <remarks>
        /// This method is called when a guidance extension is 
        /// first instantiated in a solution.
        /// </remarks>
        void Instantiate(IGuidanceExtensionRegistration registration, string instanceName, IGuidanceManager guidanceManager);

        /// <summary>
        /// Initializes the guidance extension.
        /// </summary>
        /// <param name="registration">The guidance extension registration information, which includes its identifier and manifest information..</param>
        /// <param name="instanceName">Name of the instance.</param>
        /// <param name="persistedVersion">The version of the guidance extension.</param>
        /// <param name="guidanceManager">Reference to the <see cref="GuidanceManager"/></param>
        /// <remarks>
        /// This method is called after <see cref="Instantiate"/>
        /// when a guidance extension is first initialized in a solution,
        /// or after reopening a solution where the guidance extension
        /// had been previously instantiated.
        /// </remarks>
        void Initialize(IGuidanceExtensionRegistration registration, string instanceName, Version persistedVersion, IGuidanceManager guidanceManager);

        /// <summary>
        /// Called so that guidance extensions who desire a persistent blackboard
        /// can call BlackboardManager.Current.IsPersisted = true;
        /// </summary>
        /// <remarks>
        /// Called during RunFinished of the template wizard.
        /// If instantiating 
        /// </remarks>
        void PostInitialize();

        /// <summary>
        /// Finishes the guidance extension.
        /// </summary>
        void Finish();

        ///// <summary>
        ///// Invoked when the guidance extension is being deleted from the solution. 
        ///// guidance extension author can cancel the deletion by setting 
        ///// the <see cref="CancelEventArgs.Cancel"/> flag.
        ///// </summary>
        ///// <param name="cancel">Allows specifying whether the deletion must be canceled.</param>
        //void Deleting(CancelEventArgs cancel);
    }
}