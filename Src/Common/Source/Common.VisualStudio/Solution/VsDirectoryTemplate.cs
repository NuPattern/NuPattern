using System;
using System.Linq;

namespace NuPattern.VisualStudio.Solution
{
    /// <summary>
    /// Allows adding an entire folder to a container.
    /// </summary>
    internal class VsDirectoryTemplate : ITemplate
    {
        public VsDirectoryTemplate(string sourcePath)
        {
            this.SourcePath = sourcePath;
        }

        public string SourcePath { get; private set; }

        public IItemContainer Unfold(string name, IItemContainer parent)
        {
            Guard.NotNullOrEmpty(() => name, name);

            var container = parent.As<dynamic>();
            var item = container.ProjectItems.AddFromDirectory(this.SourcePath);

            return parent.Items.FirstOrDefault(i => i.Name == item.Name);
        }

        public dynamic Parameters
        {
            get { throw new NotSupportedException(); }
        }
    }
}
