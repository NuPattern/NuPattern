using System;
using System.ComponentModel.Composition;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.Shell;
using NuPattern.Diagnostics;

namespace NuPattern.Runtime.Guidance.UriProviders
{
    /// <summary>
    /// A <see cref="IUriReferenceProvider"/> that resolves and creates Uris from 
    /// <see cref="GuidanceContent"/> instances. The Uri format is: 
    /// Example: <c>content://[VSIXID]/path</c>.
    /// </summary>
    // TODO: Fix FERT so that this class does not have ot be public to register itself.
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IUriReferenceProvider))]
    public class GuidanceContentUriReferenceProvider : IUriReferenceProvider<GuidanceContent>
    {
        private const string UriFormat = GuidanceContentUri.HostFormat + "{ExtensionId}/{Path}";
        static readonly ITraceSource tracer = Tracer.GetSourceFor<GuidanceContentUriReferenceProvider>();

        private IServiceProvider ServiceProvider { get; set; }
        private IGuidanceManager guidanceManager { get; set; }

        [ImportingConstructor]
        internal GuidanceContentUriReferenceProvider(SVsServiceProvider serviceProvider, IGuidanceManager guidanceManager)
        {
            Guard.NotNull(() => serviceProvider, serviceProvider);
            Guard.NotNull(() => guidanceManager, guidanceManager);

            this.ServiceProvider = serviceProvider;
            this.guidanceManager = guidanceManager;
        }

        /// <summary>
        /// Gets the URI scheme.
        /// </summary>
        /// <value>The URI scheme.</value>
        public string UriScheme
        {
            get { return GuidanceContentUri.UriScheme; }
        }

        /// <summary>
        /// Creates the URI.
        /// </summary>
        public Uri CreateUri(GuidanceContent instance)
        {
            tracer.TraceVerbose("Creating uri for content with feature id {0}, path {1}", instance.GuidanceExtensionId, instance.Path);

            var extension = this.guidanceManager.InstalledGuidanceExtensions
                .FirstOrDefault(installedFeature => installedFeature.ExtensionId.Equals(instance.GuidanceExtensionId, StringComparison.InvariantCultureIgnoreCase));
            if (extension == null)
                throw new ArgumentException(string.Format("Feature '{0}' not found", instance.GuidanceExtensionId));

            var path = instance.Path.Replace(extension.InstallPath, string.Empty).Replace("\\", "/");

            var uri = new Uri(UriFormat.NamedFormat(new
            {
                ExtensionId = extension.ExtensionId,
                Path = path,
            }));

            tracer.TraceVerbose("Created uri {0}", uri);

            return uri;
        }

        /// <summary>
        /// Resolves the URI
        /// </summary>
        public GuidanceContent ResolveUri(Uri uri)
        {
            tracer.TraceVerbose("Resolving uri {0}", uri);
            var extensionId = uri.Host;

            if (extensionId == "." && guidanceManager.ActiveGuidanceExtension != null)
                extensionId = guidanceManager.ActiveGuidanceExtension.ExtensionId;

            var extension = this.guidanceManager.InstalledGuidanceExtensions
                .FirstOrDefault(installedFeature => installedFeature.ExtensionId.Equals(extensionId, StringComparison.InvariantCultureIgnoreCase));
            if (extension == null)
                throw new ArgumentException(string.Format("Guidance extension '{0}' not found", extensionId));

            var path = Path.Combine(extension.InstallPath, Uri.UnescapeDataString(uri.PathAndQuery.Substring(1))
                .Replace("/", @"\"));

            if (!File.Exists(path))
                throw new FileNotFoundException(string.Format(CultureInfo.CurrentCulture, "File '{0}' does not exist.", path));

            tracer.TraceVerbose("Resolved to path {0}", path);
            return new GuidanceContent(extensionId, path);
        }

        /// <summary>
        /// Opens the URI
        /// </summary>
        public void Open(GuidanceContent instance)
        {
            tracer.TraceVerbose("Opening content ", instance.Path);
            dynamic vs = this.ServiceProvider.GetService(typeof(EnvDTE.DTE));

            if (vs != null)
            {
                vs.ItemOperations.OpenFile(instance.Path);
            }
        }
    }
}