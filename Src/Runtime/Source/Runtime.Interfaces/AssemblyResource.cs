
namespace NuPattern.Runtime
{
    /// <summary>
    /// A resource embedded in an assembly
    /// </summary>
    internal class AssemblyResource : IAssemblyResource
    {
        private IAssemblyReference assembly;
        private string name;

        /// <summary>
        /// Creates a new instance of the <see cref="AssemblyResource"/> class.
        /// </summary>
        public AssemblyResource(IAssemblyReference assembly, string name)
        {
            Guard.NotNull(()=> assembly, assembly);
            Guard.NotNullOrEmpty(() => name, name);

            this.assembly = assembly;
            this.name = name;
        }

        /// <summary>
        /// Gets the name of the resource
        /// </summary>
        public string Name
        {
            get 
            { 
                return this.name; 
            }
        }

        /// <summary>
        /// Gets the containing assembly.
        /// </summary>
        public IAssemblyReference Assembly
        {
            get 
            { 
                return this.assembly; 
            }
        }
    }
}
