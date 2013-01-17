using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;

namespace NuPattern.Authoring.PatternToolkit
{
    /// <summary>
    /// The package class for this extension.
    /// </summary>
    public partial class AuthoringPackage : Package
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<AuthoringPackage>();
        internal static string CurrentToolkitVersion = "1.3.20.0";
        private static string MsBuildPath = @"%localappdata%\Microsoft\MSBuild\NuPattern\NuPattern Toolkit Builder VS2012";
		private static string TargetsFilename = "NuPattern.Authoring.PatternToolkitVersion.targets";
        internal static readonly string TargetsPath = Environment.ExpandEnvironmentVariables(Path.Combine(MsBuildPath, TargetsFilename));
		internal static readonly Dictionary<string,string> InstalledExtensionProperties = new Dictionary<string, string>
        { 
            {"PatternToolkitRuntime", "93373818-600f-414b-8181-3a0cb79fa785" },
            {"PatternToolkitBuilder", "9f6dc301-6f66-4d21-9f9c-b37412b162f6" },
        };
    }
}