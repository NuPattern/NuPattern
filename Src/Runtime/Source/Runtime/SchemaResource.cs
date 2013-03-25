using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.ExtensionManager;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using NuPattern.Runtime.Properties;

namespace NuPattern.Runtime
{
    /// <summary>
    /// Default implementation of a <see cref="ISchemaResource"/> 
    /// which resides in the <see cref="IExtensionContent"/> 
    /// part of an extension.
    /// </summary>
    internal class SchemaResource : ISchemaResource
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<SchemaResource>();
        internal const string AssemblyFileProperty = "AssemblyFile";

        private Assembly schemaAssembly;

        /// <summary>
        /// Initializes a new instance of the <see cref="SchemaResource"/> class from the given context.
        /// </summary>
        /// <param name="extensionPath">The path for the extension.</param>
        /// <param name="content">The content.</param>
        public SchemaResource(string extensionPath, IExtensionContent content)
        {
            NuPattern.Guard.NotNullOrEmpty(() => extensionPath, extensionPath);
            NuPattern.Guard.NotNull(() => content, content);

            string assemblyFile = null;
            if (content.Attributes == null || !content.Attributes.TryGetValue(AssemblyFileProperty, out assemblyFile))
            {
                throw new ArgumentException(Resources.SchemaResource_AssemblyFileAttributeMissing);
            }

            this.AssemblyPath = Path.Combine(extensionPath, assemblyFile);

            var assemblyName = AssemblyName.GetAssemblyName(this.AssemblyPath);
            this.schemaAssembly = Assembly.Load(assemblyName);

            var resourceNames = this.schemaAssembly.GetManifestResourceNames();

            if (resourceNames.Contains(content.RelativePath))
            {
                this.ResourceName = content.RelativePath;
            }
            else if (resourceNames.Contains(assemblyName.Name + "." + content.RelativePath))
            {
                this.ResourceName = assemblyName.Name + "." + content.RelativePath;
            }
            else
            {
                var message = string.Format(CultureInfo.CurrentCulture,
                    Resources.SchemaResource_FailedToLoadSchemaStream,
                    content.RelativePath,
                    assemblyName.Name + "." + this.ResourceName,
                    this.AssemblyPath);

                tracer.TraceError(message);
                throw new InvalidOperationException(message);
            }
        }

        /// <summary>
        /// Gets the assembly where the schema resides.
        /// </summary>
        public string AssemblyPath { get; private set; }

        /// <summary>
        /// Gets the name of the embedded schema resource.
        /// </summary>
        public string ResourceName { get; private set; }

        /// <summary>
        /// Creates a stream out of the resource so that it can be read.
        /// </summary>
        public Stream CreateStream()
        {
            var assemblyName = AssemblyName.GetAssemblyName(this.AssemblyPath);
            var factoryAssembly = Assembly.Load(assemblyName);

            var stream = factoryAssembly.GetManifestResourceStream(this.ResourceName);
            if (stream == null)
                stream = factoryAssembly.GetManifestResourceStream(assemblyName.Name + "." + this.ResourceName);

            if (stream == null)
            {
                var message = string.Format(CultureInfo.CurrentCulture,
                    Resources.SchemaResource_FailedToLoadSchemaStream,
                    this.ResourceName,
                    assemblyName.Name + "." + this.ResourceName,
                    this.AssemblyPath);

                tracer.TraceError(message);
                throw new InvalidOperationException(message);
            }

            return stream;
        }
    }
}