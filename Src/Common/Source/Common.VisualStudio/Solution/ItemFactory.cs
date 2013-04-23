using System;
using System.Diagnostics;
using Microsoft.VisualStudio.Shell.Interop;
using NuPattern.VisualStudio.Solution.Hierarchy;

namespace NuPattern.VisualStudio.Solution
{
    internal static class ItemFactory
    {
        /// <summary>
        /// Create an IItemContainer wrapper object against IVsSolution
        /// </summary>
        internal static IItemContainer CreateItem(IServiceProvider serviceProvider, IVsSolution solution, uint selectedItemid)
        {
            Debug.Assert(serviceProvider != null && solution != null);
            using (HierarchyNode parent = new HierarchyNode(solution))
            {
                return CreateItem(serviceProvider, parent, selectedItemid);
            }
        }

        /// <summary>
        /// Create an IItemContainer wrapper object against pHier with itemId represents the selected item
        /// </summary>
        internal static IItemContainer CreateItem(IServiceProvider serviceProvider, IVsSolution solution, IVsHierarchy pHier, uint selectedItemid)
        {
            Debug.Assert(serviceProvider != null && solution != null && pHier != null);
            using (HierarchyNode parent = new HierarchyNode(solution, pHier))
            {
                return CreateItem(serviceProvider, parent, selectedItemid);
            }
        }

        /// <summary>
        /// Create an IItemContainer wrapper object against pHier
        /// </summary>
        internal static IItemContainer CreateItem(IServiceProvider serviceProvider, IVsSolution solution, IVsHierarchy pHier)
        {
            Debug.Assert(serviceProvider != null && solution != null && pHier != null);
            using (HierarchyNode current = new HierarchyNode(solution, pHier))
            {
                return CreateItem(serviceProvider, current);
            }
        }

        /// <summary>
        /// Private helper to facilitate the previous CreateItem methods.
        /// </summary>
        private static IItemContainer CreateItem(IServiceProvider serviceProvider, HierarchyNode parent, uint selectedItemid)
        {
            using (HierarchyNode current = new HierarchyNode(parent, selectedItemid))
            {
                return ItemFactory.CreateItem(serviceProvider, current);
            }
        }

        /// <summary>
        /// Create an IItemContainer wrapper against IHierarchyNode
        /// </summary>
        internal static IItemContainer CreateItem(IServiceProvider serviceProvider, IHierarchyNode node)
        {
            if (node == null)
            {
                return null;
            }

            var nestedHiearchy = node.GetObject<IVsHierarchy>();
            var extObject = node.ExtObject;

            if (nestedHiearchy != null)
            {
                if (nestedHiearchy.GetType().FullName == "Microsoft.VisualStudio.Project.ProjectNode")
                {
                    return new VsProject(serviceProvider, node);
                }
                else if (nestedHiearchy.GetType().FullName == "Microsoft.VisualStudio.Project.FolderNode")
                {
                    return new VsFolder(serviceProvider, node);
                }
                else if (nestedHiearchy is VSLangProj.References)
                {
                    return new VsProjectReference(serviceProvider, node);
                }
            }

            if (extObject != null)
            {
                if (extObject.GetType().FullName == "Microsoft.VisualStudio.Project.Automation.OAProject")
                {
                    return new VsProject(serviceProvider, node);
                }
                else if (extObject.GetType().FullName == "Microsoft.VisualStudio.Project.Automation.OAFolderItem")
                {
                    return new VsFolder(serviceProvider, node);
                }
                else if (extObject is EnvDTE.Project)
                {
                    if (((EnvDTE.Project)extObject).Object is EnvDTE80.SolutionFolder)
                    {
                        return new VsSolutionFolder(serviceProvider, node);
                    }

                    return new VsProject(serviceProvider, node);
                }
                else if (extObject is EnvDTE.ProjectItem)
                {
                    string kind = null;
                    try
                    {
                        kind = ((EnvDTE.ProjectItem)extObject).Kind;
                    }
                    catch (Exception)
                    {
                        kind = null;
                    }
                    if (kind == EnvDTE.Constants.vsProjectItemKindPhysicalFolder)
                    {
                        return new VsFolder(serviceProvider, node);
                    }
                    else
                    {
                        EnvDTE.ProjectItem x = (EnvDTE.ProjectItem)extObject;
                        if (x.Object is VSLangProj.References)
                        {
                            return new VsProjectReferences(serviceProvider, node);
                        }

                        return new VsItem(serviceProvider, node);
                    }
                }
                else if (extObject is VSLangProj.Reference)
                {
                    return new VsProjectReference(serviceProvider, node);
                }
                else if (extObject is EnvDTE.Solution)
                {
                    return new VsSolution(serviceProvider);
                }

                return new VsUnknownItem(serviceProvider, node);
            }

            return new VsUnknownItem(serviceProvider, node);
        }
    }
}
