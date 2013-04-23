namespace NuPattern.VisualStudio.Solution.Templates
{
    /// <summary>
    /// Custom template parameter
    /// </summary>
    public interface IVsTemplateCustomParameter
    {
        /// <summary>
        /// Gets the name of the parameter
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the value of thee parameter
        /// </summary>
        string Value { get; }
    }
}