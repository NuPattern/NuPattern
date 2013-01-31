
namespace NuPattern.Runtime
{
    /// <summary>
    /// Represents a resource of an assembly
    /// </summary>
    public interface IAssemblyResource
    {
        /// <summary>
        /// Gets the containing assembly.
        /// </summary>
        IAssemblyReference Assembly { get; }

        /// <summary>
        /// Gets the name of the resource
        /// </summary>
        string Name { get; }
    }
}
