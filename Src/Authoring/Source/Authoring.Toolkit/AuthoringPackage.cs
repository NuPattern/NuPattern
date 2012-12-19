using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.ExtensionManager;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using NuPattern.Runtime;

namespace NuPattern.Authoring.Authoring
{
    [ProvideAutoLoad(UIContextGuids.EmptySolution), ProvideAutoLoad(UIContextGuids.SolutionExists)]
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [CLSCompliant(false)]
    [Guid("4AABC2F4-D907-4685-A5E7-9FB851DAC3E9")]
    [ComVisible(true)]
    [Microsoft.VisualStudio.Modeling.Shell.ProvideBindingPath]
    partial class AuthoringPackage
    {
        /// <summary>
        /// Initializes the package.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            var extensionManager = this.GetService<SVsExtensionManager, IVsExtensionManager>();

            // Update MSBUILD properties in version independeny targets
            VersionHelper.SyncTargets(tracer,
                new TargetsInfo
                {
                    TargetsPath = TargetsPath,
                    ToolkitVersion = CurrentToolkitVersion,
                    InstalledExtensionProperties = VersionHelper.GetInstalledExtensionPaths(extensionManager, InstalledExtensionProperties),
                });
        }

        [Conditional("DEBUG")]
        private void DumpMefLog(IComponentModel componentModel)
        {
        }
    }
}
