namespace NuPattern.Runtime.Guidance.Workflow
{
    /// <summary>
    /// The source of a state change
    /// </summary>
    public enum StateChangeSource
    {
        /// <summary>
        /// The source was a predecessor
        /// </summary>
        Predecessor,

        /// <summary>
        /// The source was a successor
        /// </summary>
        Successor
    }
}