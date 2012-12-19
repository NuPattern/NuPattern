using System;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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
    /// Command to sync the name of the view in solution builder and in the pattern toolkit
    /// </summary>
    [CLSCompliant(false)]
    [DisplayNameResource("SynchViewNameCommand_DisplayName", typeof(Resources))]
    [CategoryResource("AutomationCategory_PatternToolkitAuthoring", typeof(Resources))]
    [DescriptionResource("SynchViewNameCommand_Description", typeof(Resources))]
    public class SynchViewNameCommand : FeatureCommand
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<SynchViewNameCommand>();

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
        /// Sync the names
        /// </summary>
        public override void Execute()
        {
            this.ValidateObject();

            var patternModel = this.CurrentElement.Parent.Parent.AsElement();

            tracer.TraceInformation(
                Resources.SyncViewNameCommand_TraceInitial, patternModel.InstanceName, this.CurrentElement.InstanceName);

            var reference = SolutionArtifactLinkReference.GetResolvedReferences(patternModel, this.UriService).FirstOrDefault();

            if (reference != null)
            {
                using (tracer.StartActivity(Resources.SynchViewNameCommand_TraceSynchronizingViewName, patternModel.InstanceName, this.CurrentElement.InstanceName))
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
                                    viewSchema.Name = this.CurrentElement.InstanceName;
                                }
                                else
                                {
                                    tracer.TraceWarning(
                                        Resources.SyncViewNameCommand_TraceViewNotFound, patternModel.InstanceName, viewReference.Host);
                                }
                            }
                            else
                            {
                                tracer.TraceWarning(
                                    Resources.SyncViewNameCommand_TraceReferenceNotFound, patternModel.InstanceName);
                            }
                        });
                }
            }
            else
            {
                tracer.TraceWarning(
                    Resources.SyncViewNameCommand_TraceReferenceNotFound, patternModel.InstanceName);
            }
        }
    }
}