using System;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using NuPattern.Library;

namespace NuPattern.Authoring.Guidance
{
	/// <summary>
	/// A text template class for transforming a TOC workflow.
	/// </summary>
	[CLSCompliant(false)]
	public abstract class TocProcessWorkflowTextTransformation : ModelElementTextTransformation
	{
		/// <summary>
		/// Renders the initial connection code.
		/// </summary>
		public abstract void WriteInitialConnectionCode(string initialIdentifier);

		/// <summary>
		/// Renders the connection code for any type of node.
		/// </summary>
		public abstract void WriteNodeConnectionCode(string predecessorIdentifier, string nodeIdentifier);

		/// <summary>
		/// Renders the connection code for any type of node.
		/// </summary>
		public abstract void WriteNodeVariableCode(INode node, string nodeIdentifier);
	}
}
