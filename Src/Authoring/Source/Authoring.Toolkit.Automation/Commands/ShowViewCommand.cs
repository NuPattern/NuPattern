using System;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.VisualStudio.Modeling.Shell;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using NuPattern.Authoring.Authoring;
using NuPattern.Authoring.Automation.Properties;
using NuPattern.Authoring.Automation.UriProviders;
using NuPattern.Extensibility;
using NuPattern.Extensibility.References;
using NuPattern.Runtime.Schema;

namespace NuPattern.Authoring.Automation.Commands
{
    /// <summary>
    /// Command to show views
    /// </summary>
    [CLSCompliant(false)]
    [DisplayNameResource("ShowViewCommand_DisplayName", typeof(Resources))]
    [CategoryResource("AutomationCategory_PatternToolkitAuthoring", typeof(Resources))]
    [DescriptionResource("ShowViewCommand_Description", typeof(Resources))]
    public class ShowViewCommand : FeatureCommand
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<ShowViewCommand>();

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
        /// Shows the view
        /// </summary>
        public override void Execute()
        {
            this.ValidateObject();

            var patternModel = this.CurrentElement.Parent.Parent.AsElement();

            tracer.TraceInformation(
                Resources.ShowViewCommand_TraceInitial, patternModel.InstanceName, this.CurrentElement.InstanceName);

            var reference = SolutionArtifactLinkReference.GetResolvedReferences(patternModel, this.UriService).FirstOrDefault();

            if (reference != null)
            {
                using (tracer.StartActivity(Resources.ShowViewCommand_TraceShowingView, patternModel.InstanceName, this.CurrentElement.InstanceName))
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
                                var docview = docdata.DocViews.First() as SingleDiagramDocView;
                                var diagram = docdata.Store.GetDiagramForView(viewSchema);

                                if (docview != null && diagram != null)
                                {
                                    docview.Diagram = diagram;
                                    viewSchema.Pattern.CurrentDiagramId = diagram.Id.ToString();
                                }
                            }
                            else
                            {
                                tracer.TraceWarning(
                                    Resources.ShowViewCommand_TraceViewNotFound, patternModel.InstanceName, viewReference.Host);
                            }
                        }
                        else
                        {
                            tracer.TraceWarning(
                                Resources.ShowViewCommand_TraceReferenceNotFound, patternModel.InstanceName);
                        }
                    },
                    true);
                }
            }
            else
            {
                tracer.TraceWarning(
                    Resources.ShowViewCommand_TraceReferenceNotFound, patternModel.InstanceName);
            }
        }
    }
}