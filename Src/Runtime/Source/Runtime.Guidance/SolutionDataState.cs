using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using NuPattern.VisualStudio.Solution;

namespace NuPattern.Runtime.Guidance
{
    [Export(typeof(ISolutionState))]
    internal class SolutionDataState : ISolutionState
    {
        public const string FeaturesProperty = "Features";

        private ISolution theSolution;

        [ImportingConstructor]
        public SolutionDataState(ISolution solution)
        {
            Guard.NotNull(() => solution, solution);

            this.Solution = solution;
        }

        public ISolution Solution
        {
            get
            {
                return theSolution;
            }

            private set
            {
                theSolution = value;
            }
        }

        public IEnumerable<ISolutionFeatureState> InstantiatedFeatures
        {
            get
            {
                var features = (string)Solution.Data.Features;
                if (String.IsNullOrEmpty(features))
                    return Enumerable.Empty<SolutionFeatureState>();

                Version version;
                return from feature in features.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries)
                       let state = feature.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries)
                       // We ignore whatever is mangled and invalid there
                       where state.Length == 3 &&
                            !String.IsNullOrEmpty(state[0]) &&
                            !String.IsNullOrEmpty(state[1]) &&
                            Version.TryParse(state[2], out version)
                       select new SolutionFeatureState
                       {
                           FeatureId = state[0],
                           InstanceName = state[1],
                           Version = new Version(state[2])
                       };
            }
        }

        public void AddFeature(string featureId, string instanceName, Version version)
        {
            var existingFeatures = InstantiatedFeatures.ToList();
            var thisFeature = existingFeatures.FirstOrDefault(f => f.FeatureId == featureId && f.InstanceName == instanceName && f.Version == version);
            if (thisFeature != null)
                return;

            SerializeFeatures(
                InstantiatedFeatures.Concat(new[] 
                { 
                    new SolutionFeatureState 
                    {
                        FeatureId = featureId, 
                        InstanceName = instanceName,
                         Version = version
                    }
                })
            );
        }

        public void RemoveFeature(string featureId, string instanceName)
        {
            var features = InstantiatedFeatures.ToList();
            features.RemoveAll(state => state.FeatureId == featureId && state.InstanceName == instanceName);

            SerializeFeatures(features);
        }

        public void Update(string featureId, string instanceName, Version newVersion)
        {
            var features = InstantiatedFeatures.ToList();
            var feature = features.FirstOrDefault(f => f.FeatureId == featureId && f.InstanceName == instanceName);

            if (feature != null)
            {
                feature.Version = newVersion;
                SerializeFeatures(features);
            }
        }

        private void SerializeFeatures(IEnumerable<ISolutionFeatureState> features)
        {
            Solution.Data.Features = String.Join("|", features
                .Select(state => state.FeatureId + ":" + state.InstanceName + ":" + state.Version.ToString())
                .ToArray());
        }
    }
}
