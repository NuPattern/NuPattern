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
using NuPattern.Runtime.References;

namespace NuPattern.Library.Commands
{
    /// <summary>
    /// Command that instantiates a new feature.
    /// </summary>
    [DisplayNameResource(@"InstantiateGuidanceWorkflowCommand_DisplayName", typeof(Resources))]
    [DescriptionResource(@"InstantiateGuidanceWorkflowCommand_Description", typeof(Resources))]
    [CategoryResource(@"AutomationCategory_Guidance", typeof(Resources))]
    [CLSCompliant(false)]
    public class InstantiateGuidanceWorkflowCommand : Command
    {
        private const bool DefaultActivateOnInstantiation = true;
        private const bool DefaultSharedInstance = false;
        private static readonly ITracer tracer = Tracer.Get<InstantiateGuidanceWorkflowCommand>();

        /// <summary>
        /// Initializes a new instance of the <see cref="InstantiateGuidanceWorkflowCommand"/> class.
        /// </summary>
        public InstantiateGuidanceWorkflowCommand()
        {
            this.ActivateOnInstantiation = DefaultActivateOnInstantiation;
            this.SharedInstance = DefaultSharedInstance;
            this.DefaultInstanceName = string.Empty;
        }

        /// <summary>
        /// Gets or sets the guidance extension id.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [TypeConverter(typeof(GuidanceExtensionsTypeConverter))]
        [Editor(typeof(StandardValuesEditor), typeof(UITypeEditor))]
        [DisplayNameResource(@"InstantiateGuidanceWorkflowCommand_ExtensionId_DisplayName", typeof(Resources))]
        [DescriptionResource(@"InstantiateGuidanceWorkflowCommand_ExtensionId_Description", typeof(Resources))]
        public string ExtensionId { get; set; }

        /// <summary>
        /// Gets or sets the default instance name.
        /// </summary>
        [DisplayNameResource(@"InstantiateGuidanceWorkflowCommand_DefaultInstanceName_DisplayName", typeof(Resources))]
        [DescriptionResource(@"InstantiateGuidanceWorkflowCommand_DefaultInstanceName_Description", typeof(Resources))]
        public string DefaultInstanceName { get; set; }

        /// <summary>
        /// Gets or sets the default instance name.
        /// </summary>
        [DefaultValue(DefaultSharedInstance)]
        [DisplayNameResource(@"InstantiateGuidanceWorkflowCommand_SharedInstance_DisplayName", typeof(Resources))]
        [DescriptionResource(@"InstantiateGuidanceWorkflowCommand_SharedInstance_Description", typeof(Resources))]
        public bool SharedInstance { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to make the newly created instance the current active instance.
        /// </summary>
        [DefaultValue(DefaultActivateOnInstantiation)]
        [DisplayNameResource(@"InstantiateGuidanceWorkflowCommand_ActivateOnInstantiation_DisplayName", typeof(Resources))]
        [DescriptionResource(@"InstantiateGuidanceWorkflowCommand_ActivateOnInstantiation_Description", typeof(Resources))]
        public bool ActivateOnInstantiation { get; set; }

        /// <summary>
        /// Gets or sets the current element.
        /// </summary>
        [Required]
        [Import(AllowDefault = true)]
        public IProductElement CurrentElement { get; set; }

        /// <summary>
        /// Gets or sets the guidance extension manager.
        /// </summary>
        [Required]
        [Import]
        public IGuidanceManager GuidanceManager { get; set; }

        /// <summary>
        /// Gets or sets the service provider.
        /// </summary>
        [Required]
        [Import(typeof(SVsServiceProvider))]
        public IServiceProvider ServiceProvider { get; set; }

        /// <summary>
        /// Executes this instance.
        /// </summary>
        public override void Execute()
        {
            this.ValidateObject();

            tracer.Info(
                Resources.InstantiateGuidanceWorkflowCommand_TraceInitial,
                this.CurrentElement.InstanceName, this.ExtensionId, this.DefaultInstanceName, this.SharedInstance, this.ActivateOnInstantiation);

            // Ensure the feature type exists
            var featureRegistration =
                this.GuidanceManager.InstalledGuidanceExtensions.FirstOrDefault(
                    feature => feature.ExtensionId.Equals(this.ExtensionId, StringComparison.OrdinalIgnoreCase));
            if (featureRegistration != null)
            {
                string instanceName = string.Empty;

                // Ensure we are not sharing
                var sharedInstance = this.GuidanceManager.InstantiatedGuidanceExtensions.FirstOrDefault(f => f.ExtensionId.Equals(this.ExtensionId, StringComparison.OrdinalIgnoreCase));
                if (this.SharedInstance && sharedInstance != null)
                {
                    instanceName = sharedInstance.InstanceName;

                    // Activate shared feature
                    if (this.ActivateOnInstantiation)
                    {
                        tracer.Info(
                            Resources.InstantiateGuidanceWorkflowCommand_TraceActivateShared, this.CurrentElement.InstanceName, instanceName);

                        this.GuidanceManager.ActivateGuidanceInstance(this.ServiceProvider, sharedInstance);
                    }
                }
                else
                {
                    // Create a default name
                    instanceName = this.GuidanceManager.GetUniqueInstanceName(featureRegistration.DefaultName);
                    if (!string.IsNullOrEmpty(this.DefaultInstanceName))
                    {
                        instanceName = this.GuidanceManager.GetUniqueInstanceName(this.DefaultInstanceName);

                        tracer.Verbose(
                            Resources.InstantiateGuidanceWorkflowCommand_TraceCreateUniqueInstanceName, this.CurrentElement.InstanceName, instanceName);
                    }

                    tracer.Info(
                        Resources.InstantiateGuidanceWorkflowCommand_TraceInstantiateNew, this.CurrentElement.InstanceName, this.ExtensionId, instanceName);

                    // Instantiate the feature and activate it.
                    var feature = this.GuidanceManager.InstantiateGuidanceInstance(this.ServiceProvider, this.ExtensionId, instanceName);
                    if (this.ActivateOnInstantiation)
                    {
                        tracer.Info(
                            Resources.InstantiateGuidanceWorkflowCommand_TraceActivateNew, this.CurrentElement.InstanceName, instanceName);

                        this.GuidanceManager.ActivateGuidanceInstance(this.ServiceProvider, feature);
                    }

                    instanceName = feature.InstanceName;
                }

                tracer.Info(
                    Resources.InstantiateGuidanceWorkflowCommand_TraceAddingReference, this.CurrentElement.InstanceName, instanceName);

                // Add the Guidance reference
                this.CurrentElement.AddReference(ReferenceKindConstants.Guidance, instanceName, true);
            }
            else
            {
                tracer.Warn(
                    Resources.InstantiateGuidanceWorkflowCommand_TraceWorkflowNotFound, this.CurrentElement.InstanceName, this.ExtensionId);
            }
        }
    }
}