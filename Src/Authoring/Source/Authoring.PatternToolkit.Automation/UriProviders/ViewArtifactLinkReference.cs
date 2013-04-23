using System;
using System.Collections.Generic;
using System.Linq;
using NuPattern.Authoring.PatternToolkit.Automation.Properties;
using NuPattern.ComponentModel.Design;
using NuPattern.Runtime;
using NuPattern.Runtime.References;

namespace NuPattern.Authoring.PatternToolkit.Automation.UriProviders
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
        public static IEnumerable<Guid> GetResolvedReferences(IProductElement element, IUriReferenceService uriService)
        {
            return GetResolvedReferences(element, uriService, r => true);
        }

        /// <summary>
        /// Returns all the <see cref="IReference"/>s for the current element that can be converted to the given type and are not empty.
        /// </summary>
        [CLSCompliant(false)]
        public static IEnumerable<Guid> GetResolvedReferences(IProductElement element, IUriReferenceService uriService, Func<IReference, bool> whereFilter)
        {
            Guard.NotNull(() => element, element);
            Guard.NotNull(() => uriService, uriService);
            Guard.NotNull(() => whereFilter, whereFilter);

            return Enumerable.Empty<Guid>();
        }
    }
}