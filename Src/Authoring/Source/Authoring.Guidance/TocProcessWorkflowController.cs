using System;
using System.Globalization;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;

namespace Microsoft.VisualStudio.Patterning.Authoring.Guidance
{
	/// <summary>
	/// A text template controller for transforming a Word guidance document into a feature extension TOC guidance workflow.
	/// </summary>
	[CLSCompliant(false)]
	public class TocProcessWorkflowController
	{
		private int nodeIndex = 0;
		private TocProcessWorkflowTextTransformation template;
		private TocGuidanceProcessor processor;

		/// <summary>
		/// Initializes a new instance of the <see cref="TocProcessWorkflowController"/> class.
		/// </summary>
		public TocProcessWorkflowController(TocProcessWorkflowTextTransformation template, string documentPath, string vsixId, string contentPath)
		{
			Guard.NotNull(() => template, template);
			Guard.NotNull(() => documentPath, documentPath);
			Guard.NotNull(() => vsixId, vsixId);
			Guard.NotNull(() => contentPath, contentPath);

			this.processor = new TocGuidanceProcessor(documentPath, vsixId, contentPath);
			this.template = template;
		}

		/// <summary>
		/// Renders the workflow.
		/// </summary>
		public void RenderWorkflow()
		{
			// Calculate the workflow
			var nodes = this.processor.CalculateWorkflow();

			// Get initial node
			var initialNode = nodes.Dequeue();
			var nextNodeIdentifier = WriteInitialConnectionCode(initialNode);

			// Connect up remaining workflow
			while (nodes.Count > 0)
			{
				//Write out connection code
				var node = nodes.Dequeue();
				nextNodeIdentifier = WriteNodeConnectionCode(node, nextNodeIdentifier);
			}
		}

		private string GetNodeVariableName(INode node)
		{
			nodeIndex++;
			return string.Format(CultureInfo.CurrentCulture, "{0}{1}", node.GetType().Name.ToLower(), nodeIndex);
		}

		private string WriteInitialConnectionCode(INode initial)
		{
			var initialIdentifier = GetNodeVariableName(initial);
			this.template.WriteNodeVariableCode(initial, initialIdentifier);
			this.template.WriteInitialConnectionCode(initialIdentifier);

			return initialIdentifier;
		}

		private string WriteNodeConnectionCode(INode node, string predecessorIdentifier)
		{
			var nodeIdentifier = GetNodeVariableName(node);
			this.template.WriteNodeVariableCode(node, nodeIdentifier);
			this.template.WriteNodeConnectionCode(predecessorIdentifier, nodeIdentifier);

			return nodeIdentifier;
		}
	}
}
