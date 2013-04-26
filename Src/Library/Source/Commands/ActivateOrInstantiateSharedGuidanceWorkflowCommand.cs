using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Drawing.Design;
using System.Linq;
using Microsoft.VisualStudio.Shell;
using NuPattern.ComponentModel.Design;
using NuPattern.Diagnostics;
using NuPattern.Library.Properties;
using NuPattern.Runtime;
using NuPattern.Runtime.Design;
using NuPattern.Runtime.Guidance;

namespace NuPattern.Library.Commands
{
    /// <summary>
    /// Command that activates or instantiates a new feature.
    /// </summary>
    [DisplayNameResource(@"ActivateOrInstantiateSharedGuidanceWorkflowCommand_DisplayName", typeof(Resources))]
    [DescriptionResource(@"ActivateOrInstantiateSharedGuidanceWorkflowCommand_Description", typeof(Resources))]
    [CategoryResource(@"AutomationCategory_Guidance", typeof(Resources))]
    [CLSCompliant(false)]
    public class ActivateOrInstantiateSharedGuidanceWorkflowCommand : Command
    {
        private static readonly ITracer tracer = Tracer.Get<ActivateOrInstantiateSharedGuidanceWorkflowCommand>();

        /// <summary>
        /// Gets or sets the feature id.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [TypeConverter(typeof(GuidanceExtensionsTypeConverter))]
        [Editor(typeof(StandardValuesEditor), typeof(UITypeEditor))]
        [DisplayNameResource(@"InstantiateGuidanceWorkflowCommand_ExtensionId_DisplayName", typeof(Resources))]
        [DescriptionResource(@"ActivateGuidanceWorkCommand_ExtensionId_Description", typeof(Resources))]
        public string ExtensionId { get; set; }

        /// <summary>
        /// Gets or sets the guidance extension manager.
        /// </summary>
        [Required]
        [Import]
        public IGuidanceManager GuidanceManager
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the service provider.
        /// </summary>
        [Required]
        [Import(typeof(SVsServiceProvider))]
        public IServiceProvider ServiceProvider
        {
            get;
            set;
        }

        /// <summary>
        /// Executes this instance.
        /// </summary>
        public override void Execute()
        {
            this.ValidateObject();

            tracer.Info(
                Resources.ActivateOrInstantiateSharedGuidanceWorkflowCommand_TraceInitial, this.ExtensionId);

            // Ensure the feature type exists
            var featureRegistration = this.GuidanceManager.InstalledGuidanceExtensions
                .FirstOrDefault(feature => feature.ExtensionId.Equals(this.ExtensionId, StringComparison.OrdinalIgnoreCase));
            if (featureRegistration != null)
            {
                // Show the guidance windows
                if (this.ServiceProvider != null)
                {
                    tracer.Verbose(
                        Resources.ActivateOrInstantiateSharedGuidanceWorkflowCommand_TraceShowingGuidanceExplorer);

                    this.GuidanceManager.ShowGuidanceWindows(this.ServiceProvider);
                }

                // Ensure we are not sharing
                var featureInstance = this.GuidanceManager.InstantiatedGuidanceExtensions.FirstOrDefault(f => f.ExtensionId.Equals(this.ExtensionId, StringComparison.OrdinalIgnoreCase));
                if (featureInstance == null)
                {
                    // Create a default name
                    var instanceName = this.GuidanceManager.GetUniqueInstanceName(featureRegistration.DefaultName);

                    tracer.Info(
                        Resources.ActivateOrInstantiateSharedGuidanceWorkflowCommand_TraceInstantiate, this.ExtensionId, instanceName);

                    // Instantiate the feature
                    featureInstance = this.GuidanceManager.Instantiate(this.ExtensionId, instanceName);
                }

                tracer.Info(
                        Resources.ActivateOrInstantiateSharedGuidanceWorkflowCommand_TraceActivate, featureInstance.InstanceName);

                // Activate feature
                this.GuidanceManager.ActiveGuidanceExtension = featureInstance;
            }
            else
            {
                tracer.Error(
                    Resources.ActivateOrInstantiateSharedGuidanceWorkflowCommand_TraceWorkflowNotFound, this.ExtensionId);
            }
        }
    }
}
