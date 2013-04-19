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

        public IEnumerable<IGuidanceExtensionPersistState> InstantiatedGuidanceExtensions
        {
            get
            {
                var extensions = (string)Solution.Data.Features;
                if (String.IsNullOrEmpty(extensions))
                    return Enumerable.Empty<GuidanceExtensionPersistState>();

                Version version;
                return from extension in extensions.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries)
                       let state = extension.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries)
                       // We ignore whatever is mangled and invalid there
                       where state.Length == 3 &&
                            !String.IsNullOrEmpty(state[0]) &&
                            !String.IsNullOrEmpty(state[1]) &&
                            Version.TryParse(state[2], out version)
                       select new GuidanceExtensionPersistState
                       {
                           ExtensionId = state[0],
                           InstanceName = state[1],
                           Version = new Version(state[2])
                       };
            }
        }

        public void AddExtension(string extensionId, string instanceName, Version version)
        {
            var existingExtensions = InstantiatedGuidanceExtensions.ToList();
            var thisExtension = existingExtensions.FirstOrDefault(f => f.ExtensionId == extensionId && f.InstanceName == instanceName && f.Version == version);
            if (thisExtension != null)
                return;

            SerializeExtensions(
                InstantiatedGuidanceExtensions.Concat(new[] 
                { 
                    new GuidanceExtensionPersistState 
                    {
                        ExtensionId = extensionId, 
                        InstanceName = instanceName,
                        Version = version,
                    }
                })
            );
        }

        public void RemoveExtension(string extensionId, string instanceName)
        {
            var extensions = InstantiatedGuidanceExtensions.ToList();
            extensions.RemoveAll(state => state.ExtensionId == extensionId && state.InstanceName == instanceName);

            SerializeExtensions(extensions);
        }

        public void Update(string extensionId, string instanceName, Version newVersion)
        {
            var extensions = InstantiatedGuidanceExtensions.ToList();
            var extension = extensions.FirstOrDefault(f => f.ExtensionId == extensionId && f.InstanceName == instanceName);

            if (extension != null)
            {
                extension.Version = newVersion;
                SerializeExtensions(extensions);
            }
        }

        private void SerializeExtensions(IEnumerable<IGuidanceExtensionPersistState> extensions)
        {
            Solution.Data.Features = String.Join("|", extensions
                .Select(state => state.ExtensionId + ":" + state.InstanceName + ":" + state.Version.ToString())
                .ToArray());
        }
    }
}
