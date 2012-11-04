using System;
using System.IO;
using Microsoft.VSSDK.Tools.VsIdeTesting;
using Microsoft.VisualStudio.ExtensionManager;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.VisualStudio.Patterning.Runtime.IntegrationTests
{
	[TestClass]
	public class OnlineExtensionManagerRepositorySpec : IntegrationTest
	{
		private static readonly IAssertion Assert = new Assertion();

		private IVsExtensionManager extensionManager;
		private IVsExtensionRepository extensionRepository;

		[TestInitialize]
		public void Initialize()
		{
			this.extensionManager =
				VsIdeTestHostContext.ServiceProvider.GetService<SVsExtensionManager, IVsExtensionManager>();

			this.extensionRepository =
				VsIdeTestHostContext.ServiceProvider.GetService<SVsExtensionRepository, IVsExtensionRepository>();
		}

		[HostType("VS IDE")]
		[Ignore]
		[TestMethod]
		public void WhenDownloadingInSyncMode_ThenExtensionIsDownloadedAndInstalled()
		{
			var installableExtension =
				this.extensionRepository.Download(
					new ToolkitInfo
					{
						DownloadUri = new Uri(@"http://visualstudiogallery.msdn.microsoft.com/en-us/0791089a-4570-4f21-b5ee-78aba7e80651/file/8931"),
					}
					.AsRepositoryEntry());

			Assert.NotNull(installableExtension);
			Assert.True(File.Exists(installableExtension.PackagePath));

			Assert.Throws<NotInstalledException>(() =>
				this.extensionManager.GetInstalledExtension(installableExtension.Header.Identifier));

			this.extensionManager.Install(installableExtension, false);

			var installedExtension =
				this.extensionManager.GetInstalledExtension(installableExtension.Header.Identifier);

			Assert.True(installedExtension != null);

			this.extensionManager.Uninstall(installedExtension);
		}
	}
}
