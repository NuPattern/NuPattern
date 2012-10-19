using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VSSDK.Tools.VsIdeTesting;

namespace Microsoft.VisualStudio.Patterning.Authoring.IntegrationTests
{
	[TestClass]
	public class ProjectUnfoldingSpec : IntegrationTest
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
			this.toolkitTemplate = templates.Find("301af1ad3bdb-PatternToolkit", "CSharp");
		}

		[TestCleanup]
		public void Cleanup()
		{
			VsIdeTestHostContext.Dte.Solution.Close();
		}

		[Ignore]
		[TestMethod]
		[HostType("VS IDE")]
		public void WhenUnfolding_ThenProjectIsUnfolded()
		{
			var project = (IProject)this.toolkitTemplate.Unfold("Foo", this.solution);

			Assert.Equal("Foo", project.Name);
		}
	}
}