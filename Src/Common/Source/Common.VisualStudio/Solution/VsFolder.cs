using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EnvDTE;
using NuPattern.VisualStudio.Properties;
using NuPattern.VisualStudio.Solution.Hierarchy;

namespace NuPattern.VisualStudio.Solution
{
    internal class VsFolder : HierarchyItem, IFolder
    {
        public VsFolder(IServiceProvider serviceProvider, IHierarchyNode node)
            : base(serviceProvider, node)
        { }

        public override ItemKind Kind
        {
            get { return ItemKind.Folder; }
        }

        public IFolder CreateFolder(string name)
        {
            if (this.Items.Where(i => i.Name == name).FirstOrDefault() != null)
                throw new ArgumentException(String.Format(
                    CultureInfo.CurrentCulture, Resources.VsItem_DuplicateItemName, this.Name, name));

            var item = (ProjectItem)base.ExtenderObject;

            item.ProjectItems.AddFolder(name, EnvDTE.Constants.vsProjectItemKindPhysicalFolder);

            return this.Items
                .OfType<IFolder>()
                .Where(folder => folder.Name == name)
                .Single();
        }

        /// <summary>
        /// implements IFolderContainer.Folders, returns the list of all solution folders in this container
        /// </summary>
        public IEnumerable<IFolder> Folders
        {
            get { return this.Items.OfType<IFolder>(); }
        }
    }
}
