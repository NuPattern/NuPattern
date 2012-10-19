using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Patterning.Runtime;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.ExtensionManager;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.VisualStudio.Patterning.Repository.Shell
{
	/// <summary>
	/// Defines the package for the sample project.
	/// </summary>
	[ProvideAutoLoad(UIContextGuids.NoSolution)]
	[PackageRegistration(UseManagedResourcesOnly = true)]
	[Guid(Constants.SamplesShellPackageGuid)]
	[CLSCompliant(false)]
	public sealed class SamplesShellPackage : Package
	{
		private const string RepositoriesFolder = "Repositories";

		/// <summary>
		/// Called when the VSPackage is loaded by Visual Studio.
		/// </summary>
		protected override void Initialize()
		{
			base.Initialize();

			this.RegisterRepository();
		}

		private void RegisterRepository()
		{
			var componentModel = this.GetService<SComponentModel, IComponentModel>();
			var compositionContainer = (CompositionContainer)componentModel.DefaultCompositionService;
			var extensionManager = this.GetService<SVsExtensionManager, IVsExtensionManager>();

			var extension = extensionManager.GetInstalledExtension("5905b03e-fb73-46df-bd96-820c17c2525e");

			var path = Path.Combine(extension.InstallPath, RepositoriesFolder);

			IFactoryRepository fs =
				new FileSystemRepository(
					string.Format(CultureInfo.CurrentCulture, "File Repository: {0}", extension.Header.Name),
					path);

			IFactoryRepository wss4 =
				new Wss4Repository(string.Format(CultureInfo.CurrentCulture, "WSS Repository: {0}", extension.Header.Name),
				new Uri(@"http://lfenster-t61p/testsite"),
				@"test document library");

			compositionContainer.ComposeExportedValue<IFactoryRepository>(fs);
			compositionContainer.ComposeExportedValue<IFactoryRepository>(wss4);

			compositionContainer.ComposeExportedValue<IFactoryRepository>(new OnlineGalleryRepository());
		}
	}
}