namespace NuPattern.IntegrationTests.VisualStudio.Solution
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.VSSDK.Tools.VsIdeTesting;
    using NuPattern.VisualStudio.Solution;

    [TestClass]
    [DeploymentItem(@"Common.IntegrationTests.Content\\Sample", "Common.IntegrationTests.Content\\VsProjectSpec")]
    public class VsProjectSpec : IntegrationTest
    {
        private static readonly IAssertion Assert = new Assertion();

        private ISolution solution;

        [TestInitialize]
        public void Initialize()
        {
            this.solution = VsIdeTestHostContext.ServiceProvider.GetService<ISolution>();

            var solution = Path.Combine(this.TestContext.DeploymentDirectory, "Common.IntegrationTests.Content\\VsProjectSpec\\Sample.sln");
            VsIdeTestHostContext.Dte.Solution.Open(solution);
        }

        [HostType("VS IDE")]
        [TestMethod]
        public void WhenConfigurationDoesNotExist_ThenReadsAsPropertyName()
        {
            var project = this.solution.Find<IProject>().First();
            var value = project.Data["AssemblyName"] as string;

            Assert.Equal("Library", value);
        }

        [HostType("VS IDE")]
        [TestMethod]
        public void WhenReadingConfigurationProperty_ThenSucceeds()
        {
            var project = this.solution.Find<IProject>().First();
            var value = (string)project.Data["Debug"]["DebugType"];

            Assert.Equal("full", value);
        }

        [HostType("VS IDE")]
        [TestMethod]
        public void WhenReadingPlatformProperty_ThenSucceeds()
        {
            var project = this.solution.Find<IProject>().First();
            var value = (string)project.Data["x86"]["Bitness"];

            Assert.Equal("32bit", value);
        }

        [HostType("VS IDE")]
        [TestMethod]
        public void WhenSettingPlatformProperty_ThenSucceeds()
        {
            var project = this.solution.Find<IProject>().First();
            project.Data["x64"]["Bitness"] = "16bit";

            var value = project.Data["x64"]["Bitness"] as string;

            Assert.Equal("16bit", value);
        }

        [HostType("VS IDE")]
        [TestMethod]
        public void WhenSettingConfigurationProperty_ThenSucceeds()
        {
            var project = this.solution.Find<IProject>().First();
            project.Data["Release"]["DebugType"] = "full";

            var value = (string)project.Data["Release"]["DebugType"];

            Assert.Equal("full", value);
        }
    }
}