using System.IO;
using Microsoft.VisualStudio.Patterning.Authoring.Authoring;
using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VSSDK.Tools.VsIdeTesting;

namespace Microsoft.VisualStudio.Patterning.Authoring.IntegrationTests
{
    /// <summary>
    /// NOTE: These tests will fail when run in the same VS host instance. 
    /// Need to find a way to run them in seperate VS instances.
    /// </summary>
    public class VersionTargetsSpec
    {
        internal static readonly IAssertion Assert = new Assertion();
        private static readonly string TargetsFilePath = VersionHelper.CalculateTargetsPath(AuthoringPackage.MsBuildPath);

        private static bool TargetsFileIsModified()
        {
            var currentFileInfo = new FileInfo(TargetsFilePath);

            if (currentFileInfo.CreationTime > currentFileInfo.LastWriteTime)
                return false;
            else
                return !currentFileInfo.LastWriteTime.Equals(currentFileInfo.CreationTime);
        }

        private static bool TargetsFileVersionIsCurrent()
        {
            var currentVersion = VersionHelper.GetTargetsVersion(TargetsFilePath);
            return currentVersion.Equals(AuthoringPackage.CurrentToolkitVersion);
        }

        [Ignore]
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
            public void WhenNoSolutionOpen_ThenTargetsNotWritten()
            {
                Assert.False(File.Exists(TargetsFilePath));
            }

            [TestMethod]
            [HostType("VS IDE")]
            public void WhenCreateEmptySolution_ThenTargetsWritten()
            {
                this.solution = (ISolution)VsIdeTestHostContext.ServiceProvider.GetService(typeof(ISolution));
                this.solution.CreateInstance(this.DeploymentDirectory, "EmptySolution");

                Assert.True(File.Exists(TargetsFilePath));
            }

            [TestMethod]
            [HostType("VS IDE")]
            public void WhenLoadingExistingSolution_ThenTargetsWritten()
            {
                this.solution = (ISolution)VsIdeTestHostContext.ServiceProvider.GetService(typeof(ISolution));
                this.solution.Open(PathTo("VersionTargetsSpec\\SimpleLibrarySolution.sln"));

                Assert.True(File.Exists(TargetsFilePath));
            }
        }

        [Ignore]
        [TestClass]
        [DeploymentItem("VersionTargetsSpec", "VersionTargetsSpec")]
        public class GivenWrongVersionOfTargets : IntegrationTest
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
            public void WhenNoSolutionOpen_ThenTargetsNotWritten()
            {
                Assert.True(File.Exists(TargetsFilePath));
                Assert.False(TargetsFileIsModified());
            }

            [TestMethod]
            [HostType("VS IDE")]
            public void WhenCreateEmptySolution_ThenNewTargetsWritten()
            {
                this.solution = (ISolution)VsIdeTestHostContext.ServiceProvider.GetService(typeof(ISolution));
                this.solution.CreateInstance(this.DeploymentDirectory, "EmptySolution");

                Assert.True(File.Exists(TargetsFilePath));
                Assert.True(TargetsFileIsModified());
                Assert.True(TargetsFileVersionIsCurrent());
            }

            [TestMethod]
            [HostType("VS IDE")]
            public void WhenLoadingExistingSolution_ThenNewTargetsWritten()
            {
                this.solution = (ISolution)VsIdeTestHostContext.ServiceProvider.GetService(typeof(ISolution));
                this.solution.Open(PathTo("VersionTargetsSpec\\SimpleLibrarySolution.sln"));

                Assert.True(File.Exists(TargetsFilePath));
                Assert.True(TargetsFileIsModified());
                Assert.True(TargetsFileVersionIsCurrent());
            }
        }

        [Ignore]
        [TestClass]
        [DeploymentItem("VersionTargetsSpec", "VersionTargetsSpec")]
        public class GivenCurrentVersionOfTargets : IntegrationTest
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
            public void WhenNoSolutionOpen_ThenTargetsNotWritten()
            {
                Assert.True(File.Exists(TargetsFilePath));
                Assert.False(TargetsFileIsModified());
                Assert.True(TargetsFileVersionIsCurrent());
            }

            [TestMethod]
            [HostType("VS IDE")]
            public void WhenCreateEmptySolution_ThenTargetsNotWritten()
            {
                this.solution = (ISolution)VsIdeTestHostContext.ServiceProvider.GetService(typeof(ISolution));
                this.solution.CreateInstance(this.DeploymentDirectory, "EmptySolution");

                Assert.True(File.Exists(TargetsFilePath));
                Assert.False(TargetsFileIsModified());
                Assert.True(TargetsFileVersionIsCurrent());
            }

            [TestMethod]
            [HostType("VS IDE")]
            public void WhenLoadingExistingSolution_ThenTargetsNotWritten()
            {
                this.solution = (ISolution)VsIdeTestHostContext.ServiceProvider.GetService(typeof(ISolution));
                this.solution.Open(PathTo("VersionTargetsSpec\\SimpleLibrarySolution.sln"));

                Assert.True(File.Exists(TargetsFilePath));
                Assert.False(TargetsFileIsModified());
                Assert.True(TargetsFileVersionIsCurrent());
            }
        }
    }
}
