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
    internal class FeatureContentUriReferenceProvider : IUriReferenceProvider<FeatureContent>
    {
        static readonly ITraceSource tracer = Tracer.GetSourceFor<FeatureContentUriReferenceProvider>();
        public const string Scheme = "content";

        private IServiceProvider ServiceProvider { get; set; }
        private IFeatureManager FeatureManager { get; set; }

        [ImportingConstructor]
        internal FeatureContentUriReferenceProvider(SVsServiceProvider serviceProvider, IFeatureManager featureManager)
        {
            Guard.NotNull(() => serviceProvider, serviceProvider);
            Guard.NotNull(() => featureManager, featureManager);

            this.ServiceProvider = serviceProvider;
            this.FeatureManager = featureManager;
        }

        public string UriScheme
        {
            get { return Scheme; }
        }

        public Uri CreateUri(FeatureContent instance)
        {
            tracer.TraceVerbose("Creating uri for content with feature id {0}, path {1}", instance.FeatureId, instance.Path);

            var feature = this.FeatureManager.InstalledFeatures
                .FirstOrDefault(installedFeature => installedFeature.FeatureId.Equals(instance.FeatureId, StringComparison.InvariantCultureIgnoreCase));
            if (feature == null)
                throw new ArgumentException(string.Format("Feature '{0}' not found", instance.FeatureId));

            var path = instance.Path.Replace(feature.InstallPath, string.Empty).Replace("\\", "/");
            var uri = new Uri(UriScheme + Uri.SchemeDelimiter + feature.FeatureId + path);
            tracer.TraceVerbose("Created uri {0}", uri);

            return uri;
        }

        public FeatureContent ResolveUri(Uri uri)
        {
            tracer.TraceVerbose("Resolving uri {0}", uri);
            var featureId = uri.Host;

            if (featureId == "." && FeatureManager.ActiveFeature != null)
                featureId = FeatureManager.ActiveFeature.FeatureId;

            var feature = this.FeatureManager.InstalledFeatures
                .FirstOrDefault(installedFeature => installedFeature.FeatureId.Equals(featureId, StringComparison.InvariantCultureIgnoreCase));
            if (feature == null)
                throw new ArgumentException(string.Format("Feature '{0}' not found", featureId));

            var path = Path.Combine(feature.InstallPath, Uri.UnescapeDataString(uri.PathAndQuery.Substring(1))
                .Replace("/", @"\"));

            if (!File.Exists(path))
                throw new FileNotFoundException(string.Format(CultureInfo.CurrentCulture, "File '{0}' does not exist.", path));

            tracer.TraceVerbose("Resolved to path {0}", path);
            return new FeatureContent(featureId, path);
        }

        public void Open(FeatureContent instance)
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