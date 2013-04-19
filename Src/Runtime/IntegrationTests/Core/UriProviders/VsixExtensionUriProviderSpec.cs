using System;
using System.Linq;
using Microsoft.VisualStudio.ExtensionManager;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VSSDK.Tools.VsIdeTesting;

namespace NuPattern.Runtime.IntegrationTests.UriProviders
{
    [TestClass]
    public class VsixExtensionUriProviderSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [HostType("VS IDE")]
        [TestMethod, TestCategory("Integration")]
        public void WhenResolvingUri_ThenNotFeatureRuntimeVsix()
        {
            var service = VsIdeTestHostContext.ServiceProvider.GetService<IUriReferenceService>();
            Assert.NotNull(service);
            var uri = new Uri("vsix://FeatureExtensionRuntime");

            var installed = service.ResolveUri<IInstalledExtension>(uri);
            var extension = service.ResolveUri<IExtension>(uri);

            Assert.Null(installed);
            Assert.Null(extension);
        }

        [HostType("VS IDE")]
        [TestMethod, TestCategory("Integration")]
        public void WhenResolvingUri_ThenRuntimeVsix()
        {
            var service = VsIdeTestHostContext.ServiceProvider.GetService<IUriReferenceService>();
            Assert.NotNull(service);
            var uri = new Uri("vsix://" + Runtime.Shell.Constants.VsixIdentifier);

            var installed = service.ResolveUri<IInstalledExtension>(uri);
            var extension = service.ResolveUri<IExtension>(uri);

            Assert.NotNull(installed);
            Assert.NotNull(extension);
        }

        [HostType("VS IDE")]
        [TestMethod, TestCategory("Integration")]
        public void WhenCreatingUri_ThenCanRoundTrip()
        {
            var service = VsIdeTestHostContext.ServiceProvider.GetService<IUriReferenceService>();
            Assert.NotNull(service);
            var installed = VsIdeTestHostContext.ServiceProvider.GetService<SVsExtensionManager, IVsExtensionManager>().GetInstalledExtensions().First();

            Assert.True(service.CanCreateUri(installed));
            Assert.True(service.CanCreateUri<IExtension>(installed));

            var uri = service.CreateUri(installed);
            var resolved = service.ResolveUri<IExtension>(uri);
            var resolved2 = service.ResolveUri<IInstalledExtension>(uri);

            Assert.Equal(installed.Header.Identifier, resolved.Header.Identifier);
            Assert.Equal(installed.Header.Identifier, resolved2.Header.Identifier);
        }
    }
}
