using System;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using NuPattern.Authoring.Authoring;
using NuPattern.Authoring.Automation.Commands;
using NuPattern.Authoring.Automation.Properties;
using NuPattern.Extensibility;

namespace NuPattern.Authoring.Toolkit.Automation.Commands
{
    /// <summary>
    /// Creates the initial instances of descendant elements in the pattern.
    /// </summary>
    [DisplayNameResource("CreateAutomationLibraryExtensionCommand_DisplayName", typeof(Resources))]
    [CategoryResource("AutomationCategory_PatternToolkitAuthoring", typeof(Resources))]
    [DescriptionResource("CreateAutomationLibraryExtensionCommand_Description", typeof(Resources))]
    [CLSCompliant(false)]
    public class CreateAutomationLibraryExtensionCommand : FeatureCommand
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<CreateAutomationLibraryExtensionCommand>();

        /// <summary>
        /// Gets or sets the current element.
        /// </summary>
        [Required]
        [Import(AllowDefault = true)]
        public IAutomationCollection CurrentElement { get; set; }

        /// <summary>
        /// Executes the creation command.
        /// </summary>
        public override void Execute()
        {
            try
            {
                this.ValidateObject();

                tracer.TraceInformation(
                    Resources.CreateAutomationLibraryExtensionCommand_TraceInital, this.CurrentElement.InstanceName);

                // Create Automation Library
                if (this.CurrentElement.AutomationLibrary == null)
                {
                    var collection = this.CurrentElement.AsCollection();
                    var instanceName = collection.Info.ExtensionPoints
                            .FirstOrDefault(e => e.Name == typeof(AutomationLibrary).Name).DisplayName;

                    tracer.TraceInformation(
                        Resources.CreateAutomationLibraryExtensionCommand_TraceCreatingInstance, this.CurrentElement.InstanceName, instanceName);

                    this.CurrentElement.CreateAutomationLibrary(
                        instanceName,
                        AutomationLibraryToolkitInfo.ProductId,
                        AutomationLibraryToolkitInfo.ToolkitId);
                }
            }
            catch (Exception)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                    Resources.CreateAutomationLibraryExtensionCommand_ErrorFailedInstantiation, this.CurrentElement.InstanceName, AutomationLibraryToolkitInfo.RegistrationName));
            }
        }
    }
}
