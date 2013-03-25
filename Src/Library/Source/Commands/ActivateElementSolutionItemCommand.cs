using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using NuPattern.ComponentModel.Design;
using NuPattern.Extensibility;
using NuPattern.Library.Automation;
using NuPattern.Library.Properties;
using Dsl = Microsoft.VisualStudio.Modeling.Design;

namespace NuPattern.Library.Commands
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
        [Dsl.DisplayNameResource("NuPattern.Library.Automation.TemplateSettings/RawTargetPath.DisplayName", typeof(LibraryDomainModel), "NuPattern.Library.GeneratedCode.DomainModelResx.gen")]
        [Dsl.DescriptionResource("NuPattern.Library.Automation.TemplateSettings/RawTargetPath.Description", typeof(LibraryDomainModel), "NuPattern.Library.GeneratedCode.DomainModelResx.gen")]
        public virtual string TargetPath { get; set; }

        /// <summary>
        /// Gets or sets the name of the target.
        /// </summary>
        [Required]
        [Dsl.DisplayNameResource("NuPattern.Library.Automation.TemplateSettings/RawTargetFileName.DisplayName", typeof(LibraryDomainModel), "NuPattern.Library.GeneratedCode.DomainModelResx.gen")]
        [Dsl.DescriptionResource("NuPattern.Library.Automation.TemplateSettings/RawTargetFileName.Description", typeof(LibraryDomainModel), "NuPattern.Library.GeneratedCode.DomainModelResx.gen")]
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
