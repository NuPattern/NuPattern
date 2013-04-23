using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using NuPattern.Diagnostics;
using NuPattern.Runtime.Properties;
using NuPattern.VisualStudio.Extensions;

namespace NuPattern.Runtime
{
    /// <summary>
    /// Adapter class that exposes installed toolkits from the <see cref="IExtensionManager"/> 
    /// as <see cref="IInstalledToolkitInfo"/> instances.
    /// </summary>
    [PartCreationPolicy(CreationPolicy.Shared)]
    internal class InstalledToolkitAdapter
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<InstalledToolkitAdapter>();

        /// <summary>
        /// Initializes a new instance of the <see cref="InstalledToolkitAdapter"/> class.
        /// </summary>
        /// <param name="extensionManager">The <see cref="IExtensionManager"/>.</param>
        /// <param name="reader">The <see cref="ISchemaReader"/>.</param>
        [ImportingConstructor]
        public InstalledToolkitAdapter(
            [Import(typeof(IExtensionManager))] IExtensionManager extensionManager,
            [Import(typeof(ISchemaReader))] ISchemaReader reader)
        {
            this.InstalledToolkits = GetInstalledToolkits(extensionManager, reader);
        }

        /// <summary>
        /// Gets the installed toolkits.
        /// </summary>
        [Export]
        public IEnumerable<IInstalledToolkitInfo> InstalledToolkits { get; private set; }

        private static IEnumerable<IInstalledToolkitInfo> GetInstalledToolkits(IExtensionManager extensionManager, ISchemaReader reader)
        {
            Guard.NotNull(() => extensionManager, extensionManager);

            return extensionManager.GetInstalledExtensions()
                .Where(extension => IsToolkit(extension))
                .Select(extension => TryCreateRegistration(reader, extension))
                .Where(registration => registration != null);
        }

        private static bool IsToolkit(IInstalledExtension extension)
        {
            return extension.Content.Any(c => c.ContentTypeName.Equals(InstalledToolkitInfo.PatternModelCustomExtensionName, StringComparison.OrdinalIgnoreCase));
        }

        private static InstalledToolkitInfo TryCreateRegistration(ISchemaReader reader, IInstalledExtension extension)
        {
            try
            {
                var resource = AsSchemaResource(extension);

                return new InstalledToolkitInfo(extension, reader, resource);
            }
            catch (Exception ex)
            {
                if (Microsoft.VisualStudio.ErrorHandler.IsCriticalException(ex))
                    throw;

                tracer.TraceWarning(Resources.InstalledToolkitAdapter_FailedToCreateRegistration, extension.InstallPath);
                return null;
            }
        }

        private static ISchemaResource AsSchemaResource(IInstalledExtension extension)
        {
            Guard.NotNull(() => extension, extension);

            var content = extension.Content
                .SingleOrDefault(c => c.ContentTypeName.Equals(InstalledToolkitInfo.PatternModelCustomExtensionName, StringComparison.OrdinalIgnoreCase));

            return content != null ? new SchemaResource(extension.InstallPath, content) : null;
        }
    }
}