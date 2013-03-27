using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Xml;
using EnvDTE;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using Microsoft.VisualStudio.TemplateWizard;
using NuPattern.VisualStudio.Properties;

namespace NuPattern.VisualStudio.TemplateWizards
{
    /// <summary>
    /// A template wizard that coordinates the execution of other wizards.
    /// </summary>
    [CLSCompliant(false)]
    public class CoordinatorTemplateWizard : TemplateWizard
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<CoordinatorTemplateWizard>();

        /// <summary>
        /// The expected element name under <c>WizardData</c> element in the .vstemplate 
        /// containing the wizard extensions to coordinate.
        /// </summary>
        public const string WizardDataElement = "CoordinatedWizards";

        private List<IWizard> wizards = new List<IWizard>();

        /// <summary>
        /// Executes when the wizard starts.
        /// </summary>
        public override void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, Microsoft.VisualStudio.TemplateWizard.WizardRunKind runKind, object[] customParams)
        {
            base.RunStarted(automationObject, replacementsDictionary, runKind, customParams);

            var wizardData = this.TemplateSchema.WizardData.FirstOrDefault();
            if (wizardData != null)
            {
                LoadWizards(wizardData);

                using (var services = new ServiceProvider((Microsoft.VisualStudio.OLE.Interop.IServiceProvider)automationObject))
                {
                    var components = services.GetService<SComponentModel, IComponentModel>();

                    foreach (var wizard in wizards)
                    {
                        TryOrDispose(() => AttributedModelServices.SatisfyImportsOnce(components.DefaultCompositionService, wizard));
                    }
                }
            }

            foreach (var wizard in wizards)
            {
                TryOrDispose(() => wizard.RunStarted(automationObject, replacementsDictionary, runKind, customParams));
            }
        }

        /// <summary>
        /// Executes when the wizard ends.
        /// </summary>
        public override void RunFinished()
        {
            foreach (var wizard in wizards.AsReadOnly().Reverse())
            {
                TryOrDispose(() => wizard.RunFinished());
            }

            foreach (var wizard in wizards.OfType<IDisposable>().Reverse())
            {
                wizard.Dispose();
            }

            base.RunFinished();
        }

        /// <summary>
        /// Executed before a file is opened.
        /// </summary>
        public override void BeforeOpeningFile(ProjectItem projectItem)
        {
            base.BeforeOpeningFile(projectItem);

            foreach (var wizard in wizards)
            {
                TryOrDispose(() => wizard.BeforeOpeningFile(projectItem));
            }
        }

        /// <summary>
        /// Runs custom wizard logic when a project has finished generating.
        /// </summary>
        /// <param name="project">The project that finished generating.</param>
        public override void ProjectFinishedGenerating(Project project)
        {
            base.ProjectFinishedGenerating(project);

            foreach (var wizard in wizards)
            {
                TryOrDispose(() => wizard.ProjectFinishedGenerating(project));
            }
        }

        /// <summary>
        /// Runs custom wizard logic when a project item has finished generating.
        /// </summary>
        /// <param name="projectItem">The project item that finished generating.</param>
        public override void ProjectItemFinishedGenerating(ProjectItem projectItem)
        {
            base.ProjectItemFinishedGenerating(projectItem);

            foreach (var wizard in wizards)
            {
                TryOrDispose(() => wizard.ProjectItemFinishedGenerating(projectItem));
            }
        }

        /// <summary>
        /// Indicates whether the specified project item should be added to the project.
        /// </summary>
        /// <param name="filePath">The path to the project item.</param>
        /// <returns>
        /// true if the project item should be added to the project; otherwise, false.
        /// </returns>
        public override bool ShouldAddProjectItem(string filePath)
        {
            return wizards.All(w => w.ShouldAddProjectItem(filePath));
        }

        private void TryOrDispose(Action action)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                foreach (var disposable in this.wizards.OfType<IDisposable>())
                {
                    try
                    {
                        disposable.Dispose();
                    }
                    catch (Exception de)
                    {
                        tracer.TraceError(de, Resources.CoordinatorTemplateWizard_FailedToDispose, disposable.GetType().FullName);
                    }
                }

                tracer.TraceError(e, Resources.CoordinatorTemplateWizard_TryFailed);
                throw;
            }
        }

        private void LoadWizards(Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.IVsTemplateWizardData wizardData)
        {
            var wizardElements = from coordinator in wizardData.Elements
                                 where coordinator.Name.Equals("CoordinatedWizards", StringComparison.OrdinalIgnoreCase)
                                 from wizardNode in coordinator.ChildNodes.OfType<XmlElement>()
                                 where wizardNode.LocalName == "WizardExtension"
                                 select new
                                 {
                                     Assembly = wizardNode.ChildNodes
                                        .OfType<XmlElement>()
                                        .Where(node => node.LocalName == "Assembly")
                                        .Select(node => node.InnerText)
                                        .FirstOrDefault(),
                                     TypeName = wizardNode.ChildNodes
                                        .OfType<XmlElement>()
                                        .Where(node => node.LocalName == "FullClassName")
                                        .Select(node => node.InnerText)
                                        .FirstOrDefault(),
                                 };

            foreach (var wizardElement in wizardElements.Where(element => !string.IsNullOrEmpty(element.Assembly) && !string.IsNullOrEmpty(element.TypeName)))
            {
                tracer.TraceVerbose(Resources.CoordinatorTemplateWizard_LoadingWizardType, wizardElement.TypeName, wizardElement.Assembly);

                try
                {
                    var asm = Assembly.Load(wizardElement.Assembly);

                    try
                    {
                        var type = asm.GetType(wizardElement.TypeName, true);
                        if (!typeof(IWizard).IsAssignableFrom(type))
                            tracer.TraceError((string)Resources.CoordinatorTemplateWizard_NotIWizard, type.FullName);
                        else
                            this.wizards.Add((IWizard)Activator.CreateInstance(type));
                    }
                    catch (Exception te)
                    {
                        tracer.TraceError(te, Resources.CoordinatorTemplateWizard_FailedToLoadType, wizardElement.TypeName, wizardElement.Assembly);
                    }
                }
                catch (Exception ae)
                {
                    tracer.TraceError(ae, Resources.CoordinatorTemplateWizard_FailedToLoadAssembly, wizardElement.Assembly);
                }
            }
        }
    }
}
