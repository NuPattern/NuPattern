using NuPattern.Runtime.Bindings;

namespace NuPattern.Runtime.Guidance
{
    /// <summary>
    /// A binding to a guidance extension command
    /// </summary>
    public interface ICommandBinding : IBinding<ICommand>
    {
        /// <summary>
        /// Gets or sets the name of the binding.
        /// </summary>
        string Name { get; set; }
    }
}