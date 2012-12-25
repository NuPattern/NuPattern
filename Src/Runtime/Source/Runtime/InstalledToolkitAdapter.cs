using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.VisualStudio.ExtensionManager;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using NuPattern.Runtime.Properties;

namespace NuPattern.Runtime
{
    /// <summary>
    /// Adapter class that exposes installed toolkits from the <see cref="IVsExtensionManager"/> 
    /// as <see cref="IInstalledToolkitInfo"/> instances.
    /// </summary>
    [CLSCompliant(false)]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class InstalledToolkitAdapter
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<InstalledToolkitAdapter>();
        internal const string CustomExtensionType = "PatternModel";

        /// <summary>
        /// Initializes a new instance of the <see cref="InstalledToolkitAdapter"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="reader">The schema reader.</param>
        [ImportingConstructor]
        public InstalledToolkitAdapter(
            [Import(typeof(SVsServiceProvider))] IServiceProvider serviceProvider,
            [Import(typeof(ISchemaReader))] ISchemaReader reader)
        {
            var extensionManager = serviceProvider.GetService<SVsExtensionManager, IVsExtensionManager>();

            this.InstalledToolkits = GetInstalledToolkits(extensionManager, reader);
        }

        /// <summary>
        /// Gets the installed toolkits.
        /// </summary>
        [Export]
        public IEnumerable<IInstalledToolkitInfo> InstalledToolkits { get; private set; }

        private static IEnumerable<IInstalledToolkitInfo> GetInstalledToolkits(IVsExtensionManager extensionManager, ISchemaReader reader)
        {
            Guard.NotNull(() => extensionManager, extensionManager);

            return extensionManager.GetInstalledExtensions()
                .Where(extension => IsToolkit(extension))
                .Select(extension => TryCreateRegistration(reader, extension))
                .Where(registration => registration != null);
        }

        private static bool IsToolkit(IInstalledExtension extension)
        {
            return extension.Content.Any(c => c.ContentTypeName.Equals(CustomExtensionType, StringComparison.OrdinalIgnoreCase));
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
                .SingleOrDefault(c => c.ContentTypeName.Equals(CustomExtensionType, StringComparison.OrdinalIgnoreCase));

            return content != null ? new SchemaResource(extension.InstallPath, content) : null;
        }
    }
}