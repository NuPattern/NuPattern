using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VSSDK.Tools.VsIdeTesting;

namespace Microsoft.VisualStudio.Patterning.Runtime.IntegrationTests
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
                    .SingleOrDefault(t => t.Id == "Microsoft.VisualStudio.Patterning.Runtime.IntegrationTests.TestToolkit");
            }

            [HostType("VS IDE")]
            [TestMethod, TestCategory("Integration")]
            public void ThenTemplatesReturned()
            {
                Assert.Equal(3, this.toolkit.Templates.Count());
                Assert.Equal("DataContract", this.toolkit.Templates.First(t => Path.GetFileName(t.TemplateFileName) == "DataContract.vstemplate").TemplateData.Name.Value);
                Assert.Equal("MyTemplate1", this.toolkit.Templates.First(t => Path.GetFileName(t.TemplateFileName) == "MyTemplate1.vstemplate").TemplateData.Name.Value);
                Assert.Equal("MyTemplate2", this.toolkit.Templates.First(t => Path.GetFileName(t.TemplateFileName) == "MyTemplate2.vstemplate").TemplateData.Name.Value);
            }
        }
    }
}
