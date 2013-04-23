namespace NuPattern.Runtime
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