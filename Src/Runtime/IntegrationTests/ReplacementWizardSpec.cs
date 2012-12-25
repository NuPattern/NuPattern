using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VSSDK.Tools.VsIdeTesting;
using NuPattern.Extensibility;

namespace NuPattern.Runtime.IntegrationTests
{
	[TestClass]
	[DeploymentItem(@"Runtime.IntegrationTests.Content\TestTemplate.zip")]
	public class ReplacementWizardSpec : IntegrationTest
	{
		internal static readonly IAssertion Assert = new Assertion();

		private ISolution solution;
		private ITemplate toolkitTemplate;

		[TestInitialize]
		public void Initialize()
		{
			this.solution = (ISolution)VsIdeTestHostContext.ServiceProvider.GetService(typeof(ISolution));
			this.solution.CreateInstance(this.DeploymentDirectory, "EmptySolution");

			var templates = (IFxrTemplateService)VsIdeTestHostContext.ServiceProvider.GetService(typeof(IFxrTemplateService));
            this.toolkitTemplate = templates.Find("MyTemplate2", "CSharp");
		}

		[TestMethod, TestCategory("Integration")]
		[HostType("VS IDE")]
		public void WhenUnfolding_ThenParameterIsReplaced()
		{
			var project = (IProject)this.toolkitTemplate.Unfold("Foo", this.solution);
			var item = project.Find(@"source.vsixmanifest").First();
			var itemContent = XElement.Load(item.PhysicalPath);

			Assert.Equal("$guid3$", (string)itemContent.Elements().First().Attribute("Id"));
		}
	}
}