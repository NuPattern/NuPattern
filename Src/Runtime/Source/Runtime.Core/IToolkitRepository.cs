using System.Collections.Generic;

namespace NuPattern.Runtime
{
    /// <summary>
    /// Defines the contract for a repository of extensions.
    /// </summary>
    internal interface IToolkitRepository
    {
        /// <summary>
        /// Gets the repository name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the list of extensions provided by the repository.
        /// </summary>
        IEnumerable<IToolkitInfo> Toolkits { get; }
    }
}