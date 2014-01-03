using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using NuPattern.Build.Tasks.Properties;

namespace NuPattern.Build.Tasks
{
    /// <summary>
    /// Extracts files from an existing archive.
    /// </summary>
    public class ExtractFilesFromZipPackage : Microsoft.Build.Utilities.Task
    {
        private const string MatchExpressionWildcard = @"*";
        private const string MatchExpressionWildcardEscaped = @"\*";
        private const string MatchExpressionWildcardRegEx = @"[\w\s\d\.]*";

        /// <summary>
        /// Gets or sets the path to the source archive.
        /// </summary>
        [Required]
        public string SourceArchive { get; set; }

        /// <summary>
        /// Gets or sets the regular expression of files to extract from the archive.
        /// </summary>
        public string MatchExpression { get; set; }

        /// <summary>
        /// Get or sets the full path to where extracted files are to be written.
        /// </summary>
        [Required]
        public string TargetPath { get; set; }

        /// <summary>
        /// Executes the task.
        /// </summary>
        /// <returns></returns>
        public override bool Execute()
        {
            base.Log.LogMessage(Resources.ExtractFilesFromZipPackage_ExtractArchive, this.SourceArchive, this.MatchExpression);

            // Ensure we have an existing archive
            if (!File.Exists(this.SourceArchive))
            {
                base.Log.LogError(Resources.ExtractFilesFromZipPackage_SourceArchiveNotExist, this.SourceArchive);
                return false;
            }

            // Ensure target path exists
            if (!Directory.Exists(this.TargetPath))
            {
                Directory.CreateDirectory(this.TargetPath);
            }

            // Filter files
            var matchExpression = !string.IsNullOrEmpty(this.MatchExpression) ? this.MatchExpression : MatchExpressionWildcard;
            matchExpression = Regex.Escape(matchExpression).Replace(MatchExpressionWildcardEscaped, MatchExpressionWildcardRegEx);

            ExtractFiles(this.SourceArchive, matchExpression, this.TargetPath);

            return !base.Log.HasLoggedErrors;
        }

        private void ExtractFiles(string archivePath, string matchExpression, string targetPath)
        {
            using (var package = Package.Open(archivePath, FileMode.Open, FileAccess.Read))
            {
                    package.GetParts().ToList().ForEach(part =>
                    {
                        var partPath = part.Uri;

                        try
                        {
                            var partFilePath = GetFileNameFromPart(partPath);

                            // Copy matched files to target
                            if (Regex.IsMatch(partFilePath, matchExpression))
                            {
                                var targetFilePath = Path.Combine(targetPath, partFilePath);
                                var targetDirPath = Path.GetDirectoryName(targetFilePath);
                                if (!Directory.Exists(targetDirPath))
                                {
                                    Directory.CreateDirectory(targetDirPath);
                                }

                                base.Log.LogMessage(Resources.ExtractFilesFromZipPackage_ExtractPart, partPath, targetDirPath);

                                using (Stream streamForTargetFile = File.OpenWrite(targetFilePath))
                                {
                                    part.GetStream().CopyStream(streamForTargetFile);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            base.Log.LogError(Resources.ExtractFilesFromZipPackage_ExtractPartFailed, partPath, ex.Message);
                            throw;
                        }
                    });
            }
        }

        private static string GetFileNameFromPart(Uri uri)
        {
            var invalidChars = Path.GetInvalidFileNameChars();

            var sb = new StringBuilder(uri.OriginalString.Length);

            foreach (var c in uri.OriginalString)
            {
                if (c.Equals(Path.DirectorySeparatorChar))
                {
                    // Leave it
                }
                else if (c.Equals(Path.AltDirectorySeparatorChar))
                {
                    sb.Append(Path.DirectorySeparatorChar);
                }
                else
                {
                    sb.Append(Array.IndexOf(invalidChars, c) < 0 ? c : '_');
                }
            }

            return sb.ToString().TrimStart(Path.DirectorySeparatorChar);
        }
    }
}
