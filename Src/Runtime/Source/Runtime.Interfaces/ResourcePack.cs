using System;
using System.IO;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using NuPattern.Runtime.Interfaces;

namespace NuPattern.Runtime
{
	/// <summary>
    /// Represents a resource
	/// </summary>
	[CLSCompliant(false)]
	public class ResourcePack
	{
        private IItem item;
        private IAssemblyResource resource;
        private string assemblyName;
        private string resourcePath;

		/// <summary>
		/// Initializes a new instance of the <see cref="ResourcePack"/> class.
		/// </summary>
		/// <param name="item">The project <see cref="IItem"/> to wrap</param>
		public ResourcePack(IItem item)
		{
            Guard.NotNull(() => item, item);

            this.item = item;
            this.resource = null;

            var project = item.GetContainingProject();
            this.assemblyName = project.Data.AssemblyName;
            this.resourcePath = GetProjectItemPath(item, project);
		}
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourcePack"/> class.
        /// </summary>
        /// <param name="resource">The project <see cref="IAssemblyResource"/> to wrap</param>
        public ResourcePack(IAssemblyResource resource)
        {
            Guard.NotNull(() => resource, resource);

            this.item = null;
            this.resource = resource;
            this.assemblyName = resource.Assembly.AssemblyName;
            this.resourcePath = resource.Name;
        }

        /// <summary>
        /// Gets the name of the assembly.
        /// </summary>
        public string AssemblyName
        {
            get
            {
                return this.assemblyName;
            }
        }

        /// <summary>
        /// Gets the path of the resource.
        /// </summary>
        public string ResourcePath
        {
            get
            {
                return this.resourcePath;
            }
        }

        /// <summary>
        /// Gets the type of the pack.
        /// </summary>
        public ResourcePackType Type
        {
            get
            {
                if (this.item != null)
                {
                    return ResourcePackType.ProjectItem;
                }
                else if (this.resource != null)
                {
                    return ResourcePackType.AssemblyReference;
                }
                else
                {
                    throw new InvalidOperationException(Resources.ResourcePack_Error_UnknownResourceType);
                }
            }
        }

		/// <summary>
        /// Gets or sets the project <see cref="IItem"/> that is the resource.
		/// </summary>
        public IItem GetItem()
        {
            if (this.Type != ResourcePackType.ProjectItem)
            {
                throw new InvalidOperationException(Resources.ResourcePack_Error_InvalidResourceType);
            }
            else
            {
                return this.item;
            }
        }

        /// <summary>
        /// Gets or sets the project <see cref="IAssemblyResource"/> that is the resource.
        /// </summary>
        public IAssemblyResource GetAssemblyResource()
        {
            if (this.Type != ResourcePackType.AssemblyReference)
            {
                throw new InvalidOperationException(Resources.ResourcePack_Error_InvalidResourceType);
            }
            else
            {
                return this.resource;
            }
        }

        private string GetProjectItemPath(IItem item, IProject project)
        {
            var path = string.Empty;
            var projectPath = Path.GetDirectoryName(project.PhysicalPath);
            var itemPath = item.PhysicalPath;

            // Is the item contained in the project?
            if (itemPath.StartsWith(projectPath))
            {
                path = itemPath.Substring(projectPath.Length);
            }
            else
            {
                // The item must be linked in from outside the project
                var projectLogicalPath = project.GetLogicalPath();
                var itemLogicalPath = item.GetLogicalPath();
                path = (itemLogicalPath.Substring(projectLogicalPath.Length));
            }

            return path.Replace('\\', '/').TrimStart('/');
        }
    }

    /// <summary>
    /// The types of resource packs.
    /// </summary>
    public enum ResourcePackType
    {
        /// <summary>
        /// The resource is declared in a project
        /// </summary>
        ProjectItem = 0,

        /// <summary>
        /// The resource is declared in a project assembly reference
        /// </summary>
        AssemblyReference = 1
    }
}
