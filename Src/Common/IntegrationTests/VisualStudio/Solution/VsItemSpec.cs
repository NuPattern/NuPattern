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
    [DeploymentItem(@"Common.IntegrationTests.Content\\Sample", "Common.IntegrationTests.Content\\VsItemSpec")]
    public class VsItemSpec : IntegrationTest
    {
        private static readonly IAssertion Assert = new Assertion();

        private ISolution solution;

        [TestInitialize]
        public void Initialize()
        {
            this.solution = VsIdeTestHostContext.ServiceProvider.GetService<ISolution>();

            var solution = Path.Combine(this.TestContext.DeploymentDirectory, "Common.IntegrationTests.Content\\VsItemSpec\\Sample.sln");
            VsIdeTestHostContext.Dte.Solution.Open(solution);
        }

        [HostType("VS IDE")]
        [TestMethod]
        public void WhenGettingPropertyByName_ThenSucceeds()
        {
            var item = this.solution.Find<IItem>().First();
            var value = (string)item.Data["Generated"];

            Assert.Equal("true", value);
        }

        [HostType("VS IDE")]
        [TestMethod]
        public void WhenSettingPropertyByName_ThenSucceeds()
        {
            var item = this.solution.Find<IItem>().First();
            item.Data["Generated"] = "false";

            var value = (string)item.Data["Generated"];

            Assert.Equal("false", value);
        }
    }
}