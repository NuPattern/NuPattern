using System;
using NuPattern.Diagnostics;
using NuPattern.Runtime.Guidance.Workflow;

namespace NuPattern.Runtime.Guidance.Extensions
{
    /// <summary>
    /// Binds a guidance workflow state to the storage in 
    /// the blackboard properties for the 
    /// overriden states and the focused activity.
    /// </summary>
    internal class BlackboardWorkflowBinder
    {

        private readonly ITracer tracer;
        private readonly IGuidanceWorkflow guidanceWorkflow;
        private IGuidanceExtension feature;

        /// <summary>
        /// Binds the state of the guidance workflow to Blackboard data, 
        /// and starts tracking changes in the workflow to update the underlying 
        /// storage in the Blackboard
        /// </summary>
        public BlackboardWorkflowBinder(IGuidanceExtension feature, IGuidanceWorkflow guidanceWorkflow)
        {

            this.tracer = Tracer.Get<BlackboardWorkflowBinder>();
            this.guidanceWorkflow = guidanceWorkflow;

            this.feature = feature;

            //
            // Note: For BlackboardWorkflowBinder we don't set the WF state here because we have to wait
            // until the OnInitialize in Feature.cs initializes the Blackboard.
            //
            this.TrackChanges();
        }


        private void OnHasStateOverrideChanged(object sender, EventArgs e)
        {
            // If the override property changed, it means the state has 
            // been just overriden (HasStateOverride = true), in which 
            // case we do nothing as the OnNodeStateChanged would have 
            // caught it already. If it's false, then we have to clear 
            // the overriden state from storage.
            var guidanceNode = sender as IConditionalNode;
            if (guidanceNode != null && !guidanceNode.HasStateOverride)
            {
                BlackboardManager.Current.Set(BlackboardWorkflowBinder.StateOverrideKey(feature, guidanceNode), guidanceNode.State.ToString());
            }
        }

        private void OnIsUserAcceptedChanged(object sender, EventArgs e)
        {
            var guidanceNode = sender as IConditionalNode;
            if (guidanceNode != null)
            {
                BlackboardManager.Current.Set(BlackboardWorkflowBinder.UserAcceptedKey(feature, guidanceNode), guidanceNode.IsUserAccepted.ToString());
            }
        }

        private void OnNodeStateChanged(object sender, EventArgs e)
        {
            var guidanceNode = sender as IConditionalNode;
            if (guidanceNode != null)
            {
                BlackboardManager.Current.Set(BlackboardWorkflowBinder.StateOverrideKey(feature, guidanceNode), guidanceNode.HasStateOverride.ToString());
            }
        }

        private void TrackChanges()
        {
            foreach (var node in guidanceWorkflow.AllNodesToEvaluate)
            {
                node.StateChanged += OnNodeStateChanged;
                var conditional = node as IConditionalNode;
                conditional.IsUserAcceptedChanged += this.OnIsUserAcceptedChanged;
                conditional.HasStateOverrideChanged += this.OnHasStateOverrideChanged;
            }
        }

        public static string UserAcceptedKey(IGuidanceExtension feature, object node)
        {
            var guidanceNode = node as IConditionalNode;

            return (feature.InstanceName + @"." + guidanceNode.Name + @".UserAccepted");
        }

        public static string StateOverrideKey(IGuidanceExtension feature, object node)
        {
            var guidanceNode = node as IConditionalNode;

            return (feature.InstanceName + @"." + guidanceNode.Name + @".StateOverride");
        }

    }
}
