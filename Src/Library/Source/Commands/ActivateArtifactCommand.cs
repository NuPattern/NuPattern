using System;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using NuPattern.ComponentModel.Design;
using NuPattern.Library.Properties;
using NuPattern.Runtime.References;

namespace NuPattern.Library.Commands
{
    /// <summary>
    /// Activates (opens or selects) the linked artifacts associated to current element.
    /// </summary>
    [DisplayNameResource("ActivateArtifactCommand_DisplayName", typeof(Resources))]
    [CategoryResource("AutomationCategory_Automation", typeof(Resources))]
    [DescriptionResource("ActivateArtifactCommand_Description", typeof(Resources))]
    [CLSCompliant(false)]
    public class ActivateArtifactCommand : ActivateSolutionItemsCommand
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<ActivateArtifactCommand>();

        /// <summary>
        /// Executes the activation behavior.
        /// </summary>
        public override void Execute()
        {
            this.ValidateObject();

            tracer.TraceInformation(
                Resources.ActivateArtifactCommand_TraceInitial, this.CurrentElement.InstanceName, this.Open);

            base.Execute();
        }

        /// <summary>
        /// Gets the solution items from the current elements artifact links.
        /// </summary>
        /// <returns></returns>
        protected override System.Collections.Generic.IEnumerable<IItemContainer> GetSolutionItems()
        {
            return SolutionArtifactLinkReference.GetResolvedReferences(this.CurrentElement, this.UriReferenceService);
        }
    }
}