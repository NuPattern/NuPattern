using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EnvDTE;
using NuPattern.VisualStudio.Properties;
using NuPattern.VisualStudio.Solution.Hierarchy;

namespace NuPattern.VisualStudio.Solution
{
    internal partial class VsProject : HierarchyItem, IProject
    {
        /// <summary>
        /// Equals the string "|".
        /// </summary>
        private const string BuildConfigurationSeparator = @"|";

        // For testing
        internal VsProject() { }

        public VsProject(IServiceProvider serviceProvider, IHierarchyNode node)
            : base(serviceProvider, node)
        {
            this.Data = new AllConfigurationsProperties(this, userData: false);
            this.UserData = new AllConfigurationsProperties(this, userData: true);
        }

        public dynamic Data { get; private set; }
        public dynamic UserData { get; private set; }

        public override ItemKind Kind
        {
            get { return ItemKind.Project; }
        }

        public override string Id
        {
            get { return this.HierarchyNode.ProjectGuid.ToString(); }
        }

        public override string PhysicalPath
        {
            get
            {
                var dteProject = this.ExtenderObject as EnvDTE.Project;
                return dteProject.FullName;
            }
        }

        public IFolder CreateFolder(string name)
        {
            if (this.Items.Where(i => i.Name == name).FirstOrDefault() != null)
                throw new ArgumentException(String.Format(
                    CultureInfo.CurrentCulture, Resources.VsItem_DuplicateItemName, this.Name, name));

            var project = (Project)base.ExtenderObject;

            project.ProjectItems.AddFolder(name, EnvDTE.Constants.vsProjectItemKindPhysicalFolder);

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

        private static object GetPropertyValue(Property prop)
        {
            try
            {
                return prop.Value;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
