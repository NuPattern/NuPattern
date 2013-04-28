using System;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NuPattern.Authoring.PatternToolkit.Automation.Properties;
using NuPattern.Authoring.PatternToolkit.Automation.UriProviders;
using NuPattern.ComponentModel.Design;
using NuPattern.Diagnostics;
using NuPattern.Runtime;
using NuPattern.Runtime.References;
using NuPattern.Runtime.Schema;

namespace NuPattern.Authoring.PatternToolkit.Automation.Commands
{
    /// <summary>
    /// Command to sync the name of the view in solution builder and in the pattern toolkit
    /// </summary>
    [CLSCompliant(false)]
    [DisplayNameResource("SynchViewNameCommand_DisplayName", typeof(Resources))]
    [CategoryResource("AutomationCategory_PatternToolkitAuthoring", typeof(Resources))]
    [DescriptionResource("SynchViewNameCommand_Description", typeof(Resources))]
    public class SynchViewNameCommand : NuPattern.Runtime.Command
    {
        private static readonly ITracer tracer = Tracer.Get<SynchViewNameCommand>();

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
        /// Sync the names
        /// </summary>
        public override void Execute()
        {
            this.ValidateObject();

            var patternModel = this.CurrentElement.Parent.Parent.AsElement();

            tracer.Info(
                Resources.SyncViewNameCommand_TraceInitial, patternModel.InstanceName, this.CurrentElement.InstanceName);

            // Ensure the pattern model file exists
            var reference = SolutionArtifactLinkReference.GetResolvedReferences(patternModel, this.UriService).FirstOrDefault();
            if (reference != null)
            {
                using (tracer.StartActivity(Resources.SynchViewNameCommand_TraceSynchronizingViewName, patternModel.InstanceName, this.CurrentElement.InstanceName))
                {
                    var viewReference = ViewArtifactLinkReference.GetReferences(this.CurrentElement.AsElement()).FirstOrDefault();
                    if (viewReference != null)
                    {
                        ViewSchemaHelper.WithPatternModel(reference.PhysicalPath, (pm, docData) =>
                            {
                                var view = pm.Pattern.GetView(new Guid(viewReference.Host));
                                if (view != null)
                                {
                                    ((INamedElementSchema)view).Name = this.CurrentElement.InstanceName;
                                }
                                else
                                {
                                    tracer.Warn(
                                        Resources.SetAsDefaultViewCommand_TraceViewNotFound, patternModel.InstanceName, viewReference.Host);
                                }
                            }, false);
                    }
                    else
                    {
                        tracer.Warn(
                            Resources.SetAsDefaultViewCommand_TraceReferenceNotFound, patternModel.InstanceName);
                    }
                }
            }
            else
            {
                tracer.Warn(
                    Resources.SyncViewNameCommand_TraceReferenceNotFound, patternModel.InstanceName);
            }
        }
    }
}