using System;
using System.IO;
using System.Linq;

namespace NuPattern.VisualStudio.Solution
{
    internal class VsFileTemplate : ITemplate
    {
        public string SourcePath { get; private set; }

        private bool overwrite, openFile;

        public VsFileTemplate(string sourcePath, bool overwrite, bool openFile)
        {
            Guard.NotNullOrEmpty(() => sourcePath, sourcePath);

            if (!File.Exists(sourcePath))
            {
                throw new FileNotFoundException("Source file for template was not found.", sourcePath);
            }

            this.SourcePath = sourcePath;
            this.overwrite = overwrite;
            this.openFile = openFile;
        }

        public virtual IItemContainer Unfold(string name, IItemContainer parent)
        {
            Guard.NotNullOrEmpty(() => name, name);
            Guard.NotNull(() => parent, parent);

            if (!typeof(HierarchyItem).IsAssignableFrom(parent.GetType()))
                throw new NotSupportedException("This method requires a hierarchy item");

            Guard.NotNullOrEmpty(() => parent.PhysicalPath, parent.PhysicalPath);

            var targetPath = Path.Combine(Path.GetDirectoryName(parent.PhysicalPath), name);
            if (File.Exists(targetPath))
            {
                VsHelper.CheckOut(targetPath);
            }

            if (!this.SourcePath.Equals(targetPath, StringComparison.OrdinalIgnoreCase))
            {
                File.Copy(this.SourcePath, targetPath, this.overwrite);
            }

            var container = parent.As<dynamic>();
            EnvDTE.ProjectItem newlyAddedFile = null;

            if (!parent.Items.Any(i => i.Name == name))
            {
                newlyAddedFile = container.ProjectItems.AddFromFile(targetPath) as EnvDTE.ProjectItem;
            }

            if (this.openFile)
            {
                container.DTE.ItemOperations.OpenFile(targetPath);
            }
            else if (newlyAddedFile != null)
            {
                //
                // The file may have opened anyway, if we're not supposed to open it, we'll search for
                // the matching window and close it
                //
                foreach (EnvDTE.Window w in container.DTE.Windows)
                {
                    if (newlyAddedFile.Equals(w.ProjectItem))
                    {
                        w.Close(EnvDTE.vsSaveChanges.vsSaveChangesNo);
                        break;
                    }
                }

            }

            return parent.Items.FirstOrDefault(item => item.Kind == ItemKind.Item && item.Name == name);
        }

        public dynamic Parameters
        {
            get { throw new NotSupportedException(); }
        }
    }
}