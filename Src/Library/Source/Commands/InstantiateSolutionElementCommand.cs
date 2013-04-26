using System;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using NuPattern.ComponentModel.Design;
using NuPattern.Diagnostics;
using NuPattern.Library.Properties;
using NuPattern.Runtime;

namespace NuPattern.Library.Commands
{
    /// <summary>
    /// Creates a new instance of a pattern from another pattern toolkit.
    /// </summary>
    [DisplayNameResource(@"InstantiateSolutionElementCommand_DisplayName", typeof(Resources))]
    [DescriptionResource(@"InstantiateSolutionElementCommand_Description", typeof(Resources))]
    [CategoryResource(@"AutomationCategory_Automation", typeof(Resources))]
    [CLSCompliant(false)]
    public class InstantiateSolutionElementCommand : Command
    {
        private static readonly ITracer tracer = Tracer.Get<InstantiateSolutionElementCommand>();

        /// <summary>
        /// Gets or sets the VSIX ID of the solution element to be added.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [DisplayNameResource(@"InstantiateSolutionElementCommand_ToolkitIdentifier_DisplayName", typeof(Resources))]
        [DescriptionResource(@"InstantiateSolutionElementCommand_ToolkitIdentifier_Description", typeof(Resources))]
        public string ToolkitIdentifier
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name that will be used for the element in Solution Builder.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [DisplayNameResource(@"InstantiateSolutionElementCommand_InstanceName_DisplayName", typeof(Resources))]
        [DescriptionResource(@"InstantiateSolutionElementCommand_InstanceName_Description", typeof(Resources))]
        public string InstanceName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the pattern manager.
        /// </summary>
        [Required]
        [Import(AllowDefault = true)]
        public IPatternManager PatternManager
        {
            get;
            set;
        }

        /// <summary>
        /// Executes this commmand.
        /// </summary>
        public override void Execute()
        {
            this.ValidateObject();

            tracer.Info(
                Resources.InstantiateSolutionElementCommand_TraceInitial,
                    this.ToolkitIdentifier, this.InstanceName);

            IInstalledToolkitInfo toolkitInfo = this.PatternManager.InstalledToolkits
                .FirstOrDefault(element => element.Id == this.ToolkitIdentifier);

            if (toolkitInfo == null)
            {
                throw new InvalidOperationException(string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.InstantiateSolutionElementCommand_ErrorSolutionElementNotInstalled,
                    this.ToolkitIdentifier));
            }

            this.PatternManager.CreateProduct(toolkitInfo, this.InstanceName);

            tracer.Info(
                Resources.InstantiateSolutionElementCommand_TraceInstantiated, this.ToolkitIdentifier, this.InstanceName);
        }
    }
}