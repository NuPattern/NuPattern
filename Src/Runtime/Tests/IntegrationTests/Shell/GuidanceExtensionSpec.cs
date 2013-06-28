using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VSSDK.Tools.VsIdeTesting;
using NuPattern.Runtime.Guidance;

namespace NuPattern.Runtime.IntegrationTests
{
    [TestClass]
    public class GuidanceExtensionSpec
    {
        internal static readonly IAssertion Assert = new Assertion();
        private IGuidanceManager guidanceManager;

        [TestInitialize]
        public void InitializeContext()
        {
            this.guidanceManager = (IGuidanceManager)VsIdeTestHostContext.ServiceProvider.GetService(typeof(IGuidanceManager));
        }

        [HostType("VS IDE")]
        [TestMethod, TestCategory("Integration")]
        public void ThenGuidanceExtensionIsInstalled()
        {
            var registrations = this.guidanceManager.InstalledGuidanceExtensions;
            var extension = registrations.FirstOrDefault(registration => registration.ExtensionId == Runtime.Shell.Constants.VsixIdentifier);

            Assert.NotNull(extension);
        }
    }
}
