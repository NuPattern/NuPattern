using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuPattern.VisualStudio.Extensions;

namespace NuPattern.IntegrationTests
{
    [TestClass]
    public class VsixPackagingSpec
    {
        [TestClass]
        public abstract class GivenAVsix : IntegrationTest
        {
            /// <summary>
            /// The information for the VSIX
            /// </summary>
            protected IExtension VsixInfo { get; private set; }

            /// <summary>
            /// The identifier of the VSIX
            /// </summary>
            protected string VsixIdentifier { get; private set; }

            /// <summary>
            /// The target directory where the VSIX is upacked.
            /// </summary>
            protected string TargetDir { get; private set; }

            [TestInitialize]
            public void Initialize()
            {
                var deployedVsixItemPath = Path.Combine(this.TestContext.DeploymentDirectory, this.DeployedVsixItemPath);
                this.VsixInfo = Vsix.ReadManifest(deployedVsixItemPath);

                // Unzip VSIX content to target dir
                this.TargetDir = new DirectoryInfo("Target").FullName;
                Vsix.Unzip(deployedVsixItemPath, this.TargetDir);

                this.VsixIdentifier = Vsix.ReadManifestIdentifier(Path.Combine(this.TargetDir, "extension.vsixmanifest"));
            }

            /// <summary>
            /// Returns the relative path to the deployed Vsix file in the project
            /// </summary>
            protected abstract string DeployedVsixItemPath { get; }

            /// <summary>
            /// Determines if the given file exists in the VSIX package
            /// </summary>
            /// <returns></returns>
            protected bool FileExists(string relFilePath)
            {
                return File.Exists(Path.Combine(this.TargetDir, relFilePath));
            }

            /// <summary>
            /// Determines whether the given folder contains only the given files, and no others
            /// </summary>
            /// <returns></returns>
            protected bool FolderContainsExclusive(string relFolderPath, IEnumerable<string> filenames, Anomalies anomalies = null)
            {
                var dirPath = Path.Combine(this.TargetDir, relFolderPath);
                var exists = Directory.Exists(dirPath);
                if (!exists)
                {
                    return false;
                }
                else
                {
                    var folderFiles = new DirectoryInfo(dirPath).GetFiles();

                    var filesInFolder = folderFiles.Select(f => f.Name).Except(filenames).ToList();
                    var expectedFiles = filenames.Except(folderFiles.Select(f => f.Name)).ToList();

                    if (anomalies != null)
                    {
                        anomalies.ExpectedFiles.AddRange(expectedFiles);
                        anomalies.UnexpectedFiles.AddRange(filesInFolder);
                    }

                    return !(filesInFolder.Any())
                        && !(expectedFiles.Any());
                }
            }

            /// <summary>
            /// Determines if the given file exists in the given folder
            /// </summary>
            /// <returns></returns>
            protected bool FolderNotEmpty(string relFolderPath, string fileTypes = "*.*", Anomalies anomalies = null)
            {
                var dirPath = Path.Combine(this.TargetDir, relFolderPath);
                var exists = Directory.Exists(dirPath);
                if (!exists)
                {
                    if (anomalies != null)
                    {
                        anomalies.ExpectedFiles.Add(relFolderPath);
                    }

                    return false;
                }
                else
                {
                    var filesInFolder = new DirectoryInfo(dirPath).GetFiles(fileTypes);
                    if (!filesInFolder.Any())
                    {
                        if (anomalies != null)
                        {
                            anomalies.ExpectedFiles.Add(fileTypes);
                        }
                    }

                    return filesInFolder.Any();
                }
            }

            /// <summary>
            /// Asserts whether the given folder contains only the given files, and no others, and reports the difference
            /// </summary>
            protected void AssertFolderContainsExclusive(string relFolderPath, IEnumerable<string> filenames)
            {
                var anomalies = new Anomalies();
                Assert.IsTrue(FolderContainsExclusive(relFolderPath, filenames, anomalies), anomalies.Format());
            }

            /// <summary>
            /// Asserts whether if the given file exists in the given folder, and reports the difference
            /// </summary>
            protected void AssertFolderNotEmpty(string relFolderPath, string fileTypes = "*.*")
            {
                var anomalies = new Anomalies();
                Assert.IsTrue(FolderNotEmpty(relFolderPath, fileTypes, anomalies), anomalies.Format());
            }

            public class Anomalies
            {
                public Anomalies()
                {
                    this.ExpectedFiles = new List<string>();
                    this.UnexpectedFiles = new List<string>();
                }

                public List<string> UnexpectedFiles { get; private set; }
                public List<string> ExpectedFiles { get; private set; }

                public string Format()
                {
                    var expectedString = string.Empty;
                    if (this.ExpectedFiles.Any())
                    {
                        expectedString = string.Format(CultureInfo.CurrentCulture, "Missing files: {0}", string.Join(";", this.ExpectedFiles));
                    }

                    var unexpectedString = string.Empty;
                    if (this.UnexpectedFiles.Any())
                    {
                        unexpectedString = string.Format(CultureInfo.CurrentCulture, "Additional files: {0}", string.Join(";", this.UnexpectedFiles));
                    }

                    return string.Join(", ", new[] { expectedString, unexpectedString });
                }
            }
        }
    }
}
