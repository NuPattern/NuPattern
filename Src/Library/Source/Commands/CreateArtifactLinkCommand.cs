using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using NuPattern.ComponentModel.Design;
using NuPattern.Extensibility;
using NuPattern.Extensibility.References;
using NuPattern.Library.Properties;
using NuPattern.Runtime;

namespace NuPattern.Library.Commands
{
    /// <summary>
    /// Creates an artifact link between the owner element and the specified Items
    /// </summary>
    [DisplayNameResource("CreateArtifactLinkCommand_DisplayName", typeof(Resources))]
    [DescriptionResource("CreateArtifactLinkCommand_Description", typeof(Resources))]
    [CategoryResource("AutomationCategory_Automation", typeof(Resources))]
    [CLSCompliant(false)]
    public class CreateArtifactLinkCommand : FeatureCommand
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<CreateArtifactLinkCommand>();

        /// <summary>
        /// Gets or sets the URI reference service.
        /// </summary>
        [Required]
        [Import(AllowDefault = true)]
        public IFxrUriReferenceService UriService { get; set; }

        /// <summary>
        /// Gets or sets the current element.
        /// </summary>
        [Required]
        [Import(AllowDefault = true)]
        public IProductElement CurrentElement { get; set; }

        /// <summary>
        /// Gets or sets the solution.
        /// </summary>
        [Required]
        [Import(AllowDefault = true)]
        public ISolution Solution { get; set; }

        /// <summary>
        /// The Items to link to the owner element.
        /// </summary>
        [Required]
        [DisplayNameResource("CreateArtifactLinkCommand_Items_DisplayName", typeof(Resources))]
        [DescriptionResource("CreateArtifactLinkCommand_Items_Description", typeof(Resources))]
        public virtual IEnumerable<string> Items { get; set; }

        /// <summary>
        /// Executes the command linking the Items to the owner element.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase")]
        public override void Execute()
        {
            this.ValidateObject();

            tracer.TraceInformation(
                Resources.CreateArtifactLinkCommand_TraceInitial, this.CurrentElement.InstanceName);

            foreach (var item in Solution.Traverse().Where(t => !string.IsNullOrEmpty(t.PhysicalPath) && Items.Contains(t.PhysicalPath.ToLowerInvariant())))
            {
                tracer.TraceInformation(
                    Resources.CreateArtifactLinkCommand_TraceCreateLink, this.CurrentElement.InstanceName, item.GetLogicalPath());

                SolutionArtifactLinkReference.AddReference(this.CurrentElement, UriService.CreateUri(item));
            }
        }
    }
}
