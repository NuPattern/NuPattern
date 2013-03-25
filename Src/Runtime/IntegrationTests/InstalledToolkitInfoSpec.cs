using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VSSDK.Tools.VsIdeTesting;
using NuPattern.Extensibility;
using NuPattern.VisualStudio.Extensions;
using NuPattern.VisualStudio.Solution;

namespace NuPattern.Runtime.IntegrationTests
{
    [TestClass]
    public class InstalledToolkitInfoSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenInstalledToolkit : IntegrationTest
        {
            private ISolution solution;
            private IPatternManager patternManager;
            private IInstalledToolkitInfo toolkit;

            [TestInitialize]
            public void InitializeContext()
            {
                this.solution = VsIdeTestHostContext.ServiceProvider.GetService<ISolution>();
                this.solution.CreateInstance(this.DeploymentDirectory, "Blank");

                this.patternManager = VsIdeTestHostContext.ServiceProvider.GetService<IPatternManager>();

                this.toolkit = patternManager.InstalledToolkits
                    .SingleOrDefault(t => t.Id == "NuPattern.Runtime.IntegrationTests.TestToolkit");
            }

            [TestCleanup]
            public void Cleanup()
            {
                if (this.solution != null
                    && this.solution.IsOpen)
                {
                    this.solution.Close(false);
                }
            }

            [HostType("VS IDE")]
            [TestMethod, TestCategory("Integration")]
            public void ThenTemplatesReturned()
            {
                Assert.Equal(3, this.toolkit.Templates.Count());
                Assert.Equal("DataContract", this.toolkit.Templates.First(t => Path.GetFileName(t.TemplateFileName) == "DataContract.gen.vstemplate").TemplateData.Name.Value);
                Assert.Equal("MyTemplate1", this.toolkit.Templates.First(t => Path.GetFileName(t.TemplateFileName) == "MyTemplate1.gen.vstemplate").TemplateData.Name.Value);
                Assert.Equal("MyTemplate2", this.toolkit.Templates.First(t => Path.GetFileName(t.TemplateFileName) == "MyTemplate2.gen.vstemplate").TemplateData.Name.Value);
            }

            [HostType("VS IDE")]
            [TestMethod, TestCategory("Integration")]
            public void ThenClassificationReturned()
            {
                Assert.Equal("Test", this.toolkit.Classification.Category);
                Assert.Equal(ExtensionVisibility.Collapsed, this.toolkit.Classification.CreateVisibility);
                Assert.Equal(ExtensionVisibility.Hidden, this.toolkit.Classification.CustomizeVisibility);
            }
        }
    }
}
