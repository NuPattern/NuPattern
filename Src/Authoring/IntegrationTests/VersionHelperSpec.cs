using System.IO;
using Microsoft.VisualStudio.Patterning.Authoring.Authoring;
using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VSSDK.Tools.VsIdeTesting;
using System.Runtime.InteropServices;
using System;

namespace Microsoft.VisualStudio.Patterning.Authoring.IntegrationTests
{
    [TestClass]
    public class VersionHelperSpec
    {
        internal static readonly IAssertion Assert = new Assertion();
        private static readonly string TargetsFilePath = VersionHelper.CalculateTargetsPath(AuthoringPackage.MsBuildPath);

        private static bool IsTargetsFileExists()
        {
            return File.Exists(TargetsFilePath);
        }

        private static bool IsTargetsFileCurrent()
        {
            return IsTargetsFileExists() 
                && IsTargetsFileCurrentVersionOfToolkit()
                && IsTargetsFileCurrentVersionOfHost();
        }

        private static bool IsTargetsFileRewritten()
        {
            var currentFileInfo = new FileInfo(TargetsFilePath);

            if (currentFileInfo.CreationTime > currentFileInfo.LastWriteTime)
                return false;
            else
                return !currentFileInfo.LastWriteTime.Equals(currentFileInfo.CreationTime);
        }

        private static bool IsTargetsFileCurrentVersionOfToolkit()
        {
            var currentVersion = VersionHelper.GetTargetsInfo(TargetsFilePath);
            if (!string.IsNullOrEmpty(currentVersion.ToolkitVersion))
            {
                return currentVersion.ToolkitVersion.Equals(AuthoringPackage.CurrentToolkitVersion);
            }
            else
            {
                return false;
            }
        }

        private static bool IsTargetsFileCurrentVersionOfHost()
        {
            var currentVersion = VersionHelper.GetTargetsInfo(TargetsFilePath);
            if (!string.IsNullOrEmpty(currentVersion.HostVersion))
            {
                return currentVersion.HostVersion.Equals(AuthoringPackage.CurrentHostVersion);
            }
            else
            {
                return false;
            }
        }

        [TestClass]
        [DeploymentItem("VersionTargetsSpec", "VersionTargetsSpec")]
        public class GivenNoTargetsFile : IntegrationTest
        {
            private ISolution solution;

            [TestInitialize]
            public void Initialize()
            {
                //Ensure targets file does not exist
                File.Delete(TargetsFilePath);
            }

            [TestCleanup]
            public void Cleanup()
            {
                VsIdeTestHostContext.Dte.Solution.Close();

                // Remove file if written
                File.Delete(TargetsFilePath);
            }

            [TestMethod]
            [HostType("VS IDE")]
            [TestProperty("VSHostRestartOptions", "Before")]
            public void WhenNoSolution_ThenTargetsNotWritten()
            {
                Assert.False(IsTargetsFileExists());
            }

            [TestMethod]
            [HostType("VS IDE")]
            [TestProperty("VSHostRestartOptions", "Before")]
            public void WhenCreateEmptySolution_ThenTargetsWritten()
            {
                Assert.False(IsTargetsFileExists());

                this.solution = (ISolution)VsIdeTestHostContext.ServiceProvider.GetService(typeof(ISolution));
                this.solution.CreateInstance(this.DeploymentDirectory, "EmptySolution");

                Assert.True(IsTargetsFileCurrent());
            }

            [TestMethod]
            [HostType("VS IDE")]
            [TestProperty("VSHostRestartOptions", "Before")]
            public void WhenLoadingExistingSolution_ThenTargetsWritten()
            {
                Assert.False(IsTargetsFileExists());

                this.solution = (ISolution)VsIdeTestHostContext.ServiceProvider.GetService(typeof(ISolution));
                this.solution.Open(PathTo("VersionTargetsSpec\\SimpleLibrarySolution.sln"));

                Assert.True(IsTargetsFileCurrent());
            }
        }

        [TestClass]
        [DeploymentItem("VersionTargetsSpec", "VersionTargetsSpec")]
        public class GivenWrongToolkitVersionInTargets : IntegrationTest
        {
            private ISolution solution;
            private FileInfo targetsFileInfo;

