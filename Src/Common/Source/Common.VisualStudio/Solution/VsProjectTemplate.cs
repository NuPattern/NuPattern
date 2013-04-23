using System;
using System.IO;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using NuPattern.VisualStudio.Properties;

namespace NuPattern.VisualStudio.Solution
{
    /// <summary>
    /// A VS Project Template
    /// </summary>
    public class VsProjectTemplate : ITemplate
    {
        private string path;

        /// <summary>
        /// Creates a new instance of the <see cref="VsProjectTemplate"/> class.
        /// </summary>
        /// <param name="path"></param>
        public VsProjectTemplate(string path)
        {
            this.path = path;
        }

        /// <summary>
        /// Unfolds the template
        /// </summary>
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

        /// <summary>
        /// Gets the parameters of the template.
        /// </summary>
        public dynamic Parameters
        {
            get { throw new NotSupportedException(); }
        }
    }
}
