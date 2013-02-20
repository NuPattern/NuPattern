using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using EnvDTE;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using NuPattern.Authoring.PatternToolkit.Automation.Properties;
using NuPattern.Authoring.PatternToolkit.Automation.UriProviders;
using NuPattern.Extensibility;
using NuPattern.Extensibility.References;
using NuPattern.Runtime.Schema;

namespace NuPattern.Authoring.PatternToolkit.Automation.Commands
{
    /// <summary>
    /// Command to delete a view
    /// </summary>
    [CLSCompliant(false)]
    [DisplayNameResource("DeleteViewCommand_DisplayName", typeof(Resources))]
    [CategoryResource("AutomationCategory_PatternToolkitAuthoring", typeof(Resources))]
    [DescriptionResource("DeleteViewCommand_Description", typeof(Resources))]
    public class DeleteViewCommand : FeatureCommand
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<DeleteViewCommand>();

        /// <summary>
        /// Gets or sets the service that resolves templates.
        /// </summary>
        [Required]
        [Import(AllowDefault = true)]
        public virtual IFxrUriReferenceService UriService { get; set; }

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
                    DesignerCommandHelper.DoActionOnDesigner(
                        reference.PhysicalPath,
                        docdata =>
                        {
                            var viewReference = ViewArtifactLinkReference.GetReferences(this.CurrentElement.AsElement()).FirstOrDefault();

                            if (viewReference != null)
                            {
                                var viewSchema = docdata.Store.GetViews().FirstOrDefault(v => v.Id == new Guid(viewReference.Host));

                                if (viewSchema != null)
                                {
                                    if (viewSchema.IsDefault)
                                    {
                                        SetOtherViewAsDefault(docdata.Store.GetViews(), viewSchema);
                                    }

                                    tracer.TraceInformation(
                                        Resources.DeleteViewCommand_TraceDeleteingView, patternModel.InstanceName, viewSchema.DisplayName);

                                    DeleteView(reference, viewSchema);
                                }
                                else
                                {
                                    tracer.TraceWarning(
                                        Resources.DeleteViewCommand_TraceViewNotFound, patternModel.InstanceName, viewReference.Host);
                                }
                            }
                            else
                            {
                                tracer.TraceWarning(
                                    Resources.DeleteViewCommand_TraceReferenceNotFound, patternModel.InstanceName);
                            }
                        });
                }
            }
            else
            {
                tracer.TraceWarning(
                    Resources.DeleteViewCommand_TraceReferenceNotFound, patternModel.InstanceName);
            }
        }

        private static void SetOtherViewAsDefault(IEnumerable<ViewSchema> views, ViewSchema viewSchema)
        {
            var otherViewSchema = views.OrderBy(v => v.Name).FirstOrDefault(v => v.Id != viewSchema.Id);

            if (otherViewSchema != null)
            {
                tracer.TraceInformation(
                    Resources.DeleteViewCommand_TraceSetOtherViewDefault, otherViewSchema.DisplayName);

                otherViewSchema.IsDefault = true;
            }
        }

        private void DeleteView(IItemContainer parentItem, ViewSchema viewSchema)
        {
            viewSchema.Store.TransactionManager.DoWithinTransaction(() => viewSchema.Delete());

            this.CurrentElement.Delete();

            var path = GetDiagramFileName(parentItem, viewSchema);
            var childItem = parentItem.Items.FirstOrDefault(i => i.PhysicalPath == path);

            if (childItem != null)
            {
                childItem.As<ProjectItem>().Remove();
            }
        }

        private static string GetDiagramFileName(IItemContainer parentItem, ViewSchema viewSchema)
        {
            return System.IO.Path.Combine(
                System.IO.Path.GetDirectoryName(parentItem.PhysicalPath),
                string.Concat(viewSchema.DiagramId, DesignerConstants.ModelExtension, DesignerConstants.DiagramFileExtension));
        }
    }
}