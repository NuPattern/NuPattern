using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VSSDK.Tools.VsIdeTesting;
using NuPattern.VisualStudio.Solution;

namespace NuPattern.Runtime.IntegrationTests.UriProviders
{
    [TestClass]
    public class TemplateUriProviderSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        private IUriReferenceService service;

        [TestInitialize]
        public void Initialize()
        {
            this.service = VsIdeTestHostContext.ServiceProvider.GetService<IUriReferenceService>();
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
