using System;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using NuPattern.ComponentModel.Design;
using NuPattern.Extensibility;
using NuPattern.Library.Properties;
using NuPattern.Runtime;
using NuPattern.VisualStudio.Solution;

namespace NuPattern.Library.Commands
{
    /// <summary>
    /// Command used to set a build action over a project item
    /// </summary>
    [DisplayNameResource("SetBuildActionCommand_DisplayName", typeof(Resources))]
    [CategoryResource("AutomationCategory_Automation", typeof(Resources))]
    [DescriptionResource("SetBuildActionCommand_Description", typeof(Resources))]
    [CLSCompliant(false)]
    public class SetBuildActionCommand : FeatureCommand
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<SetBuildActionCommand>();

        /// <summary>
        /// Gets or sets the solution.
        /// </summary>
        /// <value>The solution.</value>
        [Required]
        [Import(AllowDefault = true)]
        public ISolution Solution { get; set; }

        /// <summary>
        /// Gets or sets the URI reference service.
        /// </summary>
        [Required]
        [Import(AllowDefault = true)]
        public virtual IFxrUriReferenceService UriService { get; set; }

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
        [DisplayNameResource("SetBuildActionCommand_TargetPath_DisplayName", typeof(Resources))]
        [DescriptionResource("SetBuildActionCommand_TargetPath_Description", typeof(Resources))]
        public virtual string TargetPath { get; set; }

        /// <summary>
        /// Gets or sets the target path.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [DisplayNameResource("SetBuildActionCommand_BuildAction_DisplayName", typeof(Resources))]
        [DescriptionResource("SetBuildActionCommand_BuildAction_Description", typeof(Resources))]
        public virtual BuildAction BuildAction { get; set; }

        /// <summary>
        /// Transforms the Templates
        /// </summary>
        public override void Execute()
        {
            this.ValidateObject();

            tracer.TraceInformation(
                Resources.SetBuildActionCommand_TraceInitial, this, CurrentElement.InstanceName, this.TargetPath, this.BuildAction);

            var resolver = new PathResolver(this.CurrentElement, this.UriService,
                this.TargetPath, this.CurrentElement.InstanceName);
            resolver.Resolve();

            if (!string.IsNullOrEmpty(resolver.Path))
            {
                var item = this.Solution.Find<IItem>(resolver.Path).FirstOrDefault();
                if (item == null)
                {
                    throw new InvalidOperationException(
                        string.Format(CultureInfo.CurrentCulture, Resources.SetBuildActionCommand_ErrorTargetPathNotAnItem, this.CurrentElement.InstanceName, this.TargetPath));
                }
                else
                {
                    tracer.TraceInformation(
                        Resources.SetBuildActionCommand_TraceBuildAction, this, CurrentElement.InstanceName, this.BuildAction, item.GetLogicalPath());

                    item.Data.ItemType = BuildAction.ToString();
                }
            }
        }
    }
}