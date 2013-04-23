using System;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NuPattern.Authoring.PatternToolkit.Automation.Properties;
using NuPattern.Authoring.PatternToolkit.Automation.UriProviders;
using NuPattern.ComponentModel.Design;
using NuPattern.Diagnostics;
using NuPattern.Runtime.References;
using NuPattern.Runtime.Schema;

namespace NuPattern.Authoring.PatternToolkit.Automation.Commands
{
    /// <summary>
    /// Command to show views
    /// </summary>
    [CLSCompliant(false)]
    [DisplayNameResource("ShowViewCommand_DisplayName", typeof(Resources))]
    [CategoryResource("AutomationCategory_PatternToolkitAuthoring", typeof(Resources))]
    [DescriptionResource("ShowViewCommand_Description", typeof(Resources))]
    public class ShowViewCommand : NuPattern.Runtime.Command
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<ShowViewCommand>();

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
                    var viewReference = ViewArtifactLinkReference.GetReferences(this.CurrentElement.AsElement()).FirstOrDefault();
                    if (viewReference != null)
                    {
                        ViewSchemaHelper.WithPatternModel(reference.PhysicalPath, pm =>
                        {
                            var view = pm.Pattern.GetView(new Guid(viewReference.Host));
                            if (view != null)
                            {
                                ViewSchemaHelper.SelectViewDiagram(reference.PhysicalPath, view);
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
                    Resources.ShowViewCommand_TraceReferenceNotFound, patternModel.InstanceName);
            }
        }
    }
}