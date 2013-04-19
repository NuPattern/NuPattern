using System;
using System.ComponentModel.Composition;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.Shell;
using NuPattern.Diagnostics;

namespace NuPattern.Runtime.Guidance.UriProviders
{
    [Export(typeof(IUriReferenceProvider))]
    internal class GuidanceContentUriReferenceProvider : IUriReferenceProvider<GuidanceContent>
    {
        static readonly ITraceSource tracer = Tracer.GetSourceFor<GuidanceContentUriReferenceProvider>();
        public const string Scheme = "content";

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

        public string UriScheme
        {
            get { return Scheme; }
        }

        public Uri CreateUri(GuidanceContent instance)
        {
            tracer.TraceVerbose("Creating uri for content with feature id {0}, path {1}", instance.GuidanceExtensionId, instance.Path);

            var feature = this.guidanceManager.InstalledGuidanceExtensions
                .FirstOrDefault(installedFeature => installedFeature.ExtensionId.Equals(instance.GuidanceExtensionId, StringComparison.InvariantCultureIgnoreCase));
            if (feature == null)
                throw new ArgumentException(string.Format("Feature '{0}' not found", instance.GuidanceExtensionId));

            var path = instance.Path.Replace(feature.InstallPath, string.Empty).Replace("\\", "/");
            var uri = new Uri(UriScheme + Uri.SchemeDelimiter + feature.ExtensionId + path);
            tracer.TraceVerbose("Created uri {0}", uri);

            return uri;
        }

        public GuidanceContent ResolveUri(Uri uri)
        {
            tracer.TraceVerbose("Resolving uri {0}", uri);
            var extensionId = uri.Host;

            if (extensionId == "." && guidanceManager.ActiveGuidanceExtension != null)
                extensionId = guidanceManager.ActiveGuidanceExtension.ExtensionId;

            var feature = this.guidanceManager.InstalledGuidanceExtensions
                .FirstOrDefault(installedFeature => installedFeature.ExtensionId.Equals(extensionId, StringComparison.InvariantCultureIgnoreCase));
            if (feature == null)
                throw new ArgumentException(string.Format("Guidance extension '{0}' not found", extensionId));

            var path = Path.Combine(feature.InstallPath, Uri.UnescapeDataString(uri.PathAndQuery.Substring(1))
                .Replace("/", @"\"));

            if (!File.Exists(path))
                throw new FileNotFoundException(string.Format(CultureInfo.CurrentCulture, "File '{0}' does not exist.", path));

            tracer.TraceVerbose("Resolved to path {0}", path);
            return new GuidanceContent(extensionId, path);
        }

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