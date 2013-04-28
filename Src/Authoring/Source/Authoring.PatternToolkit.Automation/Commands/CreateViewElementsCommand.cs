using System;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NuPattern.Authoring.PatternToolkit.Automation.Properties;
using NuPattern.ComponentModel.Design;
using NuPattern.Diagnostics;
using NuPattern.Runtime;
using NuPattern.Runtime.References;
using NuPattern.Runtime.Schema;

namespace NuPattern.Authoring.PatternToolkit.Automation.Commands
{
    /// <summary>
    /// Command to create all views from the current pattern, when first created.
    /// </summary>
    [CLSCompliant(false)]
    [DisplayNameResource("CreateViewElementsCommand_DisplayName", typeof(Resources))]
    [CategoryResource("AutomationCategory_PatternToolkitAuthoring", typeof(Resources))]
    [DescriptionResource("CreateViewElementsCommand_Description", typeof(Resources))]
    public class CreateViewElementsCommand : NuPattern.Runtime.Command
    {
        private static readonly ITracer tracer = Tracer.Get<CreateViewElementsCommand>();

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
        public IPatternModel CurrentElement { get; set; }

        /// <summary>
        /// Executes the command.
        /// </summary>
        public override void Execute()
        {
            this.ValidateObject();

            tracer.Info(
                Resources.CreateViewElementsCommand_TraceInitial, this.CurrentElement.InstanceName);

            // Ensure the pattern model file exists
            var reference = SolutionArtifactLinkReference.GetResolvedReferences(this.CurrentElement.AsElement(), this.UriService).FirstOrDefault();
            if (reference != null)
            {
                using (tracer.StartActivity(Resources.CreateViewElementsCommand_TraceAddingViews, this.CurrentElement.InstanceName))
                {
                    ViewSchemaHelper.WithPatternModel(reference.PhysicalPath, (pm, docData) =>
                        {
                            pm.Pattern.Views.ForEach(v =>
                            {
                                var viewName = ((INamedElementSchema)v).Name;
                                tracer.Info(
                                    Resources.CreateViewElementsCommand_TraceCreatingView, this.CurrentElement.InstanceName, viewName);

                                this.CurrentElement.Views.CreateViewModel(viewName);
                            });
                        }, false);
                }
            }
            else
            {
                tracer.Warn(
                    Resources.CreateViewElementsCommand_TraceReferenceNotFound, this.CurrentElement.InstanceName);
            }
        }
    }
}