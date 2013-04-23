
namespace NuPattern.ComponentModel
{
    /// <summary>
    /// Component model information gathered from an exported component.
    /// </summary>
    /// <remarks>
    /// Import this contract type to perform introspection on exported 
    /// components that have opted-in for component model attribute 
    /// reflection.
    /// </remarks>
    public interface IComponentModelInfo
    {
        /// <summary>
        /// Gets a value indicating whether the component is browsable.
        /// </summary>
        bool IsBrowsable { get; }

        /// <summary>
        /// Gets the display name of the component.
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// Gets the description of the component.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets the category of the component.
        /// </summary>
        string Category { get; }
    }
}
