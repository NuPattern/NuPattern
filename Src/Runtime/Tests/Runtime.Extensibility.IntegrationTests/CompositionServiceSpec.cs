using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VSSDK.Tools.VsIdeTesting;
using NuPattern.Runtime.Composition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuPattern.Runtime.IntegrationTests
{
    [TestClass]
    public class CompositionServiceSpec
    {
        [HostType("VS IDE")]
        [TestMethod]
        public void When_Retieving_Composition_Service_Then_Succeeds()
        {
            var components = VsIdeTestHostContext.ServiceProvider.GetService<SComponentModel, IComponentModel>();

            var composition = components.GetService<INuPatternCompositionService>();

            Assert.IsNotNull(composition);
        }
    }
}
