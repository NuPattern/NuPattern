namespace NuPattern.VisualStudio.Solution.Templates
{
    /// <summary>
    /// A resource or value
    /// </summary>
    public interface IVsTemplateResourceOrValue
    {
        /// <summary>
        /// Gets the identifier of the value.
        /// </summary>
        string ID { get; }

        /// <summary>
        /// Gets the package of the value.
        /// </summary>
        string Package { get; }

        /// <summary>
        /// Gets the actual value.
        /// </summary>
        string Value { get; }

        /// <summary>
        /// Gets a value to indicate whether the value is assigned.
        /// </summary>
        bool HasValue { get; }
    }
}