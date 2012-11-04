using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.ExtensionManager;
using Microsoft.VisualStudio.Patterning.Runtime.UriProviders;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Microsoft.VisualStudio.Patterning.Runtime.UnitTests.UriProviders
{
	[TestClass]
	public class VsixExtensionUriProviderSpec
	{
		internal static readonly IAssertion Assert = new Assertion();

		[TestMethod, TestCategory("Unit")]
		public void WhenNullExtensionManager_ThenThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new VsixExtensionUriProvider(null, file => { }));
		}

		[TestMethod, TestCategory("Unit")]
		public void WhenNullOpenFileAction_ThenThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new VsixExtensionUriProvider(new Mock<IVsExtensionManager>().Object, null));
		}

		[TestClass]
		public class GivenAnExtensionManager
		{
			IFxrUriReferenceProvider<IInstalledExtension> provider;
			Mock<IVsExtensionManager> manager;
			Action<string> openFileAction = file => { };

			[TestInitialize]
			public void Initialize()
			{
				this.manager = new Mock<IVsExtensionManager>();
				this.provider = new VsixExtensionUriProvider(this.manager.Object, file => this.openFileAction(file));
			}

			[TestMethod, TestCategory("Unit")]
			public void WhenOpeningExtension_ThenInvokesOpenAction()
			{
				var extension = Mocks.Of<IInstalledExtension>().First(x => x.InstallPath == Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()));
				Directory.CreateDirectory(extension.InstallPath);
				File.WriteAllText(Path.Combine(extension.InstallPath, "extension.vsixmanifest"), "");
				string openedFile = null;
				this.openFileAction = file => openedFile = file;

				this.provider.Open(extension);

				Assert.NotNull(openedFile);
				Assert.Equal("extension.vsixmanifest", Path.GetFileName(openedFile));
			}

			[TestMethod, TestCategory("Unit")]
			public void WhenOpeningExtensionWithMissingManifestFile_ThenThrowsArgumentException()
			{
				var extension = Mocks.Of<IInstalledExtension>().First(x =>
					x.InstallPath == Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()) &&
					x.Header.Identifier == "foo-extension");

				Assert.Throws<ArgumentException>(() => this.provider.Open(extension));
			}

			[TestMethod, TestCategory("Unit")]
			public void WhenResolvingUriWithUnsupportedScheme_ThenThrowsNotSupportedException()
			{
				Assert.Throws<NotSupportedException>(() => this.provider.ResolveUri(new Uri("foo://bar")));
			}

			[TestMethod, TestCategory("Unit")]
			public void WhenResolvingValidUri_ThenRetrievesInstalledExtensionFromManager()
			{
				var extension = Mocks.Of<IInstalledExtension>().First(x => x.Header.Identifier == "foo-extension");
				this.manager.Setup(x => x.GetInstalledExtensions()).Returns(new[] { extension });

				var resolved = this.provider.ResolveUri(new Uri("vsix://foo-extension"));

				Assert.Same(extension, resolved);
			}
		}
	}
}
