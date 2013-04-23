using System.Collections.Generic;
using NuPattern.Runtime.Bindings;

namespace NuPattern.Runtime
{
    public partial interface IPropertyInfo
    {
        /// <summary>
        /// Gets the validation settings.
        /// </summary>
        IEnumerable<IBindingSettings> ValidationSettings { get; }

        /// <summary>
        /// Gets the default value settings. This property never returns <see langword="null"/>.
        /// </summary>
        IPropertyBindingSettings DefaultValue { get; }

        /// <summary>
        /// Gets the value provider settings. This property never returns <see langword="null"/>.
        /// </summary>
        IValueProviderBindingSettings ValueProvider { get; }
    }
}