using System;
using EnvDTE;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;

namespace NuPattern.Runtime.Shell
{
    internal class ProductStateValidator : IDisposable
    {
        private bool disposed = false;
        private IPatternManager patternManager;
        private BuildEvents buildEvents;

        public ProductStateValidator(IServiceProvider serviceProvider)
        {
            Guard.NotNull(() => serviceProvider, serviceProvider);

            this.patternManager = serviceProvider.GetService<IPatternManager>();
            this.buildEvents = serviceProvider.GetService<SDTE, DTE>().Events.BuildEvents;
            this.buildEvents.OnBuildDone += OnBuildDone;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (this.buildEvents != null)
                    {
                        this.buildEvents.OnBuildDone -= OnBuildDone;
                        this.buildEvents = null;
                    }
                }

                this.disposed = true;
            }
        }

        private void OnBuildDone(vsBuildScope scope, vsBuildAction action)
        {
            if (this.patternManager.IsOpen)
            {
                this.patternManager.ValidateProductState();
            }
        }
    }
}