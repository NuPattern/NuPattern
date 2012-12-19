using System;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using NuPattern.Extensibility;
using NuPattern.Extensibility.References;
using NuPattern.Library.Properties;
using NuPattern.Runtime;

namespace NuPattern.Library.Conditions
{
    /// <summary>
    /// A <see cref="Condition"/> that evaluates to true if all artifacts on the current element are saved.
    /// </summary>
    [DisplayNameResource("ArtifactsSavedCondition_DisplayName", typeof(Resources))]
    [CategoryResource("AutomationCategory_Automation", typeof(Resources))]
    [DescriptionResource("ArtifactsSavedCondition_Description", typeof(Resources))]
    [CLSCompliant(false)]
    public class ArtifactsSavedCondition : Condition
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<ArtifactsSavedCondition>();

        /// <summary>
        /// Gets or sets the current element.
        /// </summary>
        [Required]
        [Import(AllowDefault = true)]
        public IProductElement CurrentElement { get; set; }

        /// <summary>
        /// Gets or sets the URI reference service.
        /// </summary>
        [Required]
        [Import(AllowDefault = true)]
        public IFxrUriReferenceService UriReferenceService { get; set; }

        /// <summary>
        /// Evaluates the condition by verifying the existence of any artifact references.
        /// </summary>
        public override bool Evaluate()
        {
            this.ValidateObject();

            tracer.TraceInformation(
                Resources.ArtifactsSavedCondition_TraceInitial, this.CurrentElement.InstanceName);

            var solutionItems = SolutionArtifactLinkReference.GetResolvedReferences(this.CurrentElement, this.UriReferenceService).ToList();
            foreach (IItemContainer solutionItem in solutionItems)
            {
                // Filter for items only
                var item = solutionItem as IItem;
                if (item != null)
                {
                    var projectItem = item.As<EnvDTE.ProjectItem>();
                    if (projectItem != null)
                    {
                        if (!projectItem.Saved)
                        {
                            tracer.TraceInformation(
                                Resources.ArtifactsSavedCondition_TraceEvaluatedNotSaved, projectItem.Name, this.CurrentElement.InstanceName);

                            return false;
                        }
                    }
                }
            }

            tracer.TraceInformation(
                Resources.ArtifactsSavedCondition_TraceEvaluatedAllSaved, this.CurrentElement.InstanceName);

            return true;
        }
    }
}
