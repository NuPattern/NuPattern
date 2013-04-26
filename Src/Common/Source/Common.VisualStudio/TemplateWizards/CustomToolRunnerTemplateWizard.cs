using System;
using System.Linq;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using NuPattern.Diagnostics;
using NuPattern.VisualStudio.Properties;
using NuPattern.VisualStudio.Solution;
using VSLangProj;

namespace NuPattern.VisualStudio.TemplateWizards
{
    /// <summary>
    /// Custom template wizard extension that runs every custom tool associated with a project item.
    /// </summary>
    [CLSCompliant(false)]
    public class CustomToolRunnerTemplateWizard : TemplateWizard
    {
        private static readonly ITracer tracer = Tracer.Get<CustomToolRunnerTemplateWizard>();

        /// <summary>
        /// Runs custom wizard logic when a project has finished generating.
        /// </summary>
        /// <param name="project">The project that finished generating.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "None.")]
        public override void ProjectFinishedGenerating(Project project)
        {
            base.ProjectFinishedGenerating(project);

            var vs = project.DTE;
            if (vs != null)
            {
                using (var serviceProvider = new ServiceProvider((Microsoft.VisualStudio.OLE.Interop.IServiceProvider)vs))
                {
                    TraceSourceExtensions.ShieldUI(tracer, (Action)(() =>
                        {
                            var solution = serviceProvider.GetService<ISolution>();
                            var projectAdded = solution.Find<IProject>(p => p.As<Project>().UniqueName == project.UniqueName).FirstOrDefault();

                            var items = projectAdded.Find<IItem>(item => item.As<ProjectItem>() != null);

                            foreach (var item in items)
                            {
                                if (!string.IsNullOrEmpty(item.Data.CustomTool))
                                {
                                    var runCustomToolOnUnfold = item.Data.RunCustomToolOnUnfold;

                                    if (!string.IsNullOrEmpty(runCustomToolOnUnfold))
                                    {
                                        bool runTool = false;
                                        Boolean.TryParse(runCustomToolOnUnfold, out runTool);

                                        if (!runTool)
                                        {
                                            continue;
                                        }
                                    }

                                    var projectItem = item.As<ProjectItem>().Object as VSProjectItem;

                                    if (projectItem != null)
                                    {
                                        tracer.Info(Resources.CustomToolRunnerTemplateWizard_RunningCustomTool, ((object)item.Data.CustomTool).ToString(), item.GetLogicalPath());
                                        projectItem.RunCustomTool();
                                    }
                                }
                            }
                        }),
                    Resources.CustomToolRunnerTemplateWizard_FailedToRunCustomTools);
                }
            }
        }
    }
}