namespace NuPattern.Runtime
{
    /// <summary>
    /// A condition used in automation
    /// </summary>
    [Condition]
    public abstract class Condition : ICondition
    {
        /// <summary>
        /// Evaluates the condition
        /// </summary>
        /// <returns></returns>
        public abstract bool Evaluate();
    }
}