using System.IO;
using Microsoft.VSSDK.Tools.VsIdeTesting;
using Microsoft.VisualStudio.ExtensionManager;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.VisualStudio.Patterning.Runtime.IntegrationTests
{
    [TestClass]
    public class RuntimeShellPackageSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenNoContext : IntegrationTest
        {
            private string extensionDir;

            [TestInitialize]
            public void InitializeContext()
            {
                var extensionManager = VsIdeTestHostContext.ServiceProvider.GetService<SVsExtensionManager, IVsExtensionManager>();
                var runtimeExtension = extensionManager.GetInstalledExtension(Microsoft.VisualStudio.Patterning.Runtime.Shell.Constants.RuntimeShellPkgGuid);
                this.extensionDir = runtimeExtension.InstallPath;
            }

            [HostType("VS IDE")]
            [TestMethod, TestCategory("Integration")]
            public void ThenDependencyIsDownloadedAndDeployed()
            {
                Assert.True(File.Exists(Path.Combine(this.extensionDir, Shell.Constants.PackageDependencies.Marker)));
            }
        }
    }
}
