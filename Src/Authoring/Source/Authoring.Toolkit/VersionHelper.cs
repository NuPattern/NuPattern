using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Microsoft.VisualStudio.Patterning.Authoring.Authoring.Properties;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;

namespace Microsoft.VisualStudio.Patterning.Authoring.Authoring
{
    /// <summary>
    /// Helper for toolkit versioning.
    /// </summary>
    internal static class VersionHelper
    {
        private static string TargetsFilename = "Microsoft.VisualStudio.Patterning.Authoring.PatternToolkitVersion.targets";
        private static string MSBuildNamespace = "http://schemas.microsoft.com/developer/msbuild/2003";
        private static string ToolkitVersionPath = "{{{0}}}PatternToolkitVersion";
        private static string HostVersionPath = "{{{0}}}PatternToolkitHostVersion";

        /// <summary>
        /// Synchronizes the targets file on disk with targets file in this version of the toolkit.
        /// </summary>
        internal static void SyncTargets(ITraceSource tracer, string MsBuildPath, string currentVersion)
        {
            try
            {
                tracer.TraceInformation(Resources.AuthoringPackage_TraceSyncTargetsInitial);

                // Verify targets file exists
                var targetsPath = CalculateTargetsPath(MsBuildPath);
                if (File.Exists(targetsPath))
                {
                    tracer.TraceVerbose(Resources.AuthoringPackage_TraceSyncTargetsRetrievingVersion, targetsPath);

                    var version = GetTargetsInfo(targetsPath);
                    if (!string.IsNullOrEmpty(version.ToolkitVersion))
                    {
                        // Compare version info
                        if (!version.ToolkitVersion.Equals(currentVersion, StringComparison.OrdinalIgnoreCase))
                        {
                            tracer.TraceInformation(Resources.AuthoringPackage_TraceSyncTargetsUpdateVersion, targetsPath, version, currentVersion);

                            // Write updated targets
                            WriteFile(tracer, targetsPath, currentVersion);
                        }
                        else
                        {
                            tracer.TraceInformation(Resources.AuthoringPackage_TraceSyncTargetsNoVersionUpdate, targetsPath, currentVersion);
                        }
                    }
                    else
                    {
                        tracer.TraceError(Resources.AuthoringPackage_TraceSyncTargetsNoVersion, targetsPath);
                    }
                }
                else
                {
                    // Write current targets
                    WriteFile(tracer, targetsPath, currentVersion);
                }
            }
            catch (Exception ex)
            {
                tracer.TraceError(ex, Resources.AuthoringPackage_FailedSyncTargets);
            }
        }

        /// <summary>
        /// Returns the full path to the targets file, given a root directory.
        /// </summary>
        internal static string CalculateTargetsPath(string directory)
        {
            return Environment.ExpandEnvironmentVariables(Path.Combine(directory, TargetsFilename));
        }

        /// <summary>
        /// Returns the info from targets file
        /// </summary>
        internal static TargetsInfo GetTargetsInfo(string targetsPath)
        {
            var targetsInfo = new TargetsInfo();

            XDocument document = XDocument.Load(targetsPath);
            var nsManager = new XmlNamespaceManager(new NameTable());
            nsManager.AddNamespace("", MSBuildNamespace);

            var toolkitVersionElement = document.Descendants(XName.Get(string.Format(CultureInfo.InvariantCulture, ToolkitVersionPath, MSBuildNamespace))).FirstOrDefault();
            if (toolkitVersionElement != null)
            {
                targetsInfo.ToolkitVersion = (string)toolkitVersionElement.Value;
            }

            var hostVersionElement = document.Descendants(XName.Get(string.Format(CultureInfo.InvariantCulture, HostVersionPath, MSBuildNamespace))).FirstOrDefault();
            if (hostVersionElement != null)
            {
                targetsInfo.HostVersion = (string)hostVersionElement.Value;
            }

            return targetsInfo;
        }

        private static void WriteFile(ITraceSource tracer, string targetsPath, string currentVersion)
        {
            tracer.TraceInformation(Resources.AuthoringPackage_TraceSyncTargetsWritingNewTargets, targetsPath, currentVersion);

            //Delete file if exists
            if (File.Exists(targetsPath))
            {
                File.Delete(targetsPath);
            }

            // Ensure directory exists
            var targetsFolder = Path.GetDirectoryName(targetsPath);
            if (!Directory.Exists(targetsFolder))
            {
                Directory.CreateDirectory(targetsFolder);
            }

            // Check file can be written
            using (var file = new FileStream(targetsPath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                file.Write(Resources.Microsoft_VisualStudio_Patterning_Authoring_PatternToolkitVersion, 0, Resources.Microsoft_VisualStudio_Patterning_Authoring_PatternToolkitVersion.Length);
                file.Flush();
            }
        }
    
        internal class TargetsInfo
        {
            /// <summary>
            /// Gets or sets the version of the toolkit
            /// </summary>
            public string ToolkitVersion { get; set; }

            /// <summary>
            /// Gets or sets the version of the host
            /// </summary>
            public string HostVersion { get; set; }
        }
    }
}
