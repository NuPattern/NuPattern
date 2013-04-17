using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TemplateWizard;
using NuPattern.Diagnostics;
using NuPattern.VisualStudio.Properties;
using NuPattern.VisualStudio.Solution;

namespace NuPattern.VisualStudio.TemplateWizards
{
    /// <summary>
    /// Custom template wizard extension that opens the designer file.
    /// </summary>
    [CLSCompliant(false)]
    public class OpenDesignerTemplateWizard : TemplateWizard
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<OpenDesignerTemplateWizard>();

        private ISolution solution;
        private string projectName;
        private string designerFileName;

        /// <summary>
        /// Runs custom wizard logic when a project has finished generating.
        /// </summary>
        /// <param name="project">The project that finished generating.</param>
        public override void ProjectFinishedGenerating(Project project)
        {
            base.ProjectFinishedGenerating(project);

            this.projectName = project.FullName;
        }

        /// <summary>
        /// Runs custom wizard logic when the wizard has completed all tasks.
        /// </summary>
        public override void RunFinished()
        {
            base.RunFinished();

            if (!string.IsNullOrEmpty(this.designerFileName))
            {
                System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    var project = this.solution.Items.Where(i => i.PhysicalPath.Equals(this.projectName)).FirstOrDefault();

                    //The project is un-available because is being reloaded
                    //Lets wait until it becomes available
                    while (!IsProjectAvailable(project.As<Project>()))
                    {
                        System.Threading.Thread.Sleep(100);
                    }
                }).ContinueWith(task =>
                {
                    var project = this.solution.Items.Where(i => i.PhysicalPath.Equals(this.projectName)).FirstOrDefault();

                    var item = project.Items.Where(i => i.Name.Equals(this.designerFileName)).FirstOrDefault();

                    if (item != null)
                    {
                        tracer.TraceVerbose(Resources.OpenDesignerTemplateWizard_OpeningDesigner, this.designerFileName);

                        var window = item.As<ProjectItem>().Open();
                        window.Visible = true;
                        window.Activate();
                    }
                    else
                    {
                        tracer.TraceWarning(Resources.OpenDesignerTemplateWizard_DesignerItemNotFound, this.designerFileName);
                    }
                });
            }
        }

        /// <summary>
        /// Runs custom wizard logic at the beginning of a template wizard run.
        /// </summary>
        /// <param name="automationObject">The automation object being used by the template wizard.</param>
        /// <param name="replacementsDictionary">The list of standard parameters to be replaced.</param>
        /// <param name="runKind">A <see cref="T:Microsoft.VisualStudio.TemplateWizard.WizardRunKind"/> indicating the type of wizard run.</param>
        /// <param name="customParams">The custom parameters with which to perform parameter replacement in the project.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Never dispose the service provider.")]
        public override void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
        {
            base.RunStarted(automationObject, replacementsDictionary, runKind, customParams);

            var dte = (automationObject as DTE);

            if (!replacementsDictionary.TryGetValue("$patterndefinition$", out this.designerFileName))
                tracer.TraceWarning(Resources.OpenDesignerTemplateWizard_DesignerItemNotFound);

            using (var serviceProvider = new ServiceProvider((Microsoft.VisualStudio.OLE.Interop.IServiceProvider)dte))
            {
                this.solution = serviceProvider.GetService<ISolution>();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "name")]
        private static bool IsProjectAvailable(Project project)
        {
            try
            {
                var name = project.FullName;

                return true;
            }
            catch (COMException)
            {
                return false;
            }
        }
    }
}