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
    /// Command to add a new view
    /// </summary>
    [CLSCompliant(false)]
    [DisplayNameResource("AddViewCommand_DisplayName", typeof(Resources))]
    [CategoryResource("AutomationCategory_PatternToolkitAuthoring", typeof(Resources))]
    [DescriptionResource("AddViewCommand_Description", typeof(Resources))]
    public class AddViewCommand : NuPattern.Runtime.Command
    {
        private static readonly ITracer tracer = Tracer.Get<AddViewCommand>();

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
        /// Adds a new view
        /// </summary>
        public override void Execute()
        {
            this.ValidateObject();

            var patternModel = this.CurrentElement.Parent.Parent.AsElement();

            tracer.Info(
                Resources.AddViewCommand_TraceInitial, this.CurrentElement.InstanceName);

            // Ensure the pattern model file exists
            var reference = SolutionArtifactLinkReference.GetResolvedReferences(patternModel, this.UriService).FirstOrDefault();
            if (reference != null)
            {
                using (tracer.StartActivity(Resources.AddViewCommand_TraceAddingView, this.CurrentElement.InstanceName, this.CurrentElement.InstanceName))
                {
                    ViewSchemaHelper.WithPatternModel(reference.PhysicalPath, (pm, docData) =>
                        {
                            // Add new view (if not exists)
                            var viewSchema = pm.Pattern.Views.FirstOrDefault(v => ((INamedElementSchema)v).Name.Equals(this.CurrentElement.InstanceName, StringComparison.OrdinalIgnoreCase));
                            if (viewSchema == null)
                            {
                                // Create a new view
                                viewSchema = pm.CreateNewViewDiagram(docData, this.CurrentElement.InstanceName);
                            }

                            // Add artifact link (if not exist)
                            if (ViewArtifactLinkReference.GetReferenceValues(this.CurrentElement.AsElement()).FirstOrDefault() == null)
                            {
                                tracer.Info(
                                    Resources.AddViewCommand_TraceAddingReference, this.CurrentElement.InstanceName);

                                ViewArtifactLinkReference.AddReference(this.CurrentElement.AsElement(),
                                                                       this.UriService.CreateUri(viewSchema));
                            }
                        }, true);
                }
            }
            else
            {
                tracer.Warn(
                    Resources.AddViewCommand_TraceReferenceNotFound, this.CurrentElement.InstanceName);
            }
        }
    }
}
