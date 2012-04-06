using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.Patterning.Library.Automation;
using Microsoft.VisualStudio.Patterning.Library.Properties;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using Dsl = Microsoft.VisualStudio.Modeling.Design;

namespace Microsoft.VisualStudio.Patterning.Library.Commands
{
    /// <summary>
    /// Activates (opens or selects) the specified solution item.
    /// </summary>
    [DisplayNameResource("ActivateElementSolutionItemCommand_DisplayName", typeof(Resources))]
    [CategoryResource("AutomationCategory_Automation", typeof(Resources))]
    [DescriptionResource("ActivateElementSolutionItemCommand_Description", typeof(Resources))]
    [CLSCompliant(false)]
    public class ActivateElementSolutionItemCommand : ActivateSolutionItemsCommand
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<ActivateElementSolutionItemCommand>();

        /// <summary>
        /// Gets or sets the target path.
        /// </summary>
        [Dsl.DisplayNameResource("Microsoft.VisualStudio.Patterning.Library.Automation.TemplateSettings/RawTargetPath.DisplayName", typeof(LibraryDomainModel), "Microsoft.VisualStudio.Patterning.Library.GeneratedCode.DomainModelResx")]
        [Dsl.DescriptionResource("Microsoft.VisualStudio.Patterning.Library.Automation.TemplateSettings/RawTargetPath.Description", typeof(LibraryDomainModel), "Microsoft.VisualStudio.Patterning.Library.GeneratedCode.DomainModelResx")]
        public virtual string TargetPath { get; set; }

        /// <summary>
        /// Gets or sets the name of the target.
        /// </summary>
        [Required]
        [Dsl.DisplayNameResource("Microsoft.VisualStudio.Patterning.Library.Automation.TemplateSettings/RawTargetFileName.DisplayName", typeof(LibraryDomainModel), "Microsoft.VisualStudio.Patterning.Library.GeneratedCode.DomainModelResx")]
        [Dsl.DescriptionResource("Microsoft.VisualStudio.Patterning.Library.Automation.TemplateSettings/RawTargetFileName.Description", typeof(LibraryDomainModel), "Microsoft.VisualStudio.Patterning.Library.GeneratedCode.DomainModelResx")]
        public virtual string TargetFileName { get; set; }

        /// <summary>
        /// Executes the activation behavior.
        /// </summary>
        public override void Execute()
        {
            this.ValidateObject();

            tracer.TraceInformation(
                Resources.ActivateElementSolutionItemCommand_TraceInitial, this.CurrentElement.InstanceName, this.Open, this.TargetPath, this.TargetFileName);

            base.Execute();
        }

        /// <summary>
        /// Gets the solution items from resolving the path and filename.
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<IItemContainer> GetSolutionItems()
        {
            var resolver = new PathResolver(this.CurrentElement, this.UriReferenceService, this.TargetPath, this.TargetFileName);
            resolver.Resolve();

            var container = this.Solution.Find(resolver.Path).FirstOrDefault();
            var item = container.Items.FirstOrDefault(i => i.Name == resolver.FileName);
            return (item == null) ? Enumerable.Empty<IItemContainer>() : new[] { item };
        }
    }
}
