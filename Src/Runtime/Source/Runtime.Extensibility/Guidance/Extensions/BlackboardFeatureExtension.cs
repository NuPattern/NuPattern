using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using NuPattern.VisualStudio.Solution;

namespace Microsoft.VisualStudio.TeamArchitect.PowerTools.Features
{
    public class BlackboardFeatureExtension<TGeneratedWorkflow> : BlackboardFeatureExtension
        where TGeneratedWorkflow : GuidanceWorkflow, new()
    {
        private BlackboardWorkflowBinder binder;

        public override IEnumerable<ICommandBinding> Commands
        {
            get { yield break; }
        }

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

    public abstract class BlackboardFeatureExtension : FeatureExtension, IDisposable
    {
        /// <summary>
        /// Import Blackboard Manager instance
        /// </summary>
        [Import]
        public BlackboardManager BlackboardManager { get; set; }

        public const string FeatureProjectCategory = "Blackboard";

        ~BlackboardFeatureExtension()
        {
            this.Dispose(false);
        }

        public IProject IdeProject { get; private set; }

        protected internal abstract IGuidanceWorkflow OnCreateWorkflow();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public override IGuidanceWorkflow CreateWorkflow()
        {
            IGuidanceWorkflow newWF = this.OnCreateWorkflow();
            return newWF;
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        protected internal override void OnFinish()
        {
        }

        protected internal override void OnInitialize(Version persistedVersion)
        {
            base.OnInitialize(persistedVersion);
            BlackboardManager.Initialize();
        }

        protected internal override void OnInstantiate()
        {
        }

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