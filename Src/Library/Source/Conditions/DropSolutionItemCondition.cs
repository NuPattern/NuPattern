using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using NuPattern.Extensibility;
using NuPattern.Library.Properties;

namespace NuPattern.Library.Conditions
{
	/// <summary>
	/// Checks if the dragged data contains solution items that can be dropped
	/// </summary>
	[DisplayNameResource("DropSolutionItemCondition_DisplayName", typeof(Resources))]
	[DescriptionResource("DropSolutionItemCondition_Description", typeof(Resources))]
	[CategoryResource("AutomationCategory_Automation", typeof(Resources))]
	[CLSCompliant(false)]
	public class DropSolutionItemCondition : DropFileCondition
	{
		private static readonly ITraceSource tracer = Tracer.GetSourceFor<DropSolutionItemCondition>();

		/// <summary>
		/// Returns the solution items that can be dragged over the current element.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<string> GetDraggedFiles()
		{
			tracer.TraceInformation(
				Resources.DropSolutionItemCondition_TraceGettingFiles, this.Extension);

			var items = this.DragArgs.GetVSProjectItemsPaths();
			if (items.Any())
			{
				return items.GetPathsEndingWithExtensions(this.Extension);
			}

			return Enumerable.Empty<string>();
		}
	}
}
