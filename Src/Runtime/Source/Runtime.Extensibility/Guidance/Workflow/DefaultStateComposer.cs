namespace NuPattern.Runtime.Guidance.Workflow
{
    /// <summary>
    /// State composer that blocks a node until all its predecessors 
    /// are in the <see cref="NodeState.Completed"/> state.
    /// </summary>
    internal class DefaultStateComposer : IStateComposer
    {
        private static readonly IStateComposer instance = new DefaultStateComposer();

        private DefaultStateComposer()
        {
        }

        public static IStateComposer Instance
        {
            get { return instance; }
        }

        public NodeState ComposeState(INode node, StateChangeSource changeSource)
        {
            Guard.NotNull(() => node, node);

#if ImplicitPredecessorsCompleteCondition
            if (changeSource == StateChangeSource.Predecessor && node.Predecessors.Any())
            {
                return node.Predecessors.All(n => n.State == NodeState.Completed) ? NodeState.Completed : NodeState.Blocked;
            }
#endif
            // Ignores successor changes.
            return node.State;
        }
    }
}