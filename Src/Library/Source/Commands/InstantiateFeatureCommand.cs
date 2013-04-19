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
    [DisplayNameResource("InstantiateFeatureCommand_DisplayName", typeof(Resources))]
    [CategoryResource("AutomationCategory_Guidance", typeof(Resources))]
    [DescriptionResource("InstantiateFeatureCommand_Description", typeof(Resources))]
    [CLSCompliant(false)]
    public class InstantiateFeatureCommand : Command
    {
        private const bool DefaultActivateOnInstantiation = true;
        private const bool DefaultSharedInstance = false;
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<InstantiateFeatureCommand>();

        /// <summary>
        /// Initializes a new instance of the <see cref="InstantiateFeatureCommand"/> class.
        /// </summary>
        public InstantiateFeatureCommand()
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
        [DisplayNameResource("InstantiateFeatureCommand_GuidanceExtensionId_DisplayName", typeof(Resources))]
        [DescriptionResource("InstantiateFeatureCommand_GuidanceExtensionId_Description", typeof(Resources))]
        public string GuidanceExtensionId { get; set; }

        /// <summary>
        /// Gets or sets the default instance name.
        /// </summary>
        [DisplayNameResource("InstantiateFeatureCommand_DefaultInstanceName_DisplayName", typeof(Resources))]
        [DescriptionResource("InstantiateFeatureCommand_DefaultInstanceName_Description", typeof(Resources))]
        public string DefaultInstanceName { get; set; }

        /// <summary>
        /// Gets or sets the default instance name.
        /// </summary>
        [DefaultValue(DefaultSharedInstance)]
        [DisplayNameResource("InstantiateFeatureCommand_SharedInstance_DisplayName", typeof(Resources))]
        [DescriptionResource("InstantiateFeatureCommand_SharedInstance_Description", typeof(Resources))]
        public bool SharedInstance { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to make the newly created instance the current active instance.
        /// </summary>
        [DefaultValue(DefaultActivateOnInstantiation)]
        [DisplayNameResource("InstantiateFeatureCommand_ActivateOnInstantiation_DisplayName", typeof(Resources))]
        [DescriptionResource("InstantiateFeatureCommand_ActivateOnInstantiation_Description", typeof(Resources))]
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

            tracer.TraceInformation(
                Resources.InstantiateFeatureCommand_TraceInitial,
                this.CurrentElement.InstanceName, this.GuidanceExtensionId, this.DefaultInstanceName, this.SharedInstance, this.ActivateOnInstantiation);

            // Ensure the feature type exists
            var featureRegistration =
                this.GuidanceManager.InstalledGuidanceExtensions.FirstOrDefault(
                    feature => feature.ExtensionId.Equals(this.GuidanceExtensionId, StringComparison.OrdinalIgnoreCase));
            if (featureRegistration != null)
            {
                string instanceName = string.Empty;

                // Ensure we are not sharing
                var sharedInstance = this.GuidanceManager.InstantiatedGuidanceExtensions.FirstOrDefault(f => f.ExtensionId.Equals(this.GuidanceExtensionId, StringComparison.OrdinalIgnoreCase));
                if (this.SharedInstance && sharedInstance != null)
                {
                    instanceName = sharedInstance.InstanceName;

                    // Activate shared feature
                    if (this.ActivateOnInstantiation)
                    {
                        tracer.TraceInformation(
                            Resources.InstantiateFeatureCommand_TraceActivateShared, this.CurrentElement.InstanceName, instanceName);

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

                        tracer.TraceVerbose(
                            Resources.InstantiateFeatureCommand_TraceCreateUniqueInstanceName, this.CurrentElement.InstanceName, instanceName);
                    }

                    tracer.TraceInformation(
                        Resources.InstantiateFeatureCommand_TraceInstantiateNew, this.CurrentElement.InstanceName, this.GuidanceExtensionId, instanceName);

                    // Instantiate the feature and activate it.
                    var feature = this.GuidanceManager.Instantiate(this.GuidanceExtensionId, instanceName);
                    if (this.ActivateOnInstantiation)
                    {
                        tracer.TraceInformation(
                            Resources.InstantiateFeatureCommand_TraceActivateNew, this.CurrentElement.InstanceName, instanceName);

                        this.GuidanceManager.ActivateGuidanceInstance(this.ServiceProvider, feature);
                    }

                    instanceName = feature.InstanceName;
                }

                tracer.TraceInformation(
                    Resources.InstantiateFeatureCommand_TraceAddingReference, this.CurrentElement.InstanceName, instanceName);

                // Add the Guidance reference
                this.CurrentElement.AddReference(ReferenceKindConstants.Guidance, instanceName, true);
            }
            else
            {
                tracer.TraceWarning(
                    Resources.InstantiateFeatureCommand_TraceFeatureNotFound, this.CurrentElement.InstanceName, this.GuidanceExtensionId);
            }
        }
    }
}