namespace NuPattern.VisualStudio.Solution
{
    /// <summary>
    /// The scope of an operation that works based on the current 
    /// Visual Studio solution.
    /// </summary>
    public enum Scope
    {
        /// <summary>
        /// The operation applies to the entire solution.
        /// </summary>
        Solution,

        /// <summary>
        /// The operation applies to the current selection in the 
        /// solution explorer.
        /// </summary>
        Selection
    }
}
