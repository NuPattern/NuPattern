using Microsoft.VisualStudio.Modeling;
using NuPattern.Runtime.Bindings;

namespace NuPattern.Library.Automation
{
    /// <summary>
    /// Adds the <see cref="IBindingSettings"/> implementation to the interface.
    /// </summary>
    public partial interface ICommandSettings : IBindingSettings
    {
        /// <summary>
        /// Store property
        /// </summary>
        Store Store { get; }
    }
}
