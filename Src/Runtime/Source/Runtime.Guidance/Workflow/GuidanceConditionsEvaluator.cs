using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NuPattern;
using NuPattern.Diagnostics;
using NuPattern.Runtime.Guidance;

namespace Microsoft.VisualStudio.TeamArchitect.PowerTools.Features
{
    internal class GuidanceConditionsEvaluator
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<GuidanceConditionsEvaluator>();
        public static bool TraceStateChanges = false;

        private IFeatureManager featureManager;
        private Queue<IFeatureExtension> queuedFeatures = new Queue<IFeatureExtension>();
        private Queue<IConditionalNode> queuedActions = new Queue<IConditionalNode>();
        private string currentFeature;
        private static IFeatureExtension currentFeatureInstance;
        private static GuidanceConditionsEvaluator Current;

        public GuidanceConditionsEvaluator(IFeatureManager featureManager)
        {
            Guard.NotNull(() => featureManager, featureManager);

            this.featureManager = featureManager;
            Current = this;
        }

        public static void EvaluateGraph(IFeatureExtension feature)
        {
            Guard.NotNull(() => feature, feature);

            currentFeatureInstance = feature;

            var workflow = feature.GuidanceWorkflow;
            if (workflow != null)
            {
                TraceStateChanges = FeatureManagerSettings.VerboseTracing = workflow.Successors.ToArray<INode>()[0].Name.StartsWith("DebugTrace");
                FeatureManagerSettings.VerboseBindingTracing = workflow.Successors.ToArray<INode>()[0].Name == "DebugTraceWithBindings";

                EvaluateNode(workflow);

                IList<IConditionalNode> nodes = NodesToEvaluate(workflow);
                foreach (var conditionalNode in nodes)
                {
                    EvaluateNode(conditionalNode);
                }
            }
        }


        /// <summary>
        /// Process the states of the following activity.
        /// </summary>
        /// <returns><see langword="true"/>, if there is more activities to process; otherwise <see langword="false"/>.</returns>
        public virtual bool EvaluateGraphs()
        {
            if (this.featureManager.IsOpened)
            {
                if (queuedFeatures.Count == 0 && queuedActions.Count == 0)
                {
                    if (TraceStateChanges)
                        tracer.TraceVerbose("========================== Evaluating Conditions for '{0}.{1}'.",
                            currentFeatureInstance.InstanceName,
                            "Workflow");
                    EnqueueAll<IFeatureExtension>(queuedFeatures, this.featureManager.InstantiatedFeatures);
                    return false;
                }
                else
                {
                    if (queuedActions.Count == 0)
                    {
                        var feature = queuedFeatures.Dequeue();
                        this.currentFeature = feature.InstanceName;

                        var workflow = feature.GuidanceWorkflow;
                        if (workflow != null)
                        {
                            if (workflow.Successors.ToArray<INode>()[0].Name == "Trace")
                                TraceStateChanges = true;
                            EnqueueAll(queuedActions, workflow.AllNodesToEvaluate);

                            EvaluateNode(workflow);
                        }
                        return true;
                    }

                    var conditionalNode = queuedActions.Dequeue();
                    EvaluateNode(conditionalNode);
                    return true;
                }
            }

            return true;
        }

        private static void EnqueueAll<T>(Queue<T> queue, IEnumerable<T> items)
        {
            foreach (var feature in items)
            {
                queue.Enqueue(feature);
            }
        }

        private static IList<IConditionalNode> NodesToEvaluate(IGuidanceWorkflow w)
        {
            return w.AllNodesToEvaluate;
        }


        /// <summary>
        /// Evaluates the pre and post conditions of the node.
        /// </summary>
        public static void EvaluateNode(IConditionalNode node)
        {
            //
            // Here is where we do the actual work of calling the
            // conditions.
            //
            // First, set the node into the FeatureCallContext so the
            // conditions can know
            //
            FeatureCallContext.Current.DefaultConditionTarget = node as INode;

            //
            // Then, check the preconditions.
            // Note: We do not automatically set the result of
            // precondition calculation for Fork/Join/Decision/Merge
            // to enable them full latitude in state management
            //
            if (!EvaluateBindings(node.Preconditions))
            {
                //if (!(node is Fork) &&
                //    !(node is Join) &&
                //    !(node is Decision) &&
                //    !(node is Merge))
                if (TraceStateChanges &&
                    node.State != NodeState.Blocked)
                    tracer.TraceVerbose("Setting state to BLOCKED '{0}.{1}'.",
                        currentFeatureInstance.InstanceName,
                        node.Name);
                node.SetState(NodeState.Blocked, false);
            }
            else
            {
                NodeState postConditionsState = NodeState.Enabled;

                if (currentFeatureInstance != null)
                {
                    if ((currentFeatureInstance.GuidanceWorkflow != null) &&
                    ((GuidanceWorkflow)currentFeatureInstance.GuidanceWorkflow).IgnorePostConditions)
                        postConditionsState = NodeState.Enabled;
                    else
                        postConditionsState = EvaluateBindings(node.Postconditions) ? NodeState.Completed : NodeState.Enabled;
                }
                else if (GuidanceConditionsEvaluator.Current != null &&
                         GuidanceConditionsEvaluator.Current.featureManager != null &&
                         GuidanceConditionsEvaluator.Current.featureManager.ActiveFeature != null)
                {
                    if ((GuidanceConditionsEvaluator.Current.featureManager.ActiveFeature.GuidanceWorkflow != null) &&
                    ((GuidanceWorkflow)GuidanceConditionsEvaluator.Current.featureManager.ActiveFeature.GuidanceWorkflow).IgnorePostConditions)
                        postConditionsState = NodeState.Enabled;
                    else
                        postConditionsState = EvaluateBindings(node.Postconditions) ? NodeState.Completed : NodeState.Enabled;
                }
                else
                    postConditionsState = EvaluateBindings(node.Postconditions) ? NodeState.Completed : NodeState.Enabled;

                //
                // Same rule for post condtions, let the "big guys"
                // calculate state for themselves
                //
                //if (!(node is Fork) &&
                //    !(node is Join) &&
                //    !(node is Decision) &&
                //    !(node is Merge))
                if (TraceStateChanges &&
                    node.State != postConditionsState)
                    tracer.TraceVerbose("Setting state to " + postConditionsState.ToString() + " '{0}.{1}'.",
                        currentFeatureInstance.InstanceName,
                        node.Name);
                node.SetState(postConditionsState, false);
            }
        }

        private static bool EvaluateBindings(IEnumerable<IBinding<ICondition>> bindings)
        {
            return !bindings.Any() || bindings.All(binding => binding.Evaluate());
        }
    }
}