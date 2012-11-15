using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VSSDK.Tools.VsIdeTesting;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.VisualStudio.Patterning.Runtime.IntegrationTests
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
            var feature = registrations.FirstOrDefault(registration => registration.FeatureId == Runtime.Shell.Constants.VsixIdentifier);

            Assert.NotNull(feature);
        }
    }
}
