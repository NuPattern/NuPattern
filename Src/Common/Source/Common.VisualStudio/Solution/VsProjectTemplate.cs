using System;
using System.IO;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using NuPattern.VisualStudio.Properties;

namespace NuPattern.VisualStudio.Solution
{
    internal class VsProjectTemplate : ITemplate
    {
        private string path;

        public VsProjectTemplate(string path)
        {
            this.path = path;
        }

        public IItemContainer Unfold(string name, IItemContainer parent)
        {
            if (parent.Kind == ItemKind.Solution)
            {
                var solution = ((VsSolution)parent).ExtenderObject as EnvDTE.Solution;
                var destinationPath = Path.GetDirectoryName(parent.PhysicalPath);
                solution.AddFromTemplate(path, Path.Combine(destinationPath, name), name);
            }
            else if (parent.Kind == ItemKind.SolutionFolder)
            {
                var project = ((VsSolutionFolder)parent).ExtenderObject as Project;
                var solutionFolder = project.Object as SolutionFolder;
                solutionFolder.AddFromTemplate(path, Path.Combine(parent.PhysicalPath, name), name);
            }
            else
            {
                throw new NotSupportedException(Resources.VsProjectTemplate_UnsupportedTarget);
            }

            return parent.Items.FirstOrDefault(item => item.Kind == ItemKind.Project && item.Name == name);
        }

        public dynamic Parameters
        {
            get { throw new NotSupportedException(); }
        }
    }
}
