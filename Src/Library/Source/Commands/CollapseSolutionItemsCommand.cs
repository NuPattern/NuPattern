using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using NuPattern.Extensibility;
using NuPattern.Library.Properties;

namespace NuPattern.Library.Commands
{
    /// <summary>
    /// Collapses all solution items from the specified path.
    /// </summary>
    [DisplayNameResource("CollapseSolutionItemsCommand_DisplayName", typeof(Resources))]
    [CategoryResource("AutomationCategory_VisualStudio", typeof(Resources))]
    [DescriptionResource("CollapseSolutionItemsCommand_Description", typeof(Resources))]
    [CLSCompliant(false)]
    public class CollapseSolutionItemsCommand : FeatureCommand
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<CollapseSolutionItemsCommand>();
        private const bool DefaultIncludeProjects = false;

        /// <summary>
        /// Creates a new instance of the <see cref="CollapseSolutionItemsCommand"/> class.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CollapseSolutionItemsCommand()
        {
            this.IncludeProjects = DefaultIncludeProjects;
        }

        /// <summary>
        /// Gets or sets the solution.
        /// </summary>
        [Required]
        [Import(AllowDefault = true)]
        public ISolution Solution
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets whether to collapse projects.
        /// </summary>
        [DefaultValue(DefaultIncludeProjects)]
        [DisplayNameResource("CollapseSolutionItemsCommand_IncludeProjects_DisplayName", typeof(Resources))]
        [DescriptionResource("CollapseSolutionItemsCommand_IncludeProjects_Description", typeof(Resources))]
        public virtual bool IncludeProjects
        {
            get;
            set;
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        public override void Execute()
        {
            this.ValidateObject();

            tracer.TraceInformation(
                Resources.CollapseSolutionItemsCommand_TraceInitial, this.IncludeProjects);

            var options = (this.IncludeProjects) ? CollapseOptions.All : CollapseOptions.All & ~(CollapseOptions.IncludeProjects);
            this.Solution.CollapseAll(options);
        }
    }
}
