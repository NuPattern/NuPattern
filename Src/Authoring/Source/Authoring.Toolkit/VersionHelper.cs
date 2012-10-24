using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Microsoft.VisualStudio.Patterning.Authoring.Authoring.Properties;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using System.Collections.Generic;
using Microsoft.VisualStudio.ExtensionManager;

namespace Microsoft.VisualStudio.Patterning.Authoring.Authoring
{
    /// <summary>
    /// Helper for toolkit versioning.
    /// </summary>
    internal static class VersionHelper
    {
        private static string MSBuildNamespace = "http://schemas.microsoft.com/developer/msbuild/2003";
        private static string PatternToolkitVersionElementName = "PatternToolkitVersion";

        /// <summary>
        /// Converts the given collection of extension properties and their identifiers to full paths.
        /// </summary>
        internal static Dictionary<string, string> GetInstalledExtensionPaths(IVsExtensionManager extensionManager, IDictionary<string, string> extensionIds)
        {
            // Update installed Extensions
            var installedExtensionPaths = new Dictionary<string, string>();
            extensionIds.ToList().ForEach(ie =>
            {
                installedExtensionPaths.Add(ie.Key, GetInstalledExtensionPath(extensionManager, ie.Value));
            });

            return installedExtensionPaths;
        }

        /// <summary>
        /// Synchronizes the targets file on disk with targets file in this version of the toolkit.
        /// </summary>
        internal static void SyncTargets(ITraceSource tracer, TargetsInfo targetsInfo)
        {
            try
            {
                tracer.TraceInformation(Resources.AuthoringPackage_TraceSyncTargetsInitial);

                //Write updated targets file
                WriteUpdatedTargets(tracer, targetsInfo);
            }
            catch (Exception ex)
            {
                tracer.TraceError(ex, Resources.AuthoringPackage_FailedSyncTargets);
            }
        }

        private static void WriteUpdatedTargets(ITraceSource tracer, TargetsInfo targetsInfo)
        {
            tracer.TraceInformation(Resources.AuthoringPackage_TraceSyncTargetsWritingNewTargets, targetsInfo.TargetsPath, targetsInfo.ToolkitVersion);

            //Delete file if exists
            if (File.Exists(targetsInfo.TargetsPath))
            {
                File.Delete(targetsInfo.TargetsPath);
            }

            // Ensure directory exists
            var targetsFolder = Path.GetDirectoryName(targetsInfo.TargetsPath);
            if (!Directory.Exists(targetsFolder))
            {
                Directory.CreateDirectory(targetsFolder);
            }

            // Write new targets
            PopulateTargets(targetsInfo);
        }

        private static string GetInstalledExtensionPath(IVsExtensionManager extensionManager, string extensionId)
        {
            if (extensionManager != null)
            {
                IInstalledExtension extension;
                if (extensionManager.TryGetInstalledExtension(extensionId, out extension))
                {
                    return NormalizePathForMsBuild(extension.InstallPath);
                }
            }

            return null;
        }

        internal static void ReadTargetsValues(TargetsInfo targetsInfo)
        {
            // Read toolkit version number
            targetsInfo.ToolkitVersion = ReadPropertyValue(targetsInfo, PatternToolkitVersionElementName);

            // Read extension paths
            if (targetsInfo.InstalledExtensionProperties != null)
            {
                var readInstalledExtensions = new Dictionary<string, string>();
                targetsInfo.InstalledExtensionProperties.ToList().ForEach(ie =>
                    {
                        readInstalledExtensions.Add(ie.Key, ReadPropertyValue(targetsInfo, ie.Key));
                    });
                targetsInfo.InstalledExtensionProperties = readInstalledExtensions;
            }
        }

        private static string ReadPropertyValue(TargetsInfo targetsInfo, string propertyName)
        {
            XDocument document = XDocument.Load(targetsInfo.TargetsPath);
            var nsManager = new XmlNamespaceManager(new NameTable());
            nsManager.AddNamespace(string.Empty, MSBuildNamespace);

            var propertyElement = document.Descendants(XName.Get(
                            string.Format(CultureInfo.InvariantCulture, "{{{1}}}{0}", propertyName, MSBuildNamespace))).FirstOrDefault();
            if (propertyElement != null)
            {
                return propertyElement.Value;
            }
            else
            {
                return null;
            }
        }

        private static void PopulateTargets(TargetsInfo targetsInfo)
        {
            using (var stream = new MemoryStream(Resources.Microsoft_VisualStudio_Patterning_Authoring_PatternToolkitVersion))
            {
                XDocument document = XDocument.Load(stream);
                var nsManager = new XmlNamespaceManager(new NameTable());
                nsManager.AddNamespace(string.Empty, MSBuildNamespace);

                // Update extension paths
                targetsInfo.InstalledExtensionProperties.ToList().ForEach(ie =>
                    {
                        var extensionProp = document.Descendants(XName.Get(
                            string.Format(CultureInfo.InvariantCulture, "{{{1}}}{0}", ie.Key, MSBuildNamespace))).FirstOrDefault();
                        if (extensionProp != null)
                        {
                            extensionProp.Value = ie.Value ?? string.Empty;
                        }
                    });

                // Save targets
                document.Save(targetsInfo.TargetsPath, SaveOptions.None);
            }
        }

        private static string NormalizePathForMsBuild(string path)
        {
            var normalized = path;

            //Trim trailing slashes
            normalized = normalized.TrimEnd(new[] { Path.DirectorySeparatorChar});
            
            // Substitute environment variables
            var localAppDataPath = Environment.ExpandEnvironmentVariables("%localappdata%");
            normalized = normalized.Replace(localAppDataPath, "$(LocalAppData)");
            
            return normalized;
        }
    }
}
