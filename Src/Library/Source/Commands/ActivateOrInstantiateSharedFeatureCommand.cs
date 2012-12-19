using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Drawing.Design;
using System.Linq;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Design;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using NuPattern.Extensibility;
using NuPattern.Library.Properties;

namespace NuPattern.Library.Commands
{
    /// <summary>
    /// Command that activates or instantiates a new feature.
    /// </summary>
    [DisplayNameResource("ActivateOrInstantiateSharedFeatureCommand_DisplayName", typeof(Resources))]
    [CategoryResource("AutomationCategory_Guidance", typeof(Resources))]
    [DescriptionResource("ActivateOrInstantiateSharedFeatureCommand_Description", typeof(Resources))]
    [CLSCompliant(false)]
    public class ActivateOrInstantiateSharedFeatureCommand : FeatureCommand
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<ActivateOrInstantiateSharedFeatureCommand>();

        /// <summary>
        /// Gets or sets the feature id.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [TypeConverter(typeof(FeatureExtensionsTypeConverter))]
        [Editor(typeof(StandardValuesEditor), typeof(UITypeEditor))]
        [DisplayNameResource("InstantiateFeatureCommand_FeatureId_DisplayName", typeof(Resources))]
        [DescriptionResource("ActivateFeatureCommand_FeatureId_Description", typeof(Resources))]
        public string FeatureId { get; set; }

        /// <summary>
        /// Gets or sets the feature extension manager.
        /// </summary>
        [Required]
        [Import]
        public IFeatureManager FeatureManager
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

            tracer.TraceInformation(
                Resources.ActivateOrInstantiateSharedFeatureCommand_TraceInitial, this.FeatureId);

            // Ensure the feature type exists
            var featureRegistration = this.FeatureManager.InstalledFeatures
                .FirstOrDefault(feature => feature.FeatureId.Equals(this.FeatureId, StringComparison.OrdinalIgnoreCase));
            if (featureRegistration != null)
            {
                // Show the guidance windows
                if (this.ServiceProvider != null)
                {
                    tracer.TraceVerbose(
                        Resources.ActivateOrInstantiateSharedFeatureCommand_TraceShowingGuidanceExplorer);

                    this.FeatureManager.ShowGuidanceWindows(this.ServiceProvider);
                }

                // Ensure we are not sharing
                var featureInstance = this.FeatureManager.InstantiatedFeatures.Where(f => f.FeatureId.Equals(this.FeatureId, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                if (featureInstance == null)
                {
                    // Create a default name
                    var instanceName = this.FeatureManager.GetUniqueInstanceName(featureRegistration.DefaultName);

                    tracer.TraceInformation(
                        Resources.ActivateOrInstantiateSharedFeatureCommand_TraceInstantiate, this.FeatureId, instanceName);

                    // Instantiate the feature
                    featureInstance = this.FeatureManager.Instantiate(this.FeatureId, instanceName);
                }

                tracer.TraceInformation(
                        Resources.ActivateOrInstantiateSharedFeatureCommand_TraceActivate, featureInstance.InstanceName);

                // Activate feature
                this.FeatureManager.ActiveFeature = featureInstance;
            }
            else
            {
                tracer.TraceError(
                    Resources.ActivateOrInstantiateSharedFeatureCommand_TraceFeatureNotFound, this.FeatureId);
            }
        }
    }
}
