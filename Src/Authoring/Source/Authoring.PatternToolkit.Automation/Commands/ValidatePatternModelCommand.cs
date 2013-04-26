using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using NuPattern.Authoring.PatternToolkit.Automation.Properties;
using NuPattern.ComponentModel.Design;
using NuPattern.Diagnostics;
using NuPattern.Runtime;
using NuPattern.Runtime.Schema;
using NuPattern.VisualStudio;
using NuPattern.VisualStudio.Solution;

namespace NuPattern.Authoring.PatternToolkit.Automation.Commands
{
    /// <summary>
    /// Command used to validate the pattern model 
    /// </summary>
    [DisplayNameResource("ValidatePatternModelCommand_DisplayName", typeof(Resources))]
    [CategoryResource("AutomationCategory_PatternToolkitAuthoring", typeof(Resources))]
    [DescriptionResource("ValidatePatternModelCommand_Description", typeof(Resources))]
    [CLSCompliant(false)]
    public class ValidatePatternModelCommand : NuPattern.Runtime.Command
    {
        private static readonly ITracer tracer = Tracer.Get<ValidatePatternModelCommand>();

        private DTE dte;

        [Browsable(false)]
        internal DTE Dte
        {
            get
            {
                return this.dte ?? (this.dte = ServiceProvider.GlobalProvider.GetService<SDTE, DTE>());
            }
        }

        /// <summary>
        /// Gets or sets the solution.
        /// </summary>
        /// <value>The solution.</value>
        [Required]
        [Import(AllowDefault = true)]
        public ISolution Solution { get; set; }

        /// <summary>
        /// Gets or sets the service that resolves templates.
        /// </summary>
        [Required]
        [Import(AllowDefault = true)]
        public virtual IUriReferenceService UriService { get; set; }

        /// <summary>
        /// Gets or sets the current element.
        /// </summary>
        /// <value>The current element.</value>
        [Required]
        [Import(AllowDefault = true)]
        public IProductElement CurrentElement { get; set; }

        /// <summary>
        /// Gets or sets the target path.
        /// </summary>
        [DisplayNameResource("ValidatePatternModelCommand_TargetPath_DisplayName", typeof(Resources))]
        [DescriptionResource("ValidatePatternModelCommand_TargetPath_Description", typeof(Resources))]
        public virtual string TargetPath { get; set; }

        /// <summary>
        /// Executes this instance.
        /// </summary>
        public override void Execute()
        {
            this.ValidateObject();

            tracer.Info(
                Resources.ValidatePatternModelCommand_TraceInitial, this.CurrentElement.InstanceName, this.TargetPath);

            var resolver = new PathResolver(this.CurrentElement, this.UriService,
                this.TargetPath);

            resolver.Resolve();

            if (!string.IsNullOrEmpty(resolver.Path))
            {
                var itemContainer = this.Solution.Find(resolver.Path).FirstOrDefault();

                if (itemContainer != null)
                {
                    tracer.Info(
                        Resources.ValidatePatternModelCommand_TraceValidating, this.CurrentElement.InstanceName, itemContainer.PhysicalPath);

                    if (!PatternModelDocHelper.ValidateDocument(itemContainer.PhysicalPath))
                    {
                        tracer.Info(
                            Resources.ValidatePatternModelCommand_TraceOpeningForResolution, this.CurrentElement.InstanceName, itemContainer.Name);
                    }
                }
            }
            else
            {
                tracer.Warn(
                    Resources.ValidatePatternModelCommand_TraceDesignerNotFound, this.CurrentElement.InstanceName);
            }
        }

        private static Window OpenDesigner(DTE dte, string fileName)
        {
            var projectItem = dte.Solution.FindProjectItem(fileName);

            return projectItem.Open(EnvDTE.Constants.vsViewKindDesigner);
        }
    }
}