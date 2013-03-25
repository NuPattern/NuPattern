using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using Microsoft.VisualStudio.TemplateWizard;
using NuPattern.Extensibility.Properties;
using NuPattern.VisualStudio;
using NuPattern.VisualStudio.Solution;

namespace NuPattern.Extensibility.TemplateWizards
{
    /// <summary>
    /// Custom template wizard extension that fixes Include paths on linked project items.
    /// </summary>
    [CLSCompliant(false)]
    public class FixLinkPathTemplateWizard : Extensibility.TemplateWizard
    {
        private const string Link = "Link";
        private const string FixedLink = "FixedLink";

        private static readonly ITraceSource tracer = Tracer.GetSourceFor<FixLinkPathTemplateWizard>();

        private DTE dte;

        /// <summary>
        /// Runs custom wizard logic when the wizard has completed all tasks.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        public override void RunFinished()
        {
            base.RunFinished();

            if (this.dte != null)
            {
                using (var serviceProvider = new ServiceProvider((Microsoft.VisualStudio.OLE.Interop.IServiceProvider)this.dte))
                {
                    tracer.ShieldUI(() =>
                    {
                        var projects = dte.Solution.Projects
                            .OfType<Project>()
                            .Where(p => !string.IsNullOrEmpty(p.FullName));

                        foreach (var project in projects)
                        {
                            var fileInfo = new FileInfo(project.FullName);
                            var projectBuilder = Microsoft.Build.Construction.ProjectRootElement.Open(fileInfo.FullName);
                            var projectEvaluated = Microsoft.Build.Evaluation.ProjectCollection.GlobalProjectCollection.LoadedProjects
                                .FirstOrDefault<Microsoft.Build.Evaluation.Project>(p => p.FullPath.Equals(fileInfo.FullName, StringComparison.InvariantCultureIgnoreCase));

                            if (projectBuilder != null && projectEvaluated != null)
                            {
                                var items = projectBuilder.Items.Where(i =>
                                    i.Metadata.Any(m => m.Name == Link) &&
                                    i.Metadata.Any(m => m.Name == FixedLink));

                                if (items.Any())
                                {
                                    // Suspend project file change notifications
                                    var fileChange = serviceProvider.GetService<SVsFileChangeEx, IVsFileChangeEx>();
                                    uint cookie = 0;

                                    if (fileChange != null)
                                    {
                                        Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(fileChange.IgnoreFile(cookie, fileInfo.FullName, 1));
                                    }

                                    foreach (var item in items)
                                    {
                                        // Ensure we have a fixed link property
                                        var fixedLink = item.Metadata.First(m => m.Name == FixedLink);
                                        if (fixedLink != null
                                            && !String.IsNullOrEmpty(fixedLink.Value))
                                        {
                                            // Ensure we have an Include attribute
                                            if (!string.IsNullOrEmpty(item.Include))
                                            {
                                                item.Include = fixedLink.Value;
                                                fixedLink.Value = string.Empty;

                                                tracer.TraceVerbose(Resources.FixLinkPathTemplateWizard_LinkFixed, item.Label, fixedLink.Value);
                                            }

                                            // TODO: Remove fixed link (cant do this, collection is readonly)
                                            ////item.Metadata.Remove(fixedLink);
                                        }
                                    }

                                    projectBuilder.Save();

                                    // Resume project file change notifications
                                    if (fileChange != null)
                                    {
                                        Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(fileChange.SyncFile(fileInfo.FullName));
                                        Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(fileChange.IgnoreFile(cookie, fileInfo.FullName, 0));

                                        project.ReloadProject();
                                    }
                                }
                            }
                        }
                    },
                    Resources.FixLinkPathTemplateWizard_FailedToFixLinkPaths);
                }
            }
        }

        /// <summary>
        /// Runs custom wizard logic at the beginning of a template wizard run.
        /// </summary>
        /// <param name="automationObject">The automation object being used by the template wizard.</param>
        /// <param name="replacementsDictionary">The list of standard parameters to be replaced.</param>
        /// <param name="runKind">A <see cref="T:Microsoft.VisualStudio.TemplateWizard.WizardRunKind"/> indicating the type of wizard run.</param>
        /// <param name="customParams">The custom parameters with which to perform parameter replacement in the project.</param>
        public override void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
        {
            base.RunStarted(automationObject, replacementsDictionary, runKind, customParams);

            this.dte = (DTE)automationObject;
        }
    }
}