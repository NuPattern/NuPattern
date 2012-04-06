using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using DataAnnotations = System.ComponentModel.DataAnnotations;

namespace Microsoft.VisualStudio.Patterning.Authoring.Guidance
{
	/// <summary>
	/// Defines a guidance processor.
	/// </summary>
	[CLSCompliant(false)]
	public interface IGuidanceProcessor
	{
		/// <summary>
		/// Calculates a guidance workflow.
		/// </summary>
		/// <returns></returns>
		Queue<INode> CalculateWorkflow();

		/// <summary>
		/// Generates guidance documents in specified path.
		/// </summary>
		/// <param name="targetPath"></param>
		/// <returns></returns>
		IEnumerable<string> GenerateWorkflowDocuments(string targetPath);

		/// <summary>
		/// Validates the guidance document for correctness and completeness.
		/// </summary>
		/// <returns></returns>
		IEnumerable<DataAnnotations.ValidationResult> ValidateDocument();
	}
}
