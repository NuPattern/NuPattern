using System;
using System.Linq;
using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.ExtensionManager;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VSSDK.Tools.VsIdeTesting;

namespace Microsoft.VisualStudio.Patterning.Runtime.IntegrationTests.UriProviders
{
	[TestClass]
	public class VsixExtensionUriProviderSpec
	{
		internal static readonly IAssertion Assert = new Assertion();

		[HostType("VS IDE")]
		[TestMethod]
		public void WhenResolvingUri_ThenNotFeatureRuntimeVsix()
		{
			var service = VsIdeTestHostContext.ServiceProvider.GetService<IFxrUriReferenceService>();
			Assert.NotNull(service);
			var uri = new Uri("vsix://FeatureExtensionRuntime");

			var installed = service.ResolveUri<IInstalledExtension>(uri);
			var extension = service.ResolveUri<IExtension>(uri);

			Assert.Null(installed);
			Assert.Null(extension);
		}

        [HostType("VS IDE")]
        [TestMethod]
        public void WhenResolvingUri_ThenRuntimeVsix()
        {
            var service = VsIdeTestHostContext.ServiceProvider.GetService<IFxrUriReferenceService>();
            Assert.NotNull(service);
            var uri = new Uri("vsix://93373818-600f-414b-8181-3a0cb79fa785");

            var installed = service.ResolveUri<IInstalledExtension>(uri);
            var extension = service.ResolveUri<IExtension>(uri);

            Assert.NotNull(installed);
            Assert.NotNull(extension);
        }

		[HostType("VS IDE")]
		[TestMethod]
		public void WhenCreatingUri_ThenCanRoundTrip()
		{
			var service = VsIdeTestHostContext.ServiceProvider.GetService<IFxrUriReferenceService>();
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
