using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using NuPattern.ComponentModel.Design;
using NuPattern.Extensibility.Design;
using NuPattern.Extensibility.Properties;
using NuPattern.Runtime;

namespace NuPattern.Extensibility.References
{
    /// <summary>
    /// Provides type information for the <see cref="SolutionArtifactLinkReference"/> reference kind.
    /// </summary>
    [ReferenceKindProvider]
    [DisplayNameResource("SolutionArtifactLinkReference_DisplayName", typeof(Resources))]
    [DescriptionResource("SolutionArtifactLinkReference_Description", typeof(Resources))]
    [Editor(typeof(SolutionItemUriEditor), typeof(UITypeEditor))]
    [SolutionEditorSettings(TitleResourceName = "SolutionArtifactLinkReference_EditorTitle", ResourceType = typeof(Resources),
        Kind = ItemKind.Solution | ItemKind.SolutionFolder | ItemKind.Project | ItemKind.Folder | ItemKind.Item)]
    public class SolutionArtifactLinkReference : ReferenceKindProvider<SolutionArtifactLinkReference, Uri>
    {
        /// <summary>
        /// Returns all the <see cref="IReference"/>s for the current element that can be converted to the given type and are not empty.
        /// </summary>
        [CLSCompliant(false)]
        public static IEnumerable<IItemContainer> GetResolvedReferences(IProductElement element, IFxrUriReferenceService uriService)
        {
            return GetResolvedReferences(element, uriService, r => true);
        }

        /// <summary>
        /// Returns all the <see cref="IReference"/>s for the current element that can be converted to the given type and are not empty.
        /// </summary>
        [CLSCompliant(false)]
        public static IEnumerable<IItemContainer> GetResolvedReferences(IProductElement element, IFxrUriReferenceService uriService, Func<IReference, bool> whereFilter)
        {
            Guard.NotNull(() => element, element);
            Guard.NotNull(() => uriService, uriService);
            Guard.NotNull(() => whereFilter, whereFilter);

            return GetReferences(element, whereFilter)
                .Select(reference => uriService.TryResolveUri<IItemContainer>(reference))
                .Where(item => item != null);
        }
    }
}