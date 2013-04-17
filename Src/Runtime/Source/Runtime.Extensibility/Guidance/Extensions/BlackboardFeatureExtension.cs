using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using NuPattern.Runtime.Guidance.Workflow;
using NuPattern.VisualStudio.Solution;

namespace NuPattern.Runtime.Guidance.Extensions
{
    /// <summary>
    /// A feature extension that uses a blackboard to persist its state.
    /// </summary>
    public class BlackboardFeatureExtension<TGeneratedWorkflow> : BlackboardFeatureExtension
        where TGeneratedWorkflow : GuidanceWorkflow, new()
    {
        private BlackboardWorkflowBinder binder;

        /// <summary>
        /// Gets the commands for the extension
        /// </summary>
        public override IEnumerable<ICommandBinding> Commands
        {
            get { yield break; }
        }

        /// <summary>
        /// Called to create the workflow.
        /// </summary>
        /// <returns></returns>
        protected internal override IGuidanceWorkflow OnCreateWorkflow()
        {
            var workflow = new TGeneratedWorkflow();
            this.FeatureComposition.SatisfyImportsOnce(workflow);
            workflow.OwningFeature = this;
            workflow.Initialize();

            this.binder = new BlackboardWorkflowBinder(this, workflow);
            return workflow;
        }
    }

    /// <summary>
    /// A feature extension that uses a blackboard to persist its state.
    /// </summary>
    public abstract class BlackboardFeatureExtension : FeatureExtension, IDisposable
    {
        /// <summary>
        /// Gets the Blackboard Manager
        /// </summary>
        [Import]
        public BlackboardManager BlackboardManager { get; set; }

        /// <summary>
        /// Disposes this instance
        /// </summary>
        ~BlackboardFeatureExtension()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Gets the project for this extension
        /// </summary>
        public IProject Project { get; private set; }

        /// <summary>
        /// Called to create the workflow for this feature
        /// </summary>
        /// <returns></returns>
        protected internal abstract IGuidanceWorkflow OnCreateWorkflow();

        /// <summary>
        /// Disposes this instance
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Creates the workflow for this instance
        /// </summary>
        /// <returns></returns>
        public override IGuidanceWorkflow CreateWorkflow()
        {
            var workflow = this.OnCreateWorkflow();
            return workflow;
        }

        /// <summary>
        /// Disposes this instance
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
        }

        /// <summary>
        /// Called after the extension is finished
        /// </summary>
        protected internal override void OnFinish()
        {
        }

        /// <summary>
        /// Called to initialize this instance
        /// </summary>
        /// <param name="persistedVersion"></param>
        protected internal override void OnInitialize(Version persistedVersion)
        {
            base.OnInitialize(persistedVersion);
            BlackboardManager.Initialize();
        }

        /// <summary>
        /// Called when an instance is created.
        /// </summary>
        protected internal override void OnInstantiate()
        {
        }

        /// <summary>
        /// Called after an instance is created.
        /// </summary>
        protected internal override void OnPostInitialize()
        {
            base.OnPostInitialize();
            if (PersistStateInSolution)
            {
                BlackboardManager.Current.IsPersistent = true;

                //
                // Restore workflow state from Blackboard
                //
                IGuidanceWorkflow guidanceWorkflow = this.GuidanceWorkflow;
                foreach (var node in guidanceWorkflow.AllNodesToEvaluate)
                {
                    var conditional = node as IConditionalNode;
                    if (conditional != null)
                    {
                        string userAccepted = BlackboardManager.Current.Get(BlackboardWorkflowBinder.UserAcceptedKey(this, conditional));
                        bool userAcceptedValue = false;
                        if (userAccepted != null)
                            userAcceptedValue = bool.Parse(userAccepted);
                        conditional.IsUserAccepted = userAcceptedValue;

                        string hasOverride = BlackboardManager.Current.Get(BlackboardWorkflowBinder.StateOverrideKey(this, conditional));
                        try
                        {
                            NodeState hasOverrideValue = NodeState.Enabled;
                            if (hasOverride != null)
                            {
                                hasOverrideValue = ((NodeState)NodeState.Parse(typeof(NodeState), hasOverride));
                                conditional.SetState(hasOverrideValue, false);
                            }
                        }
                        catch
                        {
                        }
                    }

                }
            }
        }
    }
}