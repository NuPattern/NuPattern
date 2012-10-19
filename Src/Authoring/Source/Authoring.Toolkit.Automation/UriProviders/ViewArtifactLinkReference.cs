using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.Patterning.Runtime;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.Patterning.Authoring.Automation.Properties;

namespace Microsoft.VisualStudio.Patterning.Authoring.Automation.UriProviders
{
	/// <summary>
	/// Provides type information for the <see cref="ViewArtifactLinkReference"/> reference kind.
	/// </summary>
	[ReferenceKindProvider]
	[DisplayNameResource("ViewArtifactLinkReference_DisplayName", typeof(Resources))]
	[DescriptionResource("ViewArtifactLinkReference_Description", typeof(Resources))]
	public class ViewArtifactLinkReference : ReferenceKindProvider<ViewArtifactLinkReference, Uri>
	{
		/// <summary>
		/// Returns all the <see cref="IReference"/>s for the current element that can be converted to the given type and are not empty.
		/// </summary>
		[CLSCompliant(false)]
		public static IEnumerable<Guid> GetResolvedReferences(IProductElement element, IFxrUriReferenceService uriService)
		{
			return GetResolvedReferences(element, uriService, r => true);
		}

		/// <summary>
		/// Returns all the <see cref="IReference"/>s for the current element that can be converted to the given type and are not empty.
		/// </summary>
		[CLSCompliant(false)]
		public static IEnumerable<Guid> GetResolvedReferences(IProductElement element, IFxrUriReferenceService uriService, Func<IReference, bool> whereFilter)
		{
			Guard.NotNull(() => element, element);
			Guard.NotNull(() => uriService, uriService);
			Guard.NotNull(() => whereFilter, whereFilter);

			return Enumerable.Empty<Guid>();
		}
	}
}