using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using NuPattern.ComponentModel.Composition;
using NuPattern.Diagnostics;
using NuPattern.Runtime.Composition;

namespace NuPattern.Runtime.Guidance.UriProviders
{
    [Export(typeof(IUriReferenceProvider))]
    internal class LaunchPointUriReferenceProvider : IUriReferenceProvider<ILaunchPoint>
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<LaunchPointUriReferenceProvider>();
        public const string Scheme = "launchpoint";

        private Dictionary<Uri, ILaunchPoint> launchPoints;

        [Import]
        private IGuidanceManager GuidanceManager { get; set; }

        [Import]
        private INuPatternCompositionService CompositionService { get; set; }

        public string UriScheme
        {
            get { return Scheme; }
        }

        public Uri CreateUri(ILaunchPoint instance)
        {
            throw new NotImplementedException();
        }

        public void Open(ILaunchPoint instance)
        {
            throw new NotImplementedException();
        }

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

        private void Initialize()
        {
            var exports = this.CompositionService.GetExports<ILaunchPoint, IComponentMetadata>();
            this.launchPoints = new Dictionary<Uri, ILaunchPoint>();
            foreach (var export in exports)
            {
                var launchPoint = export.Value;
                var feature = this.GuidanceManager.FindGuidanceExtension(launchPoint.GetType());
                var uri = new Uri(this.UriScheme + Uri.SchemeDelimiter + feature.ExtensionId + "/" + export.Metadata.Id);

                tracer.TraceVerbose("Registering launch point uri {0}", uri);
                this.launchPoints.Add(uri, launchPoint);
            }
        }
    }
}