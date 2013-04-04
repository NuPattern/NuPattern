using System;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VSSDK.Tools.VsIdeTesting;

namespace NuPattern.Runtime.IntegrationTests.UriProviders
{
    [TestClass]
    public class TemplateUriProviderSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        private IFxrUriReferenceService service;

        [TestInitialize]
        public void Initialize()
        {
            this.service = VsIdeTestHostContext.ServiceProvider.GetService<IFxrUriReferenceService>();
        }

        [TestMethod, TestCategory("Integration")]
        [HostType("VS IDE")]
        public void WhenResolvingProjectTemplate_ThenGetsTemplate()
        {
            var template = this.service.ResolveUri<ITemplate>(new Uri("template://Project/CSharp/Microsoft.CSharp.ClassLibrary"));

            Assert.NotNull(template);
        }

        [TestMethod, TestCategory("Integration")]
        [HostType("VS IDE")]
        public void WhenResolvingItemTemplate_ThenGetsTemplate()
        {
            var template = this.service.ResolveUri<ITemplate>(new Uri("template://Item/CSharp/Microsoft.CSharp.Class"));

            Assert.NotNull(template);
        }
    }
}
