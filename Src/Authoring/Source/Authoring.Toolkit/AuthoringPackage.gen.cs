using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;

namespace Microsoft.VisualStudio.Patterning.Authoring.Authoring
{
    /// <summary>
    /// The package class for this extension.
    /// </summary>
    public partial class AuthoringPackage : Package
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<AuthoringPackage>();
        internal static string CurrentToolkitVersion = "1.3.20.0";
        private static string MsBuildPath = @"%localappdata%\Microsoft\MSBuild\Outercurve\Pattern Toolkit Builder 4VS2012";
		private static string TargetsFilename = "Microsoft.VisualStudio.Patterning.Authoring.PatternToolkitVersion.targets";
        internal static readonly string TargetsPath = Environment.ExpandEnvironmentVariables(Path.Combine(MsBuildPath, TargetsFilename));
		internal static readonly Dictionary<string,string> InstalledExtensionProperties = new Dictionary<string, string>
        { 
            {"PatternToolkitRuntime", "c869918e-f94e-4e7a-ab25-b076ff4e751b" },
            {"PatternToolkitBuilder", "84031a32-b20f-479c-a620-beacd982ea13" },
        };
    }
}