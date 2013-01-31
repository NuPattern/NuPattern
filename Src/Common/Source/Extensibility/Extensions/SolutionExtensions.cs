using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Design;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.HierarchyNodes;
using NuPattern.Extensibility.Properties;
using NuPattern.Runtime;
using VSLangProj;
using Ole = Microsoft.VisualStudio.OLE.Interop;

namespace NuPattern.Extensibility
{
    /// <summary>
    /// Provides usability methods for working with <see cref="ISolution"/>.
    /// </summary>
    public static class SolutionExtensions
    {
        /// <summary>
        /// Name of the Solutions Items folder
        /// </summary>
        public const string SolutionItemsFolderName = "Solution Items";

        /// <summary>
        /// Selects the items.
        /// </summary>
        /// <param name="solution">The solution.</param>
        /// <param name="items">The items.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "solution"), CLSCompliant(false)]
        public static void SelectItems(this ISolution solution, IEnumerable<IItemContainer> items)
        {
            Select(items.Select(item => item.As<IHierarchyNode>()));
        }

        /// <summary>
        /// Finds an existing container at the given path, or creates it.
        /// </summary>
        /// <param name="parent">The parent to start the find from.</param>
        /// <param name="pathExpression">The path to find or create.</param>
        /// <returns>Typically, a <see cref="IFolder"/> or <see cref="ISolutionFolder"/> matching the expression.</returns>
        [CLSCompliant(false)]
        public static IItemContainer FindOrCreate(this IItemContainer parent, string pathExpression)
        {
            Guard.NotNull(() => parent, parent);

            // If no path, we mean the parent.
            if ((string.IsNullOrEmpty(pathExpression)) ||
                (pathExpression.Equals(Path.DirectorySeparatorChar.ToString(), StringComparison.OrdinalIgnoreCase)))
            {
                return parent;
            }

            var targetContainer = parent.Find(pathExpression).FirstOrDefault();
            if (targetContainer != null)
            {
                return targetContainer;
            }

            var paths = pathExpression.Split(new[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
            var pathSeparator = Path.DirectorySeparatorChar.ToString();

            for (int targetPathIndex = paths.Length - 1; targetPathIndex >= 0; targetPathIndex--)
            {
                var currentPath = string.Join(pathSeparator, paths.Take(targetPathIndex));
                // When we have an empty path, we have reached the topmost "path" which is the received parent itself.
                targetContainer = currentPath.Length > 0 ? parent.Find(currentPath).FirstOrDefault() : parent;

                if (targetContainer != null)
                {
                    if (targetContainer.Kind == ItemKind.Project)
                    {
                        return CreateFolderPath(
                            ((IProject)targetContainer).CreateFolder(paths[targetPathIndex]),
                            string.Join(pathSeparator, paths.Skip(targetPathIndex + 1)));
                    }
                    else if (targetContainer.Kind == ItemKind.Folder)
                    {
                        return CreateFolderPath(
                            (IFolder)targetContainer,
                            string.Join(pathSeparator, paths.Skip(targetPathIndex)));
                    }
                    else if (targetContainer.Kind == ItemKind.Solution)
                    {
                        return CreateFolderPath(
                            ((ISolution)targetContainer).CreateSolutionFolder(paths[targetPathIndex]),
                            string.Join(pathSeparator, paths.Skip(targetPathIndex + 1)));
                    }
                    else if (targetContainer.Kind == ItemKind.SolutionFolder)
                    {
                        return CreateFolderPath(
                            (ISolutionFolder)targetContainer,
                            string.Join(pathSeparator, paths.Skip(targetPathIndex)));
                    }
                    else
                    {
                        throw new NotSupportedException(string.Format(
                            CultureInfo.CurrentCulture,
                            Resources.InvalidFolderPath,
                            targetContainer.Kind,
                            currentPath,
                            pathExpression));
                    }
                }
            }

            if (targetContainer == null)
            {
                throw new NotSupportedException(string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.CouldNotFindOrCreate,
                    pathExpression));
            }

            return targetContainer;
        }

        private static IFolder CreateFolderPath(IFolder rootFolder, string folderPath)
        {
            var paths = folderPath.Split(new[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var path in paths)
            {
                rootFolder = rootFolder.CreateFolder(path);
            }

            return rootFolder;
        }

        private static ISolutionFolder CreateFolderPath(ISolutionFolder rootFolder, string folderPath)
        {
            var paths = folderPath.Split(new[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var path in paths)
            {
                rootFolder = rootFolder.CreateSolutionFolder(path);
            }

            return rootFolder;
        }

        /// <summary>
        /// Gets the available types for the given project, included declared types as well 
        /// as types from referenced assemblies or projects that inherit or implement 
        /// the given interface.
        /// </summary>
        [CLSCompliant(false)]
        public static IEnumerable<Type> GetAvailableTypes(this IProject project, Type baseType)
        {
            var vsProject = project.As<Project>();
            if (vsProject == null)
            {
                throw new ArgumentException(Resources.SolutionExtensions_UnsupportedProject, "project");
            }

            var provider = new ServiceProvider(vsProject.DTE as Microsoft.VisualStudio.OLE.Interop.IServiceProvider);
            var dynamicTypes = provider.GetService<DynamicTypeService>();
            if (dynamicTypes == null)
            {
                throw new InvalidOperationException(string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.SolutionExtensions_MissingService,
                    typeof(DynamicTypeService).FullName));
            }

            var hierarchy = project.As<IVsHierarchy>();
            var typeDiscovery = dynamicTypes.GetTypeDiscoveryService(hierarchy);
            var availableTypes = typeDiscovery.GetTypes(typeof(object), false);
            var targetType = baseType.AssemblyQualifiedName;

            foreach (Type type in availableTypes)
            {
                if (type.Traverse(t => t.BaseType, t => t.AssemblyQualifiedName == targetType) != null)
                {
                    yield return type;
                }

                if (type.GetInterfaces().FirstOrDefault(t => t.AssemblyQualifiedName == targetType) != null)
                {
                    yield return type;
                }
            }
        }

        /// <summary>
        /// Creates a folder structure.
        /// </summary>
        /// <param name="project">The project to add the folder structure.</param>
        /// <param name="folderPath">The folder path to create.</param>
        /// <returns>A reference to the last folder in the tree.</returns>
        [CLSCompliant(false)]
        public static IFolder CreateFolderPath(this IProject project, string folderPath)
        {
            Guard.NotNull(() => project, project);
            Guard.NotNullOrEmpty(() => folderPath, folderPath);

            if (project.Find<IFolder>(folderPath).Any())
            {
                throw new InvalidOperationException(string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.FolderPathAlreadyExists,
                    folderPath));
            }

            return (IFolder)project.FindOrCreate(folderPath);
        }

        /// <summary>
        /// Creates a new solution at the given destination with the given name.
        /// </summary>
        [CLSCompliant(false)]
        public static void CreateInstance(this ISolution solution, string destination, string name)
        {
            Guard.NotNull(() => solution, solution);
            Guard.NotNullOrEmpty(() => destination, destination);
            Guard.NotNullOrEmpty(() => name, name);

            solution.As<Solution>().Create(destination, name);
        }

        /// <summary>
        /// Gets the selected items that match the given type. This method, unlike the built-in <see cref="IItemContainerExtensions.GetSelection"/> 
        /// works over mocked or faked solutions by taking the <see cref="IItemContainer.IsSelected"/> property into account 
        /// when the solution is not a DTE solution.
        /// </summary>
        [CLSCompliant(false)]
        public static IEnumerable<T> GetSelectedItems<T>(this ISolution solution) where T : IItemContainer
        {
            Guard.NotNull(() => solution, solution);

            if (solution.As<Solution>() != null)
                return solution.GetSelection<T>();
            else
                return solution
                .Traverse()
                .Concat(new[] { solution })
                .Where(item => item.IsSelected)
                .OfType<T>();
        }

        /// <summary>
        /// Gets the selected items. This method, unlike the built-in <see cref="IItemContainerExtensions.GetSelection"/> 
        /// works over mocked or faked solutions by taking the <see cref="IItemContainer.IsSelected"/> property into account 
        /// when the solution is not a DTE solution.
        /// </summary>
        [CLSCompliant(false)]
        public static IEnumerable<IItemContainer> GetSelectedItems(this ISolution solution)
        {
            Guard.NotNull(() => solution, solution);

            if (solution.As<Solution>() != null)
                return solution.GetSelection();
            else
                return solution
                .Traverse()
                .Concat(new[] { solution })
                .Where(item => item.IsSelected);
        }

        /// <summary>
        /// Opens the given item in its default view.
        /// </summary>
        [CLSCompliant(false)]
        public static void Open(this IItem item)
        {
            Guard.NotNull(() => item, item);

            var projectItem = item.As<EnvDTE.ProjectItem>();
            if (projectItem != null)
            {
                var window = projectItem.Open(EnvDTE.Constants.vsViewKindPrimary);
                if (window != null)
                {
                    window.Activate();
                }
            }
        }

        /// <summary>
        /// Renames the given item, project or folder.
        /// </summary>
        /// <param name="item">Item to rename</param>
        /// <param name="proposedItemName">The proposed new name for the item.</param>
        /// <param name="ensureUniqueness">Whether to ensure that the new name for the item is unique within the containing parent, and if not find the next nearest unique name.</param>
        /// <param name="uiService">A UI Service to use to prompt the user if the new name requires uniqueness.</param>
        /// <returns>The actual re-named name of the item.</returns>
        [CLSCompliant(false)]
        public static string Rename(this IItemContainer item, string proposedItemName, bool ensureUniqueness = false, IVsUIShell uiService = null)
        {
            Guard.NotNull(() => item, item);
            Guard.NotNullOrEmpty(() => proposedItemName, proposedItemName);

            // Ensure new name is cleaned (i.e. valid solution name)
            var newItemName = proposedItemName;
            if (!DataFormats.IsValidSolutionItemName(proposedItemName))
            {
                newItemName = DataFormats.MakeValidSolutionItemName(proposedItemName);
            }

            // Ensure rename required
            if (newItemName.Equals(item.Name, StringComparison.OrdinalIgnoreCase))
            {
                return newItemName;
            }

            // If Item, then ensure the new name has an extension (borrowed from existing).
            if (item is IItem)
            {
                var proposedExtension = Path.GetExtension(proposedItemName);
                if (string.IsNullOrEmpty(proposedExtension))
                {
                    var existingExtension = Path.GetExtension(item.Name);
                    if (!string.IsNullOrEmpty(existingExtension))
                    {
                        newItemName = Path.ChangeExtension(newItemName, existingExtension);
                    }
                }
            }

            // Check to see if new named item already exists (within parent)
            bool newNamedItemExists =
                (item.Parent.Items.Where(i => i.Name.Equals(newItemName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault() != null);
            if (newNamedItemExists)
            {
                if (ensureUniqueness)
                {
                    // Calculate new unique name
                    var uniqueItemName = string.Empty;
                    uniqueItemName = (item is IItem) ? item.Parent.CalculateNextUniqueChildItemName<IItem>(newItemName) : uniqueItemName;
                    uniqueItemName = (item is IProject) ? item.Parent.CalculateNextUniqueChildItemName<IProject>(newItemName) : uniqueItemName;
                    if (string.IsNullOrEmpty(uniqueItemName))
                    {
                        throw new InvalidOperationException(
                            string.Format(CultureInfo.CurrentCulture, Resources.SolutionExtensions_ErrorRenameUnknownType, item.Kind));
                    }

                    // Prompt user interactively
                    if (newNamedItemExists)
                    {
                        if (uiService != null)
                        {
                            uiService.ShowWarning(Resources.SolutionExtensions_PromptItemAlreadyExists_Title,
                                string.Format(CultureInfo.CurrentCulture,
                                Resources.SolutionExtensions_PromptItemAlreadyExists_Message, newItemName, uniqueItemName));
                        }
                    }

                    newItemName = uniqueItemName;
                }
                else
                {
                    throw new InvalidOperationException(
                        string.Format(CultureInfo.CurrentCulture, Resources.SolutionExtensions_ErrorRenamedItemExists, newItemName));
                }
            }

            // Rename item
            if (item is IItem || item is IFolder)
            {
                var vsItem = item.As<ProjectItem>();
                if (vsItem != null)
                {
                    vsItem.Name = newItemName;
                }
            }

            // Rename project
            var project = item as IProject;
            if (project != null)
            {
                var vsProject = project.As<Project>();
                if (vsProject != null)
                {
                    // Rename project Name
                    vsProject.Name = newItemName;

                    // Rename AssemblyName
                    if (project.Data != null)
                    {
                        if (project.Data.AssemblyName != null)
                        {
                            var assemblyName = project.Data.AssemblyName as string;

                            if (assemblyName.IndexOf(project.Name, StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                project.Data.AssemblyName = assemblyName.Replace(project.Name, newItemName);
                            }
                        }

                        // Rename Rootnamespace
                        if (project.Data.DefaultNamespace != null)
                        {
                            var defaultNamespace = project.Data.DefaultNamespace as string;

                            if (defaultNamespace.IndexOf(project.Name, StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                project.Data.DefaultNamespace = defaultNamespace.Replace(project.Name, newItemName);
                            }
                        }
                    }
                }
            }

            return newItemName;
        }

        /// <summary>
        /// Saves the given item.
        /// </summary>
        [CLSCompliant(false)]
        public static void Save(this IItem item)
        {
            Guard.NotNull(() => item, item);

            item.As<ProjectItem>().Save();
        }

        /// <summary>
        /// Checks out the item, if cource controlled.
        /// </summary>
        /// <param name="item"></param>
        [CLSCompliant(false)]
        public static void Checkout(this IItem item)
        {
            Guard.NotNull(() => item, item);

            var serviceProvider = new ServiceProvider(item.As<ProjectItem>().DTE as Ole.IServiceProvider);

            VsHelper.CheckOut(serviceProvider, item.PhysicalPath);
        }

        /// <summary>
        /// Deletes the given item.
        /// </summary>
        [CLSCompliant(false)]
        public static void Delete(this IItem item)
        {
            Guard.NotNull(() => item, item);

            item.As<ProjectItem>().Delete();
        }

        /// <summary>
        /// Opens the specified solution.
        /// </summary>
        [CLSCompliant(false)]
        public static void Open(this ISolution solution, string solutionFile)
        {
            Guard.NotNull(() => solution, solution);
            Guard.NotNullOrEmpty(() => solutionFile, solutionFile);

            solution.As<Solution>().Open(solutionFile);
        }

        /// <summary>
        /// Closes the solution, optionally saving first.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        [CLSCompliant(false)]
        public static void Close(this ISolution solution, bool saveFirst = true)
        {
            Guard.NotNull(() => solution, solution);

            if (solution.IsOpen)
            {
                solution.As<Solution>().Close(saveFirst);
            }
        }

        /// <summary>
        /// Saves the solution.
        /// </summary>
        [CLSCompliant(false)]
        public static void SaveAs(this ISolution solution, string fileName)
        {
            Guard.NotNull(() => solution, solution);

            var dteSolution = solution.As<Solution>();
            if (dteSolution != null && !dteSolution.Saved)
            {
                dteSolution.SaveAs(fileName);
            }
        }

        /// <summary>
        /// Shows the Solution Explorer window.
        /// </summary>
        /// <param name="solution"></param>
        [CLSCompliant(false)]
        public static void ShowSolutionExplorer(this ISolution solution)
        {
            Guard.NotNull(() => solution, solution);

            var vsSolution = solution.As<Solution>();
            if (vsSolution != null)
            {
                var dte = vsSolution.DTE;
                if (dte != null)
                {
                    using (var provider = new ServiceProvider(dte as Ole.IServiceProvider))
                    {
                        var service = provider.GetService<IVsUIShell>();
                        if (service != null)
                        {
                            var solutionExplorer = StandardToolWindows.ProjectExplorer;
                            IVsWindowFrame frame;
                            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(service.FindToolWindow((uint)__VSFINDTOOLWIN.FTW_fForceCreate, ref solutionExplorer, out frame));
                            if (frame != null)
                            {
                                Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(frame.Show());
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the logical path from this item to the given item.
        /// </summary>
        [CLSCompliant(false)]
        public static string GetLogicalPath(this IItemContainer item, IItemContainer ancestor)
        {
            var itemPath = item.GetLogicalPath();
            var ancestorPath = ancestor.GetLogicalPath();

            //Ensure the ancestor is valid
            if (itemPath.StartsWith(ancestorPath, StringComparison.OrdinalIgnoreCase))
            {
                return itemPath.Substring(ancestorPath.Length + 1);
            }

            // not related to ancestor
            return itemPath;
        }

        /// <summary>
        /// Adds a unique identifier to the given item, if not already exists.
        /// </summary>
        [CLSCompliant(false)]
        public static void AddItemIdentifier(this IItem item)
        {
            Guard.NotNull(() => item, item);

            // Ensure item is in a project (cant set MSBUILD properties on a solution item)
            var project = item.GetContainingProject();
            if (project != null)
            {
                // Prepare item for resilient identifier
                if (String.IsNullOrEmpty(item.Data.ItemGuid))
                {
                    // Add guid (in form: 000000000000-0000-0000-0000-00000000)
                    item.Data.ItemGuid = Guid.NewGuid().ToString("D");
                }
            }
        }

        private static object GetObject(SelectedItem selected)
        {
            if (selected.ProjectItem != null)
            {
                return selected.ProjectItem;
            }
            else if (selected.Project != null)
            {
                if (selected.Project.Object != null)
                {
                    return selected.Project.Object;
                }

                return selected.Project;
            }

            return selected.Collection.Cast<object>().FirstOrDefault();
        }

        /// <summary>
        /// Gets the toolkit manifest of the project where the given element is contained. 
        /// </summary>
        /// <exception cref="ArgumentException">The given element is not contained in a project or 
        /// the containing project does not contain an item marked with the 
        /// <see cref="ExtensibilityConstants.IsToolkitManifestItemMetadata"/> metadata flag.</exception>
        [CLSCompliant(false)]
        public static IItem GetToolkitManifest(this IItemContainer element)
        {
            Guard.NotNull(() => element, element);

            var owningProject = (IProject)element.Traverse(x => x.Parent, item => item.Kind == ItemKind.Project);
            if (owningProject == null)
            {
                throw new ArgumentException(string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.IProjectExtensions_ItemNotInProject,
                    element.GetLogicalPath()));
            }

            return owningProject.GetToolkitManifest();
        }

        /// <summary>
        /// Gets the toolkit manifest of the given project.
        /// </summary>
        /// <exception cref="ArgumentException">The given project does not contain an item marked with the 
        /// <see cref="ExtensibilityConstants.IsToolkitManifestItemMetadata"/> metadata flag.</exception>
        [CLSCompliant(false)]
        public static IItem GetToolkitManifest(this IProject project)
        {
            Guard.NotNull(() => project, project);

            bool isManifest = false;
            var manifest = project.Traverse()
                .OfType<IItem>()
                .FirstOrDefault(item =>
                    item.Data.IsToolkitManifest != null &&
                    Boolean.TryParse(item.Data.IsToolkitManifest, out isManifest) &&
                    isManifest);

            if (manifest == null)
            {
                throw new ArgumentException(string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.IProjectExtensions_NotToolkitProject,
                    project.GetLogicalPath(),
                    ExtensibilityConstants.IsToolkitManifestItemMetadata));
            }

            return manifest;
        }

        /// <summary>
        /// Returns the name of the next non-existing child item of the given parent seeded with given name.
        /// </summary>
        /// <param name="parent">The parent container</param>
        /// <param name="childNameSeed">A name used to seed the name of the child.</param>
        /// <typeparam name="T">The type of the child items.</typeparam>
        /// <returns></returns>
        [CLSCompliant(false)]
        public static string CalculateNextUniqueChildItemName<T>(this IItemContainer parent, string childNameSeed) where T : IItemContainer
        {
            Guard.NotNull(() => parent, parent);
            Guard.NotNull(() => childNameSeed, childNameSeed);

            if ((typeof(T).IsAssignableFrom(typeof(ISolution)))
                || ((parent is IProject || parent is IItem || parent is IFolder) && typeof(T).IsAssignableFrom(typeof(ISolutionFolder)))
                || ((parent is IFolder || parent is IItem) && typeof(T).IsAssignableFrom(typeof(IProject)))
                || ((parent is ISolution || parent is ISolutionFolder || parent is IItem) && typeof(T).IsAssignableFrom(typeof(IFolder))))
            {
                throw new InvalidOperationException();
            }

            var extension = Path.GetExtension(childNameSeed);
            var seed = Path.GetFileNameWithoutExtension(childNameSeed);

            return String.Format(CultureInfo.CurrentCulture, "{0}{1}",
                UniqueNameGenerator.EnsureUnique(seed,
                    newName => parent.Items.Where(item => String.IsNullOrEmpty(extension) ? item.Name.Equals(newName, StringComparison.OrdinalIgnoreCase)
                        : item.Name.Equals(Path.ChangeExtension(newName, extension), StringComparison.OrdinalIgnoreCase)).FirstOrDefault() == null), extension);
        }

        /// <summary>
        /// Runs the given command in the IDE.
        /// </summary>
        [CLSCompliant(false)]
        public static void RunCommand(this ISolution solution, string commandName)
        {
            Guard.NotNull(() => solution, solution);
            Guard.NotNull(() => commandName, commandName);

            var dteSolution = solution.As<EnvDTE.Solution>();
            if (dteSolution != null)
            {
                dteSolution.DTE.ExecuteCommand(commandName);
            }
        }

        /// <summary>
        /// Determines if the given container is the Solution Items folder of the solution.
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        [CLSCompliant(false)]
        public static bool IsSolutionItemsFolder(this IItemContainer container)
        {
            return ((container is ISolutionFolder)
                && (container.Name.Equals(SolutionItemsFolderName, StringComparison.OrdinalIgnoreCase)));
        }

        /// <summary>
        /// Returns all the projects in a solution.
        /// </summary>
        /// <param name="solution"></param>
        /// <returns></returns>
        [CLSCompliant(false)]
        public static IEnumerable<IProject> GetAllProjects(this ISolution solution)
        {
            return solution.Find<IProject>();
        }

        /// <summary>
        /// Collapses all solution items in the solution.
        /// </summary>
        [CLSCompliant(false)]
        public static void CollapseAll(this ISolution solution, CollapseOptions options = CollapseOptions.All)
        {
            Guard.NotNull(() => solution, solution);

            EnvDTE.DTE dte = solution.As<EnvDTE.Solution>().DTE;
            dte.SuppressUI = true;
            try
            {
                var hier = dte.GetHierarchy();
                hier.UIHierarchyItems.Cast<EnvDTE.UIHierarchyItem>()
                            .ForEach(item =>
                            {
                                item.Collapse(options);
                            });
            }
            catch (COMException)
            {
                // Ignore exception
            }
            finally
            {
                dte.SuppressUI = false;
            }
        }

        /// <summary>
        /// Gets the nearest project to the currently selected item in the solution, or the first project in the solution.
        /// </summary>
        /// <remarks>
        /// If a project is currently selected, then that project is returned. If a file is currently opened, then its owning project is returned.
        /// Failing that the fall back is to return the first project in the solution (if any).
        /// </remarks>
        [CLSCompliant(false)]
        public static IProject GetCurrentProjectScope(this ISolution solution)
        {
            Guard.NotNull(() => solution, solution);

            var selection = ServiceProvider.GlobalProvider.GetService<SVsShellMonitorSelection, IVsMonitorSelectionFixed>();
            if (selection != null)
            {
                IVsHierarchy hierarchy;
                uint pitemid;
                IVsMultiItemSelect ppMIS;
                ISelectionContainer ppSC;

                if (selection.GetCurrentSelection(out hierarchy, out pitemid, out ppMIS, out ppSC) == Microsoft.VisualStudio.VSConstants.S_OK)
                {
                    if (hierarchy != null)
                    {
                        var project = hierarchy.ToProject();
                        if (project != null)
                        {
                            return solution.Find<IProject>(p => p.As<EnvDTE.Project>() == project).FirstOrDefault();
                        }
                    }
                }
            }

            // Use first project in the solution
            return solution.Find<IProject>().FirstOrDefault();
        }

        /// <summary>
        /// Gets the assembly references of the project
        /// </summary>
        [CLSCompliant(false)]
        public static IEnumerable<IAssemblyReference> GetAssemblyReferences(this IProject project)
        {
            Guard.NotNull(() => project, project);

            var references = new List<AssemblyReference>();

            var dteProject = project.As<EnvDTE.Project>();
            if (dteProject != null)
            {
                var vslangProject = dteProject.Object as VSProject;
                if (vslangProject != null)
                {
                    vslangProject.References.Cast<VSLangProj.Reference>()
                        .ForEach(r =>
                            {
                                if (!String.IsNullOrEmpty(r.Path))
                                {
                                    references.Add(new AssemblyReference(r.Name, r.Path));
                                }
                            });
                }
            }
            return references;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "Microsoft.VisualStudio.Shell.Interop.IVsUIHierarchyWindow.ExpandItem(Microsoft.VisualStudio.Shell.Interop.IVsUIHierarchy,System.UInt32,Microsoft.VisualStudio.Shell.Interop.EXPANDFLAGS)"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "Microsoft.VisualStudio.Shell.Interop.IVsWindowFrame.Show")]
        private static void Select(IEnumerable<IHierarchyNode> hierarchyNodes)
        {
            IVsWindowFrame frame;
            object obj2;
            var service = ServiceProvider.GlobalProvider.GetService<SVsUIShell, IVsUIShell>();

            if (service != null)
            {
                var rguidPersistenceSlot = new Guid(EnvDTE.Constants.vsWindowKindSolutionExplorer);
                Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(service.FindToolWindow(0x80000, ref rguidPersistenceSlot, out frame));
                Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(frame.GetProperty((int)__VSFPROPID.VSFPROPID_DocView, out obj2));
                bool flag = true;
                var window = obj2 as IVsUIHierarchyWindow;

                if (window != null)
                {
                    foreach (IHierarchyNode node in hierarchyNodes)
                    {
                        window.ExpandItem(node.GetObject<IVsHierarchy>() as IVsUIHierarchy, node.ItemId, flag ? EXPANDFLAGS.EXPF_SelectItem : EXPANDFLAGS.EXPF_AddSelectItem);

                        if (flag)
                        {
                            flag = false;
                        }
                    }
                }

                frame.Show();
            }
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("55AB9450-F9C7-4305-94E8-BEF12065338D")]
        private interface IVsMonitorSelectionFixed
        {
            [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            int GetCurrentSelection([MarshalAs(UnmanagedType.Interface)] out IVsHierarchy ppHier, [ComAliasName("Microsoft.VisualStudio.Shell.Interop.VSITEMID")] out uint pitemid, [MarshalAs(UnmanagedType.Interface)] out IVsMultiItemSelect ppMIS, [MarshalAs(UnmanagedType.Interface)] out ISelectionContainer ppSC);
            [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            int AdviseSelectionEvents([In, MarshalAs(UnmanagedType.Interface)] IVsSelectionEvents pSink, [ComAliasName("Microsoft.VisualStudio.Shell.Interop.VSCOOKIE")] out uint pdwCookie);
            [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            int UnadviseSelectionEvents([In, ComAliasName("Microsoft.VisualStudio.Shell.Interop.VSCOOKIE")] uint dwCookie);
            [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            int GetCurrentElementValue([In, ComAliasName("Microsoft.VisualStudio.Shell.Interop.VSSELELEMID")] uint elementid, [MarshalAs(UnmanagedType.Struct)] out object pvarValue);
            [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            int GetCmdUIContextCookie([In, ComAliasName("Microsoft.VisualStudio.OLE.Interop.REFGUID")] ref Guid rguidCmdUI, [ComAliasName("Microsoft.VisualStudio.Shell.Interop.VSCOOKIE")] out uint pdwCmdUICookie);
            [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            int IsCmdUIContextActive([In, ComAliasName("Microsoft.VisualStudio.Shell.Interop.VSCOOKIE")] uint dwCmdUICookie, [ComAliasName("Microsoft.VisualStudio.OLE.Interop.BOOL")] out int pfActive);
            [PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            int SetCmdUIContext([In, ComAliasName("Microsoft.VisualStudio.Shell.Interop.VSCOOKIE")] uint dwCmdUICookie, [In, ComAliasName("Microsoft.VisualStudio.OLE.Interop.BOOL")] int fActive);
        }
    }
}
