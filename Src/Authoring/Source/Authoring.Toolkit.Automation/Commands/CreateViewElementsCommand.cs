using System;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using NuPattern.Authoring.Authoring;
using NuPattern.Authoring.Automation.Properties;
using NuPattern.Extensibility;
using NuPattern.Extensibility.References;
using NuPattern.Runtime.Schema;

namespace NuPattern.Authoring.Automation.Commands
{
    /// <summary>
    /// Command to create all views for the current pattern.
    /// </summary>
    [CLSCompliant(false)]
    [DisplayNameResource("CreateViewElementsCommand_DisplayName", typeof(Resources))]
    [CategoryResource("AutomationCategory_PatternToolkitAuthoring", typeof(Resources))]
    [DescriptionResource("CreateViewElementsCommand_Description", typeof(Resources))]
    public class CreateViewElementsCommand : FeatureCommand
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<CreateViewElementsCommand>();

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
        public IPatternModel CurrentElement { get; set; }

        /// <summary>
        /// Executes the command.
        /// </summary>
        public override void Execute()
        {
            this.ValidateObject();

            tracer.TraceInformation(
                Resources.CreateViewElementsCommand_TraceInitial, this.CurrentElement.InstanceName);

            var reference = SolutionArtifactLinkReference.GetResolvedReferences(this.CurrentElement.AsElement(), this.UriService).FirstOrDefault();

            if (reference != null)
            {
                using (tracer.StartActivity(Resources.CreateViewElementsCommand_TraceAddingViews, this.CurrentElement.InstanceName))
                {
                    DesignerCommandHelper.DoActionOnDesigner(
                        reference.PhysicalPath,
                        docdata =>
                        {
                            foreach (var view in docdata.Store.GetViews())
                            {
                                tracer.TraceInformation(
                                    Resources.CreateViewElementsCommand_TraceCreatingView, this.CurrentElement.InstanceName, view.Name);

                                this.CurrentElement.Views.CreateViewModel(view.Name);
                            }
                        });
                }
            }
            else
            {
                tracer.TraceWarning(
                    Resources.CreateViewElementsCommand_TraceReferenceNotFound, this.CurrentElement.InstanceName);
            }
        }
    }
}