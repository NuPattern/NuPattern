using System.Collections.Generic;

namespace NuPattern.Runtime
{
    /// <summary>
    /// Provides the info for a pattern model.
    /// </summary>
    public partial interface IPatternModelInfo
    {
        /// <summary>
        /// Gets instances of a given type.
        /// </summary>
        IEnumerable<T> FindAll<T>();
    }
}