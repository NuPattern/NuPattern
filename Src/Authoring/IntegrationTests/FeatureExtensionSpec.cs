using System.Linq;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VSSDK.Tools.VsIdeTesting;
using NuPattern.Authoring.PatternToolkit;

namespace NuPattern.Authoring.IntegrationTests
{
    [TestClass]
    public class FeatureExtensionSpec
    {
        internal static readonly IAssertion Assert = new Assertion();
        private IFeatureManager featureManager;

        [TestInitialize]
        public void InitializeContext()
        {
            this.featureManager = (IFeatureManager)VsIdeTestHostContext.ServiceProvider.GetService(typeof(IFeatureManager));
        }

        [HostType("VS IDE")]
        [TestMethod, TestCategory("Integration")]
        public void ThenFeatureExtensionIsInstalled()
        {
            var registrations = this.featureManager.InstalledFeatures;
            var feature = registrations.FirstOrDefault(registration => registration.FeatureId == AuthoringToolkitInfo.VsixIdentifier);

            Assert.NotNull(feature);
        }
    }
}
