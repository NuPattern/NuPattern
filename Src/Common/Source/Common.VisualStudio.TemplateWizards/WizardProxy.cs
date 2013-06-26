using Microsoft.VisualStudio.TemplateWizard;
using System;
using System.Linq;
using EnvDTE;
using System.Collections.Generic;

namespace NuPattern.VisualStudio.TemplateWizards
{
    /// <summary>
    /// Base template proxy class.
    /// </summary>
    [CLSCompliant(false)]
    public abstract class WizardProxy : IWizard
    {
        private Lazy<IWizard> wizard;

        /// <summary>
        /// Initializes a new instance of the <see cref="WizardProxy"/> class.
        /// </summary>
        /// <param name="wizardFullTypeName">Full type name of the wizard implementation.</param>
        protected WizardProxy(string wizardFullTypeName)
        {
            this.wizard = new Lazy<IWizard>(() => (IWizard)Activator.CreateInstance(
                Type.GetType(wizardFullTypeName, true)));
        }

        /// <summary>
        /// Invoked before opening the given project item.
        /// </summary>
        /// <param name="projectItem">The project item.</param>
        public void BeforeOpeningFile(ProjectItem projectItem)
        {
            this.wizard.Value.BeforeOpeningFile(projectItem);
        }

        /// <summary>
        /// Invoked when the given project has finished generating.
        /// </summary>
        /// <param name="project">The project.</param>
        public void ProjectFinishedGenerating(Project project)
        {
            this.wizard.Value.ProjectFinishedGenerating(project);
        }

        /// <summary>
        /// Invoked when the given item has finished generating.
        /// </summary>
        /// <param name="projectItem">The project item.</param>
        public void ProjectItemFinishedGenerating(ProjectItem projectItem)
        {
            this.wizard.Value.ProjectItemFinishedGenerating(projectItem);
        }

        /// <summary>
        /// Invoked when the wizard has finished.
        /// </summary>
        public void RunFinished()
        {
            this.wizard.Value.RunFinished();
        }

        /// <summary>
        /// Invoked when the wizard has started.
        /// </summary>
        /// <param name="automationObject">The automation object.</param>
        /// <param name="replacementsDictionary">The replacements dictionary.</param>
        /// <param name="runKind">Kind of wizard invocation.</param>
        /// <param name="customParams">The custom params.</param>
        public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
        {
            this.wizard.Value.RunStarted(automationObject, replacementsDictionary, runKind, customParams);
        }

        /// <summary>
        /// Whether the given file should be added as a project item.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        public bool ShouldAddProjectItem(string filePath)
        {
            return this.wizard.Value.ShouldAddProjectItem(filePath);
        }
    }
}