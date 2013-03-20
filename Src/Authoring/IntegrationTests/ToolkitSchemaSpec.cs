using System.ComponentModel.Design;
using System.Linq;
using Microsoft.VisualStudio.Modeling.Diagrams;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VSSDK.Tools.VsIdeTesting;
using NuPattern.Runtime.Schema;

namespace NuPattern.Authoring.IntegrationTests
{
    public class ToolkitSchemaSpec
    {
        ////internal static readonly IAssertion Assert = new Assertion();

        [Ignore] // See TODO Comment below
        [TestClass]
        [DeploymentItem(@"Authoring.IntegrationTests.Content\SampleToolkit", "SampleToolkit")]
        public class GivenATailoredToolkit : IntegrationTest
        {
            [HostType("VS IDE")]
            [TestMethod, TestCategory("Integration")]
            public void WhenDeletingTailorableProperty_ThenThrows()
            {
                var dte = VsIdeTestHostContext.Dte;

                dte.Solution.Open(PathTo("SampleToolkit\\Tailored\\SampleTailoring.gen.sln"));
                dte.ExecuteCommand("File.OpenFile", "PatternModel.gen.patterndefinition");

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
