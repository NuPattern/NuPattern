using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Modeling.Shell;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;

namespace Microsoft.VisualStudio.Patterning.Authoring.Authoring
{
    /// <summary>
    /// The package class for this extension.
    /// </summary>
    [ProvideAutoLoad(UIContextGuids.SolutionExists), ProvideAutoLoad(UIContextGuids.EmptySolution)]
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [CLSCompliant(false)]
    [Guid("4AABC2F4-D907-4685-A5E7-9FB851DAC3E9")]
    [ComVisible(true)]
    [ProvideBindingPath]
    public class AuthoringPackage : Package
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<AuthoringPackage>();

		internal static string CurrentHostVersion = "10.0";
        internal static string CurrentToolkitVersion = "1.3.20.0";
        internal static string MsBuildPath = @"%localappdata%\Microsoft\MSBuild\Outercurve\Pattern Toolkit Builder";

        /// <summary>
        /// Initializes the package.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            VersionHelper.SyncTargets(tracer, MsBuildPath, CurrentToolkitVersion);
        }
    }
}