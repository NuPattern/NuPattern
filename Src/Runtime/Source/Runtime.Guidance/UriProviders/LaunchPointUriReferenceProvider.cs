using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using NuPattern.ComponentModel.Composition;
using NuPattern.Diagnostics;
using NuPattern.Runtime.Composition;

namespace NuPattern.Runtime.Guidance.UriProviders
{
    /// <summary>
    /// A <see cref="IUriReferenceProvider"/> that resolves and creates Uris from 
    /// <see cref="ILaunchPoint"/> instances. The Uri format is: 
    /// Example: <c>launchpoint://VSIXID/MetadataId</c>.
    /// </summary>
    // TODO: Fix FERT so that this class does not have ot be public to register itself.
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IUriReferenceProvider))]
    public class LaunchPointUriReferenceProvider : IUriReferenceProvider<ILaunchPoint>
    {
        private const string UriFormat = LaunchPointUri.HostFormat + "{ExtensionId}/{ExportId}";
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<LaunchPointUriReferenceProvider>();

        private Dictionary<Uri, ILaunchPoint> launchPoints;

        [Import]
        private IGuidanceManager GuidanceManager { get; set; }

        [Import]
        private INuPatternCompositionService CompositionService { get; set; }

        /// <summary>
        /// Gets the URI scheme.
        /// </summary>
        /// <value>The URI scheme.</value>
        public string UriScheme
        {
            get { return LaunchPointUri.UriScheme; }
        }

        /// <summary>
        /// Creates the URI.
        /// </summary>
        public Uri CreateUri(ILaunchPoint instance)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Resolves the URI
        /// </summary>
        public ILaunchPoint ResolveUri(Uri uri)
        {
            if (launchPoints == null)
            {
                this.Initialize();
            }

            ILaunchPoint launchPoint;
            if (launchPoints.TryGetValue(uri, out launchPoint))
            {
                return launchPoint;
            }

            return null;
        }

        /// <summary>
        /// Opens the URI
        /// </summary>
        public void Open(ILaunchPoint instance)
        {
            throw new NotImplementedException();
        }

        private void Initialize()
        {
            var exports = this.CompositionService.GetExports<ILaunchPoint, IComponentMetadata>();
            this.launchPoints = new Dictionary<Uri, ILaunchPoint>();
            foreach (var export in exports)
            {
                var launchPoint = export.Value;
                var extension = this.GuidanceManager.FindGuidanceExtension(launchPoint.GetType());

                var uri = new Uri(UriFormat.NamedFormat(new
                {
                    ExtensionId = extension.ExtensionId,
                    ExportId = export.Metadata.Id,
                }));

                tracer.TraceVerbose("Registering launch point uri {0}", uri);
                this.launchPoints.Add(uri, launchPoint);
            }
        }
    }
}