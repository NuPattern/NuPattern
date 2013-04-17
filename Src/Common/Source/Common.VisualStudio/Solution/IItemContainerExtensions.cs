using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NuPattern.VisualStudio.Solution
{
    /// <summary>
    /// Provides usability helpers for <see cref="IItemContainer"/>.
    /// </summary>
    public static class IItemContainerExtensions
    {
        /// <summary>
        /// Adds a new element to the container by unfolding the given template and using the 
        /// specified name for the new element.
        /// </summary>
        /// <param name="parent">Parent container into which the template will be unfolded.</param>
        /// <param name="name">Name to use when unfolding the template.</param>
        /// <param name="template">Template to unfold.</param>
        /// <returns>The created element.</returns>
        public static IItemContainer Add(this IItemContainer parent, string name, ITemplate template)
        {
            Guard.NotNull(() => parent, parent);
            Guard.NotNull(() => template, template);
            Guard.NotNull(() => name, name);

            return template.Unfold(name, parent);
        }

        /// <summary>
        /// Gets the containing project for the given item (i.e. <see cref="IItem"/>, 
        /// <see cref="IItem"/>, etc.) or container (i.e. <see cref="IFolder"/>).
        /// </summary>
        /// <param name="itemOrContainer">The item or container to start traversing up the hierarchy.</param>
        /// <returns>The project or <see langword="null"/> if the item or container does not belong to a project (i.e. a solution item).</returns>
        public static IProject GetContainingProject(this IItem itemOrContainer)
        {
            return GetContainingProjectImpl(itemOrContainer);
        }

        /// <summary>
        /// Gets the containing project for the given item (i.e. <see cref="IFolder"/>, 
        /// <see cref="IItem"/>, etc.) or container (i.e. <see cref="IFolder"/>).
        /// </summary>
        /// <param name="itemOrContainer">The item or container to start traversing up the hierarchy.</param>
        /// <returns>The project or <see langword="null"/> if the item or container does not belong to a project (i.e. a solution item).</returns>
        public static IProject GetContainingProject(this IFolder itemOrContainer)
        {
            return GetContainingProjectImpl(itemOrContainer);
        }

        private static IProject GetContainingProjectImpl(IItemContainer itemOrContainer)
        {
            return (IProject)itemOrContainer.Traverse(i => i.Parent, i => i is IProject || i == null);
        }

        /// <summary>
        /// Gets the relative name of the <see cref="IItemContainer"/>.
        /// </summary>
        /// <param name="item">The item to get the relative name.</param>
        /// <returns>The relative name of the item.</returns>
        public static string GetLogicalPath(this IItemContainer item)
        {
            Guard.NotNull(() => item, item);

            var logicalPath = new StringBuilder(256);
            while (item.Parent != null)
            {
                logicalPath.Insert(0, item.Name).Insert(0, @"\");
                item = item.Parent;
            }

            return logicalPath.Length == 0 ? string.Empty : logicalPath.ToString(1, logicalPath.Length - 1);
        }



        /// <summary>
        /// Gets the selected items that match the given type.
        /// </summary>
        public static IEnumerable<T> GetSelection<T>(this IItemContainer itemContainer) where T : IItemContainer
        {
            return GetSelection(itemContainer).OfType<T>();
        }

        /// <summary>
        /// Gets the selected items of a given item container in the VS hierarchy
        /// </summary>
        public static IEnumerable<IItemContainer> GetSelection(this IItemContainer itemContainer)
        {
            Guard.NotNull(() => itemContainer, itemContainer);

            var solution = itemContainer.GetSolution();
            var dteSolution = solution.As<EnvDTE.Solution>();

            var selectedItems = dteSolution.DTE.SelectedItems.Cast<EnvDTE.SelectedItem>();
            var allHierarchyItems = itemContainer.Traverse().Concat(new[] { itemContainer }).OfType<HierarchyItem>();

            // return allHierarchyItems.Where(item => selectedItems.Any(selected => GetSelectedItemObject(selected) == item.ExtenderObject));
            return selectedItems.Select(selected => allHierarchyItems.FirstOrDefault(item => item.ExtenderObject == GetSelectedItemObject(selected))).Where(e => e != null);
        }


        /// <summary>
        /// static helper to retrieve the underlying item form a EnvDTE.SelectedItem
        /// </summary>
        /// <param name="selected"></param>
        /// <returns></returns>
        private static object GetSelectedItemObject(EnvDTE.SelectedItem selected)
        {
            if (selected.ProjectItem != null)
            {
                return selected.ProjectItem;
            }
            else if (selected.Project != null)
            {
                ////if (selected.Project.Object != null)
                //if (selected.Project.Kind == EnvDTE80.ProjectKinds.vsProjectKindSolutionFolder)
                //{
                //    return selected.Project; //.Object;
                //}

                return selected.Project;
            }
            return selected.Collection.Cast<object>().FirstOrDefault();
        }

        /// <summary>
        /// Selects the solution items
        /// </summary>
        /// <param name="solution"></param>
        /// <param name="items"></param>
        public static void SetSelection(this ISolution solution, IEnumerable<IItemContainer> items)
        {
            Guard.NotNull(() => solution, solution);

            var vsSolution = solution as VsSolution;
            if (vsSolution != null)
                vsSolution.SelectItems(items);
        }

        /// <summary>
        /// Gets the solution for the given item (i.e. <see cref="IItemContainer"/>, 
        /// <see cref="IItem"/>, etc.) or container (i.e. <see cref="IFolder"/>, <see cref="ISolutionFolder"/>).
        /// </summary>
        /// <param name="itemOrContainer">The item or container to start traversing up the hierarchy.</param>
        public static ISolution GetSolution(this IItemContainer itemOrContainer)
        {
            return (ISolution)itemOrContainer.Traverse(i => i.Parent, i => i is ISolution || i == null);
        }

        /// <summary>
        /// Finds items in the container (recursively) that are of the given type <typeparamref name="T"/>
        /// </summary>
        public static IEnumerable<T> Find<T>(this IItemContainer parent)
                where T : IItemContainer
        {
            return Find<T>(parent, i => true);
        }

        /// <summary>
        /// Finds items in the parent (recursively) that match the 
        /// given condition and are of the given type <typeparamref name="T"/>
        /// </summary>
        /// <param name="parent">The parent to traverse.</param>
        /// <param name="condition">The condition to check against each traversed item.</param>
        /// <returns></returns>
        public static IEnumerable<T> Find<T>(this IItemContainer parent, Func<T, bool> condition)
                where T : IItemContainer
        {
            return parent.Items.Traverse(i => i.Items)
                    .OfType<T>()
                    .Where(condition);
        }

        /// <summary>
        /// Finds items in the parent (recursively) that match the 
        /// given condition.
        /// </summary>
        /// <param name="parent">The parent to traverse.</param>
        /// <param name="condition">The condition to check against each traversed item.</param>
        /// <returns></returns>
        public static IEnumerable<IItemContainer> Find(this IItemContainer parent, Func<IItemContainer, bool> condition)
        {
            return Find<IItemContainer>(parent, condition);
        }

        /// <summary>
        /// Finds elements that match the given regular expression <paramref name="pathExpression"/>.
        /// </summary>
        /// <param name="parent">The parent to traverse.</param>
        /// <param name="pathExpression">A path-like expression that is matched against the items as a path (i.e. Project1\Item.cs), which supports wildcards.</param>
        public static IEnumerable<IItemContainer> Find(this IItemContainer parent, string pathExpression)
        {
            return Find<IItemContainer>(parent, pathExpression);
        }

        /// <summary>
        /// Finds elements of the given type <typeparamref name="T"/> that 
        /// match the given regular expression <paramref name="pathExpression"/>.
        /// </summary>
        /// <typeparam name="T">The type to filter the selected elements to.</typeparam>
        /// <param name="parent">The parent to traverse.</param>
        /// <param name="pathExpression">A regular expression that is matched against the items as a path (i.e. Project1\Item.cs).</param>
        public static IEnumerable<T> Find<T>(this IItemContainer parent, string pathExpression)
                where T : IItemContainer
        {
            var safeRegex = "^" + pathExpression
                    .Replace("\\", "\\\\")
                    .Replace("[", "\\[")
                    .Replace("]", "\\]")
                    .Replace("(", "\\(")
                    .Replace(")", "\\)")
                    .Replace(".", "\\.")
                    .Replace("<", "\\<")
                    .Replace(">", "\\>")
                    .Replace("*", ".+") + "$";

            return FindMatch(parent.Items, string.Empty, new Regex(safeRegex)).OfType<T>();
        }

        /// <summary>
        /// Performs a depth-first traversal of all descendents of the given container (i.e. an 
        /// <see cref="ISolution"/>, <see cref="IFolder"/>, etc.).
        /// </summary>
        /// <param name="container">The container to scope the traversal operation.</param>
        /// <returns>All descendents of the given container.</returns>
        public static IEnumerable<IItemContainer> Traverse(this IItemContainer container)
        {
            return container.Items.Traverse<IItemContainer>(c => c.Items);
        }

        private static IEnumerable<IItemContainer> FindMatch(this IEnumerable<IItemContainer> items, string currentPath, Regex matchPath)
        {
            foreach (var item in items)
            {
                var path = Path.Combine(currentPath, item.Name);

                if (matchPath.IsMatch(path))
                    yield return item;

                foreach (var subItem in FindMatch(item.Items, path, matchPath))
                {
                    yield return subItem;
                }
            }
        }
    }
}
