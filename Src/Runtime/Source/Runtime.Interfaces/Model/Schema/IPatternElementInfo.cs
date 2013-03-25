using System.Collections.Generic;
using NuPattern.Runtime.Bindings;

namespace NuPattern.Runtime
{
    public partial interface IPatternElementInfo
    {
        /// <summary>
        /// Gets the validation settings.
        /// </summary>
        IEnumerable<IBindingSettings> ValidationSettings { get; }
    }
}