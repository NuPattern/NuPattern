namespace NuPattern.Runtime.Guidance.Workflow
{
    /// <summary>
    /// Defines hte initial node in a guidance workflow.
    /// </summary>
    public interface IInitial : INode
    {
        /// <summary>
        /// Gets the initial action.
        /// </summary>
        IGuidanceAction InitialActivity { get; }
    }
}