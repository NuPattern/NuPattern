using NuPattern.Runtime.Bindings;

namespace NuPattern.Library.Automation
{
    /// <summary>
    /// Adds the <see cref="IBindingSettings"/> implementation to the interface.
    /// </summary>
    public partial interface ICommandSettings : IBindingSettings
    {
    }

    /// <summary>
    /// Wrapper for ICommandSettings used in chaining.
    /// </summary>
    internal interface ICommandSettings<T> : ICommandSettings
    {
    }
}
