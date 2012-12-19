using System;
using System.Linq;

namespace NuPattern.Extensibility
{
    /// <summary>
    /// Extensions for UIHierarchy.
    /// </summary>
    public static class UIHierarchyExtensions
    {
        /// <summary>
        /// Collapses the item and all its descendants.
        /// </summary>
        [CLSCompliant(false)]
        public static void Collapse(this EnvDTE.UIHierarchyItem item, CollapseOptions options = CollapseOptions.All)
        {
            Guard.NotNull(() => item, item);

            // Collapse all descendant items first
            item.UIHierarchyItems.Cast<EnvDTE.UIHierarchyItem>()
                .ForEach(child =>
                {
                    child.Collapse(options);
                });

            // Collapse this item
            if (ShouldExpand(item, options))
            {
                if (item.UIHierarchyItems.Expanded)
                {
                    item.UIHierarchyItems.Expanded = false;

                    //HACK: Known bug in Visual Studio 2005
                    //http://connect.microsoft.com/VisualStudio/feedback/ViewFeedback.aspx?FeedbackID=114597
                    if (item.UIHierarchyItems.Expanded)
                    {
                        item.Select(EnvDTE.vsUISelectionType.vsUISelectionTypeSelect);
                        item.DTE.GetHierarchy().DoDefaultAction();
                    }
                }
            }
        }

        private static bool ShouldExpand(EnvDTE.UIHierarchyItem item, CollapseOptions options)
        {
            var solution = item.Object as EnvDTE.Solution;
            if (solution != null)
            {
                // Solution
                return false;
            }

            var project = item.Object as EnvDTE.Project;
            if (project != null)
            {
                if (((EnvDTE.Project)item.Object).Kind != EnvDTE80.ProjectKinds.vsProjectKindSolutionFolder)
                {
                    // Project
                    return (options & CollapseOptions.IncludeProjects) == CollapseOptions.IncludeProjects;
                }
                else
                {
                    // Solution Folder
                    return (options & CollapseOptions.IncludeSolutionFolders) == CollapseOptions.IncludeSolutionFolders;
                }
            }

            //var projectItem = item.Object as EnvDTE.ProjectItem;
            //if ((((projectItem != null) && (((EnvDTE.ProjectItem)item.Object).Object is EnvDTE.Project))
            //        && (((EnvDTE.Project)((EnvDTE.ProjectItem)item.Object).Object).Kind != EnvDTE80.ProjectKinds.vsProjectKindSolutionFolder)))
            //{
            //    // Project item
            //    return true;
            //}

            // Project folders or items
            return true;
        }
    }
}
