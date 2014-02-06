using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using NuPattern.ComponentModel.Design;
using NuPattern.Runtime.Design;
using NuPattern.Runtime.Properties;
using NuPattern.VisualStudio.Solution;

namespace NuPattern.Runtime.References
{
    /// <summary>
    /// Provides type information for the <see cref="SolutionArtifactLinkReference"/> reference kind.
    /// </summary>
    [ReferenceKindProvider]
    [DisplayNameResource(@"SolutionArtifactLinkReference_DisplayName", typeof(Resources))]
    [DescriptionResource(@"SolutionArtifactLinkReference_Description", typeof(Resources))]
    [Editor(typeof(SolutionItemUriEditor), typeof(UITypeEditor))]
    [SolutionEditorSettings(TitleResourceName = @"SolutionArtifactLinkReference_EditorTitle", ResourceType = typeof(Resources),
        Kind = ItemKind.Solution | ItemKind.SolutionFolder | ItemKind.Project | ItemKind.Folder | ItemKind.Item)]
    public class SolutionArtifactLinkReference : ReferenceKindProvider<SolutionArtifactLinkReference, Uri>
    {
        /// <summary>
        /// Returns all the <see cref="IReference"/>s for the current element that can be converted to the given type and are not empty.
        /// </summary>
        [CLSCompliant(false)]
        public static IEnumerable<IItemContainer> GetResolvedReferences(IProductElement element, IUriReferenceService uriService)
        {
            return GetResolvedReferences(element, uriService, r => true);
        }

        /// <summary>
        /// Returns all the <see cref="IReference"/>s for the current element that can be converted to the given type and are not empty.
        /// </summary>
        [CLSCompliant(false)]
        public static IEnumerable<IItemContainer> GetResolvedReferences(IProductElement element, IUriReferenceService uriService, Func<IReference, bool> whereFilter)
        {
            Guard.NotNull(() => element, element);
            Guard.NotNull(() => uriService, uriService);
            Guard.NotNull(() => whereFilter, whereFilter);

            return GetReferenceValues(element, whereFilter)
                .Select(reference => uriService.TryResolveUri<IItemContainer>(reference))
                .Where(item => item != null);
        }
    }
}