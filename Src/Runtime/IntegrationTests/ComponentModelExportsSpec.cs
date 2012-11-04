using System.Collections.Generic;
using Microsoft.VSSDK.Tools.VsIdeTesting;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.VisualStudio.Patterning.Runtime.IntegrationTests
{
    [TestClass]
    public class ComponentModelExportsSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [HostType("VS IDE")]
        [TestMethod]
        public void WhenGettingInstalledToolkitInfos_ThenCanRetrieveThem()
        {
            var componentModel = VsIdeTestHostContext.ServiceProvider.GetService<SComponentModel, IComponentModel>();
            var installed = componentModel.GetService<IEnumerable<IInstalledToolkitInfo>>();

            Assert.NotNull(installed);
        }
    }
}
