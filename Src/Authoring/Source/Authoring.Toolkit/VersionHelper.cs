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

        /// <summary>
        /// Synchronizes the targets file on disk with targets file in this version of the toolkit.
        /// </summary>
        internal static void SyncTargets(ITraceSource tracer, string MsBuildPath, string currentVersion)
        {
            try
            {
                tracer.TraceInformation(Resources.AuthoringPackage_TraceSyncTargetsInitial);

                // Does the file exist where it needs to be ?
                var targetsPath = CalculateTargetsPath(MsBuildPath);
                if (File.Exists(targetsPath))
                {
                    // Load file as XML
                    tracer.TraceVerbose(Resources.AuthoringPackage_TraceSyncTargetsRetrievingVersion, targetsPath);

                    var version = GetTargetsVersion(targetsPath);
                    if (!string.IsNullOrEmpty(version))
                    {
                        // Compare versions
                        if (!version.Equals(currentVersion, StringComparison.OrdinalIgnoreCase))
                        {
                            tracer.TraceInformation(Resources.AuthoringPackage_TraceSyncTargetsUpdateVersion, targetsPath, version, currentVersion);

                            // Write new targets version
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
                    // Write new targets version
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
        /// Returns the version number within the targets file
        /// </summary>
        internal static string GetTargetsVersion(string targetsPath)
        {
            XDocument document = XDocument.Load(targetsPath);
            var nsManager = new XmlNamespaceManager(new NameTable());
            nsManager.AddNamespace("", MSBuildNamespace);
            var versionElement = document.Descendants(XName.Get(string.Format(CultureInfo.InvariantCulture, "{{{0}}}PatternToolkitVersion", MSBuildNamespace))).FirstOrDefault();
            if (versionElement != null)
            {
                return (string)versionElement.Value;
            }

            return null;
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
    }
}
