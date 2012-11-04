using System.ComponentModel.Design;
using System.Linq;
using Microsoft.VisualStudio.Modeling.Diagrams;
using Microsoft.VisualStudio.Patterning.Runtime.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VSSDK.Tools.VsIdeTesting;

namespace Microsoft.VisualStudio.Patterning.Authoring.IntegrationTests
{
	public class ToolkitSchemaSpec
	{
		////internal static readonly IAssertion Assert = new Assertion();

		[DeploymentItem("SampleFactory", "SampleFactory")]
		[TestClass]
		public class GivenATailoredToolkit : IntegrationTest
		{
			[Ignore]
			[HostType("VS IDE")]
			[TestMethod, TestCategory("Integration")]
			public void WhenDeletingTailorableProperty_ThenThrows()
			{
				var dte = VsIdeTestHostContext.Dte;

				dte.Solution.Open(PathTo("SampleFactory\\Tailored\\SampleTailoring.sln"));
				dte.ExecuteCommand("File.OpenFile", "PatternModel.patterndefinition");

				var designer = new DslDesigner(VsIdeTestHostContext.ServiceProvider);

				var patternShape = PresentationViewsSubject.GetPresentation(((PatternModelSchema)designer.DocData.RootElement).Pattern)
					.OfType<ShapeElement>().First();

				var tailorableProperty = patternShape.FindLastChild(true).Shape.FindLastChild(true);

				designer.DocView.CurrentDiagram.ActiveDiagramView.Selection.FocusedItem = tailorableProperty;

				// TODO: couldn't get the command to execute. I'm getting a null ref at the bottom of the ocean.
				var commands = new Commands(VsIdeTestHostContext.ServiceProvider);

				commands.Execute(StandardCommands.Delete);
			}
		}
	}
}
