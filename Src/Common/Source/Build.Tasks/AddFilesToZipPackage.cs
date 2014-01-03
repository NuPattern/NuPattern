using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using NuPattern.Build.Tasks.Properties;

namespace NuPattern.Build.Tasks
{
    /// <summary>
    /// Adds files to an existing archive.
    /// </summary>
    public class AddFilesToZipPackage : Microsoft.Build.Utilities.Task
    {
        private CompressionOption packageCompressionLevel = CompressionOption.NotCompressed;

        /// <summary>
        /// Gets or sets the path to the source archive.
        /// </summary>
        [Required]
        public string SourceArchive { get; set; }

        /// <summary>
        /// Gets or sets the full path to the source files to add to the archive.
        /// </summary>
        [Required]
        public string SourcePath { get; set; }

        /// <summary>
        /// Gets or sets the regular expression of files to add to the archive.
        /// </summary>
        public string MatchExpression { get; set; }

        /// <summary>
        /// Gets or sets the level of compression for the archive.
        /// </summary>
        public string CompressionLevel { get; set; }

        /// <summary>
        /// Executes the task.
        /// </summary>
        /// <returns></returns>
        public override bool Execute()
        {
            base.Log.LogMessage(Resources.AddFilesToZipPackage_AddFilesToArchive, this.SourceArchive, this.SourcePath, this.MatchExpression);

            // Ensure source path exists
            if (!Directory.Exists(this.SourcePath))
            {
                base.Log.LogError(Resources.AddFileToZipPackage_SourcePathNotExist, this.SourcePath);
                return false;
            }

            // Filter files
            var matchExpression = !string.IsNullOrEmpty(this.MatchExpression) ? this.MatchExpression : "*";

            // Ensure we have a valid CompressionLevel
            if (!string.IsNullOrWhiteSpace(this.CompressionLevel))
            {
                if (!Enum.TryParse<CompressionOption>(this.CompressionLevel, true, out this.packageCompressionLevel)
                    || (this.packageCompressionLevel != CompressionOption.Normal && 
                    this.packageCompressionLevel != CompressionOption.Maximum && 
                    this.packageCompressionLevel != CompressionOption.NotCompressed))
                {
                    base.Log.LogWarning(Resources.AddFileToZipPackage_CompressionLevelNotSupported, new object[]
                        {
                            this.CompressionLevel,
                            string.Join(",", new[]{CompressionOption.Normal.ToString(), CompressionOption.Maximum.ToString(), CompressionOption.NotCompressed.ToString()})
                        });
                    this.packageCompressionLevel = CompressionOption.NotCompressed;
                }
            }

            AddFilesToArchive(this.SourceArchive, matchExpression, this.packageCompressionLevel);

            return !base.Log.HasLoggedErrors;
        }

        private void AddFilesToArchive(string archivePath, string matchExpression, CompressionOption compressionLevel)
        {
            // Locate source files matching expression
            var sourceFilePaths = Directory.EnumerateFiles(this.SourcePath, matchExpression, SearchOption.AllDirectories);

            using (var package = Package.Open(archivePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                sourceFilePaths.ToList().ForEach(sourceFilePath =>
                {
                    try
                    {
                        // Add file to archive
                        using (Stream streamForFile = new FileStream(sourceFilePath, FileMode.Open, FileAccess.Read))
                        {
                            PackagePart packagePart;
                            var relSourceFilePath = sourceFilePath.Substring(this.SourcePath.Length);
                            var partUri = PackUriHelper.CreatePartUri(new Uri(relSourceFilePath, UriKind.Relative));
                            if (!package.PartExists(partUri))
                            {
                                base.Log.LogMessage(Resources.AddFilesToZipPackage_AddNewFileToArchive, relSourceFilePath, sourceFilePath);

                                packagePart = package.CreatePart(partUri,
                                    GetMimeTypeFromExtension(Path.GetExtension(sourceFilePath)), compressionLevel);
                            }
                            else
                            {
                                base.Log.LogMessage(Resources.AddFilesToZipPackage_AddExistingFileToArchive, relSourceFilePath, sourceFilePath);

                                packagePart = package.GetPart(partUri);
                            }

                            streamForFile.CopyStream(packagePart.GetStream());
                        }
                    }
                    catch (Exception ex)
                    {
                        base.Log.LogError(Resources.AddFileToZipPackage_AddFileFailed, sourceFilePath, ex.Message);
                        throw;
                    }
                });
            }
        }

        private static string GetMimeTypeFromExtension(string extension)
        {
            var fileType = extension.ToLower(CultureInfo.InvariantCulture);
            switch (fileType)
            {
                case ".txt":
                case ".pkgdef":
                    return "text/plain";
                case ".vsixmanifest":
                case ".xml":
                    return "text/xml";
                case ".htm":
                case ".html":
                    return "text/html";
                case ".pdf":
                    return "application/pdf";
                case ".rtf":
                    return "text/richtext";
                case ".gif":
                    return "image/gif";
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                case ".tiff":
                    return "image/tiff";
                case ".vsix":
                case ".zip":
                    return "application/zip";
            }

            return "application/octet-stream";
        }
    }
}
