namespace NuPattern.VisualStudio.Solution
{
    /// <summary>
    /// A template that can be unfolded into a parent container.
    /// </summary>
    public interface ITemplate
    {
        /// <summary>
        /// Unfolds the template in the given parent.
        /// </summary>
        /// <returns>The unfolded item (which may contain sub-items, such as in a project or a multi-file item template).</returns>
        IItemContainer Unfold(string name, IItemContainer parent);

        /// <summary>
        /// Parameters to pass to the template, using dynamic property syntax 
        /// (i.e. solution.Data.MyValue = "Hello"). 
        /// </summary>
        dynamic Parameters { get; }
    }
}
