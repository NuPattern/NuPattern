using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VSSDK.Tools.VsIdeTesting;

namespace Microsoft.VisualStudio.Patterning.Runtime.IntegrationTests
{
	[TestClass]
	[DeploymentItem(@"Runtime.IntegrationTests.Content\TestTemplate.zip")]
	public class ReplacementWizardSpec : IntegrationTest
	{
		internal static readonly IAssertion Assert = new Assertion();

		private ISolution solution;
		private ITemplate factoryTemplate;
		private string targetTemplate;

		[TestInitialize]
		public void Initialize()
		{
			this.solution = (ISolution)VsIdeTestHostContext.ServiceProvider.GetService(typeof(ISolution));
			this.solution.CreateInstance(this.DeploymentDirectory, "EmptySolution");

			this.targetTemplate = Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
				@"Visual Studio 2010\Templates\ProjectTemplates\Visual C#\TestTemplate.zip");
			var source = Path.Combine(this.DeploymentDirectory, @"TestTemplate.zip");
			File.Copy(source, this.targetTemplate, true);

			var templates = (IFxrTemplateService)VsIdeTestHostContext.ServiceProvider.GetService(typeof(IFxrTemplateService));
			this.factoryTemplate = templates.Find("TestToolkit", "CSharp");
		}

		[TestCleanup]
		public void Cleanup()
		{
			VsIdeTestHostContext.Dte.Solution.Close();
			File.Delete(this.targetTemplate);
		}

		[TestMethod]
		[HostType("VS IDE")]
		public void WhenUnfolding_ThenParameterIsReplaced()
		{
			var project = (IProject)this.factoryTemplate.Unfold("Foo", this.solution);
			var item = project.Find(@"source.vsixmanifest").First();
			var itemContent = XElement.Load(item.PhysicalPath);

			Assert.Equal("$guid3$", (string)itemContent.Elements().First().Attribute("Id"));
		}
	}
}