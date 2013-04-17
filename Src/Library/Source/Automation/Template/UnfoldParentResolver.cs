using System;
using System.Globalization;
using System.IO;
using System.Linq;
using NuPattern.Library.Properties;
using NuPattern.Runtime;
using NuPattern.Runtime.References;
using NuPattern.VisualStudio.Solution;
using NuPattern.VisualStudio.Solution.Templates;

namespace NuPattern.Library.Automation.Template
{
    internal class UnfoldParentResolver
    {
        private ISolution solution;
        private IUriReferenceService uriService;
        private IProductElement currentElement;
        private IVsTemplate theTemplate;

        public UnfoldParentResolver(ISolution solution, IUriReferenceService uriService, IProductElement currentElement, IVsTemplate template)
        {
            this.solution = solution;
            this.uriService = uriService;
            this.currentElement = currentElement;
            this.theTemplate = template;
        }

        public void ResolveParent(string targetPath, string targetFileName)
        {
            var resolver = new PathResolver(this.currentElement, this.uriService,
                    string.IsNullOrEmpty(targetPath) ? "\\" : targetPath,
                    string.IsNullOrEmpty(targetFileName) ? this.currentElement.InstanceName : targetFileName);
            resolver.Resolve();

            if (string.IsNullOrEmpty(targetPath))
            {
                ParentItem = GetUnfoldParent(this.currentElement, theTemplate.Type);
            }
            else
            {
                ParentItem = solution.FindOrCreate(resolver.Path);
            }

            this.FileName = resolver.FileName;
        }

        public string ResolveExtension(string fileName)
        {
            if (theTemplate.Type == VsTemplateType.Item)
            {
                var firstItem = theTemplate.TemplateContent.Items.OfType<VSTemplateTemplateContentProjectItem>().FirstOrDefault();
                if (firstItem != null)
                {
                    var targetName = firstItem.TargetFileName ?? firstItem.Value;
                    if (!string.IsNullOrEmpty(targetName))
                    {
                        return Path.ChangeExtension(fileName, Path.GetExtension(targetName));
                    }
                }
            }

            return fileName;
        }

        private IItemContainer GetUnfoldParent(IProductElement element, VsTemplateType templateType)
        {
            var parentWithArtifactLink = element.Traverse(
                x => x.GetParentAutomation(),
                x => x.TryGetReference(ReferenceKindConstants.ArtifactLink) != null);

            if (parentWithArtifactLink != null)
            {
                var referencedItems = SolutionArtifactLinkReference.GetResolvedReferences(parentWithArtifactLink, this.uriService);

                if (templateType == VsTemplateType.Project || templateType == VsTemplateType.ProjectGroup)
                {
                    var parentItem = referencedItems.FirstOrDefault(item => item.Kind == ItemKind.Solution || item.Kind == ItemKind.SolutionFolder);
                    if (parentItem != null)
                        return parentItem;
                }
                else if (templateType == VsTemplateType.Item)
                {
                    var parentItem = referencedItems.FirstOrDefault(item => item.Kind == ItemKind.Project || item.Kind == ItemKind.Folder);
                    if (parentItem != null)
                        return parentItem;
                }

                // Why not continue up??? For extension points it makes perfect sense!
                if (!(parentWithArtifactLink is IProduct))
                {
                    // The Traverse at the top first checks the stop condition on the source element itself, 
                    // meaning in our case that we would enter an infinite loop if were to get here again
                    // and pass ourselves up. So we travel one element up for the traversal.
                    return GetUnfoldParent(parentWithArtifactLink.GetParentAutomation(), templateType);
                }
            }

            return GetUnfoldParent(this.solution, templateType);
        }

        private static IItemContainer GetUnfoldParent(ISolution solution, VsTemplateType templateType)
        {
            var selectedItem = solution.GetSelectedItems().FirstOrDefault() ?? solution;

            if (templateType == VsTemplateType.Project || templateType == VsTemplateType.ProjectGroup)
            {
                return selectedItem.Traverse(item => item.Parent, item => item is ISolution || (item is ISolutionFolder && !item.IsSolutionItemsFolder()));
            }

            if (templateType == VsTemplateType.Item)
            {
                var parentItem = (selectedItem.Kind == ItemKind.Folder || selectedItem.Kind == ItemKind.Project) ?
                    selectedItem :
                    selectedItem.Traverse(item => item.Parent, item => item is IFolder || item is IProject || item is ISolution);

                if (parentItem is ISolution)
                {
                    var item = parentItem.Find(SolutionExtensions.SolutionItemsFolderName).FirstOrDefault();
                    if (item != null)
                    {
                        return item;
                    }

                    return parentItem.AddDirectory(SolutionExtensions.SolutionItemsFolderName);
                }

                return parentItem;
            }

            throw new NotSupportedException(
                string.Format(CultureInfo.CurrentCulture, Resources.UnfoldParentResolver_UnsupportedTemplate, templateType));
        }

        public IItemContainer ParentItem { get; set; }

        public string FileName { get; set; }
    }
}
