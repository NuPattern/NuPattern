using System.Collections.Generic;

namespace NuPattern.Runtime
{
    /// <summary>
    /// Represents an assembly reference
    /// </summary>
    public interface IAssemblyReference
    {
        /// <summary>
        /// Gets the name of the assembly.
        /// </summary>
        string AssemblyName { get; }

        /// <summary>
        /// Gets the path to the assembly.
        /// </summary>
        string Path { get; }

        /// <summary>
        /// Gets the resources of the assembly.
        /// </summary>
        IEnumerable<IAssemblyResource> Resources { get; }
    }
}
