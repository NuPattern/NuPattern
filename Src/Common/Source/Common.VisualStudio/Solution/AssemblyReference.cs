using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;

namespace NuPattern.VisualStudio.Solution
{
    /// <summary>
    /// Represents a reference to an assembly for an <see cref="IProject"/>.
    /// </summary>
    internal class AssemblyReference : IAssemblyReference
    {
        private string assemblyName;
        private string path;

        /// <summary>
        /// Creates a new instance of the <see cref="AssemblyReference"/> class.
        /// </summary>
        public AssemblyReference(string assemblyName, string path)
        {
            Guard.NotNullOrEmpty(() => assemblyName, assemblyName);
            Guard.NotNullOrEmpty(() => path, path);

            this.assemblyName = assemblyName;
            this.path = path;
        }

        /// <summary>
        /// Gets the assembly name.
        /// </summary>
        public string AssemblyName
        {
            get { return this.assemblyName; }
        }

        /// <summary>
        /// Gets the local path to the assembly.
        /// </summary>
        public string Path
        {
            get { return this.path; }
        }

        /// <summary>
        /// Gets the resources of the assembly.
        /// </summary>
        public IEnumerable<IAssemblyResource> Resources
        {
            get
            {
                return GetAssemblyResourceNames(this);
            }
        }

        private static IEnumerable<IAssemblyResource> GetAssemblyResourceNames(IAssemblyReference assemblyReference)
        {
            Guard.NotNull(() => assemblyReference, assemblyReference);

            var resources = new List<IAssemblyResource>();

            if (!String.IsNullOrEmpty(assemblyReference.Path))
            {
                if (File.Exists(assemblyReference.Path))
                {
                    try
                    {
                        var assembly = Assembly.ReflectionOnlyLoadFrom(assemblyReference.Path);
                        if (assembly != null)
                        {
                            // .NET resources
                            resources.AddRange(LoadResources(assembly, ".resources").Select(r => new AssemblyResource(assemblyReference, r)));
                            // XAML resources
                            resources.AddRange(LoadResources(assembly, ".g.resources").Select(r => new AssemblyResource(assemblyReference, r)));
                        }
                    }
                    catch (Exception)
                    {
                        // Ignore the exception
                    }
                }
            }

            return resources;
        }

        private static IEnumerable<string> LoadResources(Assembly assembly, string resourceSuffix)
        {
            var resources = new List<string>();

            string resName = assembly.GetName().Name + resourceSuffix;
            using (var stream = assembly.GetManifestResourceStream(resName))
            {
                if (stream != null)
                {
                    using (var reader = new System.Resources.ResourceReader(stream))
                    {
                        resources.AddRange(reader
                            .Cast<DictionaryEntry>()
                            .Select(entry => (string)entry.Key));
                    }
                }
            }

            return resources;
        }
    }
}
