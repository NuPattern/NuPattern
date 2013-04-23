namespace NuPattern.Runtime.Guidance.Workflow
{
    /// <summary>
    /// Defines a state composer in a guidance workflow
    /// </summary>
    public interface IStateComposer
    {
        /// <summary>
        /// Composes the state of the node if a state change in a predecessor or succesor happen.
        /// </summary>
        /// <param name="node">The node to compose the state.</param>
        /// <param name="changeSource">A member of <see cref="StateChangeSource"/> that indicates if the state change
        /// was raised by a predecessor or a successor.</param>
        /// <returns>The new state of the node.</returns>
        NodeState ComposeState(INode node, StateChangeSource changeSource);
    }
}