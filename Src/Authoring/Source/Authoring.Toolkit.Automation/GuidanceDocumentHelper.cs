using System;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.Patterning.Authoring.Automation.Properties;
using Microsoft.VisualStudio.Patterning.Extensibility.References;
using Microsoft.VisualStudio.Patterning.Runtime;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;

namespace Microsoft.VisualStudio.Patterning.Authoring.Automation
{
	/// <summary>
	/// Helper class for automation of the guidance capabilities.
	/// </summary>
	internal static class GuidanceDocumentHelper
	{
		private const string GuidanceDocumentExtension = ".doc";

		/// <summary>
		/// Gets the path to the guidance document from the current element.
		/// </summary>
		/// <remarks>
		/// Returns the first artifact link with a *.doc extension of the current element.
		/// </remarks>
		public static string GetDocumentPath(ITraceSource tracer, IProductElement element, IFxrUriReferenceService uriService)
		{
			// Return path of first reference
			var references = SolutionArtifactLinkReference.GetResolvedReferences(element, uriService);
			if (!references.Any())
			{
				tracer.TraceWarning(String.Format(CultureInfo.CurrentCulture,
					Resources.GuidanceDocumentPathProvider_NoLinksFound, element.InstanceName));
				return string.Empty;
			}
			else
			{
				var reference = references.FirstOrDefault(r => r.PhysicalPath.EndsWith(GuidanceDocumentExtension));
				if (reference == null)
				{
					tracer.TraceWarning(String.Format(CultureInfo.CurrentCulture,
						Resources.GuidanceDocumentPathProvider_NoDocumentLinkFound, element.InstanceName));
					return string.Empty;
				}
				else
				{
					tracer.TraceInformation(String.Format(CultureInfo.CurrentCulture,
						Resources.GuidanceDocumentPathProvider_LinkFound, element.InstanceName, reference.PhysicalPath));
					return reference.PhysicalPath;
				}
			}
		}
	}
}
