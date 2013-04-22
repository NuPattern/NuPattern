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
    /// Command to set a view as the default view
    /// </summary>
    [CLSCompliant(false)]
    [DisplayNameResource("SetAsDefaultViewCommand_DisplayName", typeof(Resources))]
    [CategoryResource("AutomationCategory_PatternToolkitAuthoring", typeof(Resources))]
    [DescriptionResource("SetAsDefaultViewCommand_Description", typeof(Resources))]
    public class SetAsDefaultViewCommand : NuPattern.Runtime.Command
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<SetAsDefaultViewCommand>();

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
                    var viewReference = ViewArtifactLinkReference.GetReferences(this.CurrentElement.AsElement()).FirstOrDefault();
                    if (viewReference != null)
                    {
                        ViewSchemaHelper.WithPatternModel(reference.PhysicalPath, pm =>
                            {
                                var view = pm.Pattern.GetView(new Guid(viewReference.Host));
                                if (view != null)
                                {
                                    pm.Pattern.SetDefaultView(new Guid(viewReference.Host));
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
                    Resources.SetAsDefaultViewCommand_TraceReferenceNotFound, patternModel.InstanceName);
            }
        }
    }
}