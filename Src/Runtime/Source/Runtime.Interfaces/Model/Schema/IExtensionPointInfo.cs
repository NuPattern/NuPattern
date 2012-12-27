using System;
using System.Collections.Generic;

namespace NuPattern.Runtime
{
    [CLSCompliant(false)]
    public partial interface IExtensionPointInfo : IContainedElementInfo
    {
        /// <summary>
        /// Gets the condition settings.
        /// </summary>
        IEnumerable<IBindingSettings> ConditionSettings { get; }
    }
}