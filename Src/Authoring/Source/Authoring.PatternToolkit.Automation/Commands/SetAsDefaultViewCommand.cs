using System;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using NuPattern.Authoring.PatternToolkit;
using NuPattern.Authoring.PatternToolkit.Automation.Properties;
using NuPattern.Authoring.PatternToolkit.Automation.UriProviders;
using NuPattern.Extensibility;
using NuPattern.Extensibility.References;
using NuPattern.Runtime.Schema;

namespace NuPattern.Authoring.PatternToolkit.Automation.Commands
{
    /// <summary>
    /// Command to set a view as the default view
    /// </summary>
    [CLSCompliant(false)]
    [DisplayNameResource("SetAsDefaultViewCommand_DisplayName", typeof(Resources))]
    [CategoryResource("AutomationCategory_PatternToolkitAuthoring", typeof(Resources))]
    [DescriptionResource("SetAsDefaultViewCommand_Description", typeof(Resources))]
    public class SetAsDefaultViewCommand : FeatureCommand
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<SetAsDefaultViewCommand>();

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
        /// Set the view as default
        /// </summary>
        public override void Execute()
        {
            this.ValidateObject();

            var patternModel = this.CurrentElement.Parent.Parent.AsElement();

            tracer.TraceInformation(
                Resources.SetAsDefaultViewCommand_TraceInitial, patternModel.InstanceName, this.CurrentElement.InstanceName);

            var reference = SolutionArtifactLinkReference.GetResolvedReferences(patternModel, this.UriService).FirstOrDefault();

            if (reference != null)
            {
                using (tracer.StartActivity(Resources.SetAsDefaultViewCommand_TraceSettingAsDefault, patternModel.InstanceName, this.CurrentElement.InstanceName))
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
                                    viewSchema.IsDefault = true;
                                }
                                else
                                {
                                    tracer.TraceWarning(
                                        Resources.SetAsDefaultViewCommand_TraceViewNotFound, patternModel.InstanceName, viewReference.Host);
                                }
                            }
                            else
                            {
                                tracer.TraceWarning(
                                    Resources.SetAsDefaultViewCommand_TraceReferenceNotFound, patternModel.InstanceName);
                            }
                        });
                }
            }
            else
            {
                tracer.TraceWarning(
                    Resources.SetAsDefaultViewCommand_TraceReferenceNotFound, patternModel.InstanceName);
            }
        }
    }
}