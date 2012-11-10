using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.ExtensionManager;
using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.VisualStudio.Patterning.IntegrationTests
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
            protected bool FolderContainsExclusive(string relFolderPath, IEnumerable<string> filenames)
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

                    var uniqueFolderFiles = folderFiles.Select(f => f.Name).Except(filenames);
                    var uniqueFilenames = filenames.Except(folderFiles.Select(f => f.Name));

                    return !(uniqueFolderFiles.Any()) 
                        && !(uniqueFilenames.Any());
                }
            }

            /// <summary>
            /// Determines if the given file exists in the VSIX package
            /// </summary>
            /// <returns></returns>
            protected bool FolderNotEmpty(string relFolderPath, string fileTypes="*.*")
            {
                var dirPath = Path.Combine(this.TargetDir, relFolderPath);
                var exists = Directory.Exists(dirPath);
                if (!exists)
                {
                    return false;
                }
                else
                {
                    return new DirectoryInfo(dirPath).GetFiles(fileTypes).Any();
                }
            }
        }
    }
}
