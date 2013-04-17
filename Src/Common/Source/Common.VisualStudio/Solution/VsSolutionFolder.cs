using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using NuPattern.VisualStudio.Properties;
using NuPattern.VisualStudio.Solution.Hierarchy;

namespace NuPattern.VisualStudio.Solution
{
    internal class VsSolutionFolder : HierarchyItem, ISolutionFolder
    {
        public VsSolutionFolder(IServiceProvider serviceProvider, IHierarchyNode node)
            : base(serviceProvider, node)
        { }

        public override ItemKind Kind
        {
            get { return ItemKind.SolutionFolder; }
        }

        public override string PhysicalPath
        {
            get { return BuildAndEnsureDirectoryStructure(this); }
        }

        public ISolutionFolder CreateSolutionFolder(string name)
        {
            if (this.Items.Any(i => i.Name == name))
                throw new ArgumentException(String.Format(
                    CultureInfo.CurrentCulture, Resources.VsItem_DuplicateItemName, this.Name, name));

            var vsfolder = (SolutionFolder)((Project)base.ExtenderObject).Object;
            vsfolder.AddSolutionFolder(name);

            var solutionFolder = this.Items
                .OfType<ISolutionFolder>()
                .Single(folder => folder.Name == name);

            BuildAndEnsureDirectoryStructure(solutionFolder);

            return solutionFolder;
        }

        public IEnumerable<ISolutionFolder> SolutionFolders
        {
            get { return this.Items.OfType<ISolutionFolder>(); }
        }

        private string BuildAndEnsureDirectoryStructure(ISolutionFolder solutionFolder)
        {
            var parentPath = System.IO.Path.HasExtension(Parent.PhysicalPath) ?
                System.IO.Path.GetDirectoryName(Parent.PhysicalPath) :
                Parent.PhysicalPath;

            var path = System.IO.Path.Combine(parentPath, this.Name);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path;
        }
    }
}
