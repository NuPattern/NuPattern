using System;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.VisualStudio.Shell;
using NuPattern.Authoring.PatternToolkit.Automation.Properties;
using NuPattern.Authoring.PatternToolkit.Automation.UriProviders;
using NuPattern.ComponentModel.Design;
using NuPattern.Diagnostics;
using NuPattern.Runtime;
using NuPattern.Runtime.References;
using NuPattern.Runtime.Schema;
using NuPattern.VisualStudio.Solution;

namespace NuPattern.Authoring.PatternToolkit.Automation.Commands
{
    /// <summary>
    /// Command to delete a view
    /// </summary>
    [CLSCompliant(false)]
    [DisplayNameResource("DeleteViewCommand_DisplayName", typeof(Resources))]
    [CategoryResource("AutomationCategory_PatternToolkitAuthoring", typeof(Resources))]
    [DescriptionResource("DeleteViewCommand_Description", typeof(Resources))]
    public class DeleteViewCommand : NuPattern.Runtime.Command
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<DeleteViewCommand>();

        /// <summary>
        /// Gets or sets the service that resolves templates.
        /// </summary>
        [Required]
        [Import(AllowDefault = true)]
        public virtual IUriReferenceService UriService { get; set; }

        /// <summary>
        /// Gets or sets the current element.
        /// </summary>
        [Required]
        [Import(AllowDefault = true)]
        public IViewModel CurrentElement { get; set; }

        /// <summary>
        /// Deletes the view
        /// </summary>
        public override void Execute()
        {
            this.ValidateObject();

            var patternModel = this.CurrentElement.Parent.Parent.AsElement();

            tracer.TraceInformation(
                Resources.DeleteViewCommand_TraceInitial, patternModel.InstanceName, this.CurrentElement.InstanceName);

            var reference = SolutionArtifactLinkReference.GetResolvedReferences(patternModel, this.UriService).FirstOrDefault();

            if (reference != null)
            {
                using (tracer.StartActivity(Resources.DeleteViewCommand_DeletingView, patternModel, this.CurrentElement.InstanceName))
                {
                    var viewReference = ViewArtifactLinkReference.GetReferences(this.CurrentElement.AsElement()).FirstOrDefault();
                    if (viewReference != null)
                    {
                        ViewSchemaHelper.WithPatternModel(reference.PhysicalPath, pm =>
                        {
                            // Delete the view
                            var view = pm.Pattern.GetView(new Guid(viewReference.Host));
                            if (view != null)
                            {
                                tracer.TraceInformation(Resources.DeleteViewCommand_TraceDeletingView, patternModel.InstanceName, reference.Name);

                                if (pm.Pattern.DeleteView(new Guid(viewReference.Host)))
                                {
                                    // Delete the element
                                    this.CurrentElement.Delete();

                                    DeleteView(reference, view);
                                }
                            }
                            else
                            {
                                tracer.TraceWarning(
                                    Resources.SetAsDefaultViewCommand_TraceViewNotFound, patternModel.InstanceName, viewReference.Host);
                            }
                        }, true);
                    }
                    else
                    {
                        tracer.TraceWarning(
                            Resources.SetAsDefaultViewCommand_TraceReferenceNotFound, patternModel.InstanceName);
                    }
                }
            }
            else
            {
                tracer.TraceWarning(
                    Resources.DeleteViewCommand_TraceReferenceNotFound, patternModel.InstanceName);
            }
        }

        private void DeleteView(IItemContainer parentItem, IViewSchema viewSchema)
        {
            var path = GetDiagramFileName(parentItem, viewSchema);
            var childItem = parentItem.Items.FirstOrDefault(i => i.PhysicalPath == path);

            if (childItem != null)
            {
                VsHelper.CheckOut(childItem.PhysicalPath);
                childItem.As<EnvDTE.ProjectItem>().Remove();
            }
        }

        private static string GetDiagramFileName(IItemContainer parentItem, IViewSchema viewSchema)
        {
            return System.IO.Path.Combine(
                System.IO.Path.GetDirectoryName(parentItem.PhysicalPath),
                string.Concat(viewSchema.DiagramId, ViewSchemaHelper.PatternModelFileExtension, ViewSchemaHelper.PatternModelDiagramFileExtension));
        }

    }
}