            [TestInitialize]
            public void Initialize()
            {
                //Ensure targets file does exist
                File.Copy(PathTo("VersionTargetsSpec\\VersionTargets_Old.targets"), TargetsFilePath, true);
                this.targetsFileInfo = new FileInfo(TargetsFilePath);
            }

            [TestCleanup]
            public void Cleanup()
            {
                VsIdeTestHostContext.Dte.Solution.Close();

                // Remove file if written
                File.Delete(TargetsFilePath);
            }

            [TestMethod]
            [HostType("VS IDE")]
            [TestProperty("VSHostRestartOptions", "Before")]
            public void WhenNoSolutionOpen_ThenTargetsNotWritten()
            {
                Assert.True(IsTargetsFileExists());
                Assert.False(IsTargetsFileRewritten());
            }

            [TestMethod]
            [HostType("VS IDE")]
            [TestProperty("VSHostRestartOptions", "Before")]
            public void WhenCreateEmptySolution_ThenNewTargetsWritten()
            {
                Assert.True(IsTargetsFileExists());

                this.solution = (ISolution)VsIdeTestHostContext.ServiceProvider.GetService(typeof(ISolution));
                this.solution.CreateInstance(this.DeploymentDirectory, "EmptySolution");

                Assert.True(IsTargetsFileRewritten());
                Assert.True(IsTargetsFileCurrent());
            }

            [TestMethod]
            [HostType("VS IDE")]
            [TestProperty("VSHostRestartOptions", "Before")]
            public void WhenLoadingExistingSolution_ThenNewTargetsWritten()
            {
                Assert.True(IsTargetsFileExists());

                this.solution = (ISolution)VsIdeTestHostContext.ServiceProvider.GetService(typeof(ISolution));
                this.solution.Open(PathTo("VersionTargetsSpec\\SimpleLibrarySolution.sln"));

                Assert.True(IsTargetsFileRewritten());
                Assert.True(IsTargetsFileCurrent());
            }
        }

        [TestClass]
        [DeploymentItem("VersionTargetsSpec", "VersionTargetsSpec")]
        public class GivenCurrentToolkitVersionInTargets : IntegrationTest
        {
            private ISolution solution;
            private FileInfo targetsFileInfo;

            [TestInitialize]
            public void Initialize()
            {
                //Ensure targets file does exist
                File.Copy(PathTo("VersionTargetsSpec\\VersionTargets_Current.targets"), TargetsFilePath, true);
                this.targetsFileInfo = new FileInfo(TargetsFilePath);
            }

            [TestCleanup]
            public void Cleanup()
            {
                VsIdeTestHostContext.Dte.Solution.Close();

                // Remove file if written
                File.Delete(TargetsFilePath);
            }

            [TestMethod]
            [HostType("VS IDE")]
            [TestProperty("VSHostRestartOptions", "Before")]
            public void WhenNoSolutionOpen_ThenTargetsNotWritten()
            {
                Assert.True(IsTargetsFileExists());
                Assert.True(IsTargetsFileCurrentVersionOfToolkit());
                Assert.False(IsTargetsFileRewritten());
            }

            [TestMethod]
            [HostType("VS IDE")]
            [TestProperty("VSHostRestartOptions", "Before")]
            public void WhenCreateEmptySolution_ThenTargetsNotWritten()
            {
                Assert.True(IsTargetsFileExists());
                Assert.True(IsTargetsFileCurrentVersionOfToolkit());
                
                this.solution = (ISolution)VsIdeTestHostContext.ServiceProvider.GetService(typeof(ISolution));
                this.solution.CreateInstance(this.DeploymentDirectory, "EmptySolution");

                Assert.False(IsTargetsFileRewritten());
            }

            [TestMethod]
            [HostType("VS IDE")]
            [TestProperty("VSHostRestartOptions", "Before")]
            public void WhenLoadingExistingSolution_ThenTargetsNotWritten()
            {
                Assert.True(IsTargetsFileExists());
                Assert.True(IsTargetsFileCurrentVersionOfToolkit());

                this.solution = (ISolution)VsIdeTestHostContext.ServiceProvider.GetService(typeof(ISolution));
                this.solution.Open(PathTo("VersionTargetsSpec\\SimpleLibrarySolution.sln"));

                Assert.False(IsTargetsFileRewritten());
            }
        }
    }
}
