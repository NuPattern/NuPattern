using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VSSDK.Tools.VsIdeTesting;
using Moq;

namespace Microsoft.VisualStudio.Patterning.Extensibility.IntegrationTests
{
    public class DteExtensionsSpec
    {
        private static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenAContext : IntegrationTest
        {
            private EnvDTE.DTE dte;
            private Mock<IRegistryReader> reader;

            [TestInitialize]
            public void Initialize()
            {
                this.reader = new Mock<IRegistryReader>();
                this.reader.Setup(r => r.ReadValue()).Returns(this.ProjectsDirectory);
                if (!Directory.Exists(this.ProjectsDirectory))
                {
                    Directory.CreateDirectory(this.ProjectsDirectory);
                }

                this.dte = VsIdeTestHostContext.Dte;
            }

            [TestCleanup]
            public void Cleanup()
            {
                VsIdeTestHostContext.Dte.Solution.Close(false);
                if (Directory.Exists(this.ProjectsDirectory))
                {
                    Directory.Delete(this.ProjectsDirectory, true);
                }
            }

            private string ProjectsDirectory
            {
                get
                {
                    return Path.Combine(this.TestContext.DeploymentDirectory, "Projects");
                }
            }

            [HostType("VS IDE")]
            [TestMethod, TestCategory("Integration")]
            public void WhenDefaultProjectsDirNotExist_ThenThrows()
            {
                this.reader.Setup(r => r.ReadValue()).Returns(string.Empty);

                Assert.Throws<InvalidOperationException>(
                    ()=> this.dte.CreateBlankSolution(this.reader.Object));
            }

            [HostType("VS IDE")]
            [TestMethod, TestCategory("Integration")]
            public void WhenProjectsDirEmpty_ThenNewSolutionCreated()
            {
                this.dte.CreateBlankSolution(this.reader.Object);

                Assert.True(Directory.Exists(Path.Combine(this.ProjectsDirectory, "Solution1")));
                Assert.True(File.Exists(Path.Combine(Path.Combine(this.ProjectsDirectory, "Solution1"), "Solution1.sln")));
            }

            [HostType("VS IDE")]
            [TestMethod, TestCategory("Integration")]
            public void WhenPreviousSolutionExist_ThenNewConsecutiveSolutionCreated()
            {
                this.dte.CreateBlankSolution(this.reader.Object);

                Assert.True(Directory.Exists(Path.Combine(this.ProjectsDirectory, "Solution1")));
                Assert.True(File.Exists(Path.Combine(Path.Combine(this.ProjectsDirectory, "Solution1"), "Solution1.sln")));

                this.dte.CreateBlankSolution(this.reader.Object);

                Assert.True(Directory.Exists(Path.Combine(this.ProjectsDirectory, "Solution2")));
                Assert.True(File.Exists(Path.Combine(Path.Combine(this.ProjectsDirectory, "Solution2"), "Solution2.sln")));

                this.dte.CreateBlankSolution(this.reader.Object);

                Assert.True(Directory.Exists(Path.Combine(this.ProjectsDirectory, "Solution3")));
                Assert.True(File.Exists(Path.Combine(Path.Combine(this.ProjectsDirectory, "Solution3"), "Solution3.sln")));
            }
        }
    }
}
