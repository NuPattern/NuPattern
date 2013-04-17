using NuPattern.Runtime.Bindings;

namespace NuPattern.Runtime.Guidance
{
    /// <summary>
    /// A binding to a feature command
    /// </summary>
    public interface ICommandBinding : IBinding<IFeatureCommand>
    {
        /// <summary>
        /// Gets or sets the name of the binding.
        /// </summary>
        string Name { get; set; }
    }
}