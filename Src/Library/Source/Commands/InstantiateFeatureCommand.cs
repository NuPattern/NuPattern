using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Drawing.Design;
using System.Linq;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
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
    public class InstantiateFeatureCommand : FeatureCommand
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
        /// Gets or sets the feature id.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [TypeConverter(typeof(FeatureExtensionsTypeConverter))]
        [Editor(typeof(StandardValuesEditor), typeof(UITypeEditor))]
        [DisplayNameResource("InstantiateFeatureCommand_FeatureId_DisplayName", typeof(Resources))]
        [DescriptionResource("InstantiateFeatureCommand_FeatureId_Description", typeof(Resources))]
        public string FeatureId { get; set; }

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
        /// Gets or sets the feature extension manager.
        /// </summary>
        [Required]
        [Import]
        public IFeatureManager FeatureManager { get; set; }

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
                this.CurrentElement.InstanceName, this.FeatureId, this.DefaultInstanceName, this.SharedInstance, this.ActivateOnInstantiation);

            // Ensure the feature type exists
            var featureRegistration =
                this.FeatureManager.InstalledFeatures.FirstOrDefault(
                    feature => feature.FeatureId.Equals(this.FeatureId, StringComparison.OrdinalIgnoreCase));
            if (featureRegistration != null)
            {
                string instanceName = string.Empty;

                // Ensure we are not sharing
                var sharedInstance = this.FeatureManager.InstantiatedFeatures.FirstOrDefault(f => f.FeatureId.Equals(this.FeatureId, StringComparison.OrdinalIgnoreCase));
                if (this.SharedInstance && sharedInstance != null)
                {
                    instanceName = sharedInstance.InstanceName;

                    // Activate shared feature
                    if (this.ActivateOnInstantiation)
                    {
                        tracer.TraceInformation(
                            Resources.InstantiateFeatureCommand_TraceActivateShared, this.CurrentElement.InstanceName, instanceName);

                        this.FeatureManager.ActivateGuidanceInstance(this.ServiceProvider, sharedInstance);
                    }
                }
                else
                {
                    // Create a default name
                    instanceName = this.FeatureManager.GetUniqueInstanceName(featureRegistration.DefaultName);
                    if (!string.IsNullOrEmpty(this.DefaultInstanceName))
                    {
                        instanceName = this.FeatureManager.GetUniqueInstanceName(this.DefaultInstanceName);

                        tracer.TraceVerbose(
                            Resources.InstantiateFeatureCommand_TraceCreateUniqueInstanceName, this.CurrentElement.InstanceName, instanceName);
                    }

                    tracer.TraceInformation(
                        Resources.InstantiateFeatureCommand_TraceInstantiateNew, this.CurrentElement.InstanceName, this.FeatureId, instanceName);

                    // Instantiate the feature and activate it.
                    var feature = this.FeatureManager.Instantiate(this.FeatureId, instanceName);
                    if (this.ActivateOnInstantiation)
                    {
                        tracer.TraceInformation(
                            Resources.InstantiateFeatureCommand_TraceActivateNew, this.CurrentElement.InstanceName, instanceName);

                        this.FeatureManager.ActivateGuidanceInstance(this.ServiceProvider, feature);
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
                    Resources.InstantiateFeatureCommand_TraceFeatureNotFound, this.CurrentElement.InstanceName, this.FeatureId);
            }
        }
    }
}