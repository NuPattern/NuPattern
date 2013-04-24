using System;
using System.Collections.Generic;
using System.Linq;
using NuPattern.Diagnostics;
using NuPattern.Runtime.Bindings;
using NuPattern.Runtime.Guidance.Properties;

namespace NuPattern.Runtime.Guidance.Workflow
{
    internal class GuidanceConditionsEvaluator
    {
        private const string DebugToken = @"DebugTrace";
        private const string DebugWithBindingsToken = @"DebugTraceWithBindings";
        private const string TraceToken = @"Trace";
        private static readonly ITracer tracer = Tracer.Get<GuidanceConditionsEvaluator>();
        public static bool TraceStateChanges = false;

        private IGuidanceManager guidanceManager;
        private Queue<IGuidanceExtension> queuedExtensions = new Queue<IGuidanceExtension>();
        private Queue<IConditionalNode> queuedActions = new Queue<IConditionalNode>();
        private string currentExtensionName;
        private static IGuidanceExtension currentExtensionInstance;
        private static GuidanceConditionsEvaluator Current;

        public GuidanceConditionsEvaluator(IGuidanceManager guidanceManager)
        {
            Guard.NotNull(() => guidanceManager, guidanceManager);

            this.guidanceManager = guidanceManager;
            Current = this;
        }

        public static void EvaluateGraph(IGuidanceExtension extension)
        {
            Guard.NotNull(() => extension, extension);

            currentExtensionInstance = extension;

            var workflow = extension.GuidanceWorkflow;
            if (workflow != null)
            {
                TraceStateChanges = GuidanceManagerSettings.VerboseTracing = workflow.Successors.ToArray<INode>()[0].Name.StartsWith(DebugToken);
                GuidanceManagerSettings.VerboseBindingTracing = workflow.Successors.ToArray<INode>()[0].Name.Equals(DebugWithBindingsToken, StringComparison.OrdinalIgnoreCase);

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
            if (this.guidanceManager.IsOpened)
            {
                if (queuedExtensions.Count == 0 && queuedActions.Count == 0)
                {
                    if (TraceStateChanges)
                        tracer.Verbose(Resources.GuidanceConditionsEvaluator_TraceEvaluationHeader,
                            currentExtensionInstance.InstanceName,
                            @"Workflow");
                    EnqueueAll<IGuidanceExtension>(queuedExtensions, this.guidanceManager.InstantiatedGuidanceExtensions);
                    return false;
                }
                else
                {
                    if (queuedActions.Count == 0)
                    {
                        var feature = queuedExtensions.Dequeue();
                        this.currentExtensionName = feature.InstanceName;

                        var workflow = feature.GuidanceWorkflow;
                        if (workflow != null)
                        {
                            if (workflow.Successors.ToArray<INode>()[0].Name.Equals(TraceToken, StringComparison.OrdinalIgnoreCase))
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
            GuidanceCallContext.Current.DefaultConditionTarget = node as INode;

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
                    tracer.Verbose(Resources.GuidanceConditionsEvaluator_TraceBlockState,
                        currentExtensionInstance.InstanceName,
                        node.Name);
                node.SetState(NodeState.Blocked, false);
            }
            else
            {
                NodeState postConditionsState = NodeState.Enabled;

                if (currentExtensionInstance != null)
                {
                    if ((currentExtensionInstance.GuidanceWorkflow != null) &&
                    ((GuidanceWorkflow)currentExtensionInstance.GuidanceWorkflow).IgnorePostConditions)
                        postConditionsState = NodeState.Enabled;
                    else
                        postConditionsState = EvaluateBindings(node.Postconditions) ? NodeState.Completed : NodeState.Enabled;
                }
                else if (GuidanceConditionsEvaluator.Current != null &&
                         GuidanceConditionsEvaluator.Current.guidanceManager != null &&
                         GuidanceConditionsEvaluator.Current.guidanceManager.ActiveGuidanceExtension != null)
                {
                    if ((GuidanceConditionsEvaluator.Current.guidanceManager.ActiveGuidanceExtension.GuidanceWorkflow != null) &&
                    ((GuidanceWorkflow)GuidanceConditionsEvaluator.Current.guidanceManager.ActiveGuidanceExtension.GuidanceWorkflow).IgnorePostConditions)
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
                    tracer.Verbose(Resources.GuidanceConditionsEvaluator_TraceStateToPostConditions + postConditionsState.ToString() + @" '{0}.{1}'.",
                        currentExtensionInstance.InstanceName,
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