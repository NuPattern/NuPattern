using System.Collections.Generic;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VSSDK.Tools.VsIdeTesting;
using NuPattern.VisualStudio;

namespace NuPattern.Runtime.IntegrationTests
{
    [TestClass]
    public class ComponentModelExportsSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [HostType("VS IDE")]
        [TestMethod, TestCategory("Integration")]
        public void WhenGettingInstalledToolkitInfos_ThenCanRetrieveThem()
        {
            var componentModel = VsIdeTestHostContext.ServiceProvider.GetService<SComponentModel, IComponentModel>();
            var installed = componentModel.GetService<IEnumerable<IInstalledToolkitInfo>>();

            Assert.NotNull(installed);
        }
    }
}
