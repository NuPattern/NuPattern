using System.Collections.Generic;
using Microsoft.VisualStudio.Patterning.Authoring.Guidance;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;

namespace Microsoft.VisualStudio.Patterning.Authoring.IntegrationTests
{
	internal class TestTocProcessWorkflow : TocProcessWorkflowTextTransformation
	{
		private GuidanceWorkflow workflow;
		private Dictionary<string, INode> nodes;

		public TestTocProcessWorkflow()
		{
			this.workflow = new GuidanceWorkflow();
			this.nodes = new Dictionary<string, INode>();
		}

		public override void WriteInitialConnectionCode(string initialIdentifier)
		{
			var initial = this.nodes[initialIdentifier] as Initial;
			this.workflow.ConnectTo(initial);
		}

		public override void WriteNodeConnectionCode(string predecessorIdentifier, string nodeIdentifier)
		{
			var predecessor = this.nodes[predecessorIdentifier];
			var current = this.nodes[nodeIdentifier];
		}

		public override void WriteNodeVariableCode(INode node, string nodeIdentifier)
		{
			this.nodes.Add(nodeIdentifier, node);
		}

		public override string TransformText()
		{
			throw new System.NotImplementedException();
		}
	}
}
