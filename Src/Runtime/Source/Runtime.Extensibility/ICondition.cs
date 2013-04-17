namespace Microsoft.VisualStudio.TeamArchitect.PowerTools.Features
{
    /// <summary>
    /// A condition.
    /// </summary>
    public interface ICondition
    {
        /// <summary>
        /// Evaluates the condition.
        /// </summary>
        bool Evaluate();
    }
}