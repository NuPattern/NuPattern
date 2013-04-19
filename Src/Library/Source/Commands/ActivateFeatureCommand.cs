using System;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.VisualStudio.Shell;
using NuPattern.ComponentModel.Design;
using NuPattern.Diagnostics;
using NuPattern.Library.Properties;
using NuPattern.Runtime;
using NuPattern.Runtime.Guidance;
using NuPattern.Runtime.References;

namespace NuPattern.Library.Commands
{
    /// <summary>
    /// Command that activates an existing feature.
    /// </summary>
    [DisplayNameResource("ActivateFeatureCommand_DisplayName", typeof(Resources))]
    [CategoryResource("AutomationCategory_Guidance", typeof(Resources))]
    [DescriptionResource("ActivateFeatureCommand_Description", typeof(Resources))]
    [CLSCompliant(false)]
    public class ActivateFeatureCommand : Command
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<ActivateFeatureCommand>();

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
                Resources.ActivateFeatureCommand_TraceInitial, this.CurrentElement.InstanceName);

            // Get the guidance reference from current element
            var instanceName = this.CurrentElement.TryGetReference(ReferenceKindConstants.Guidance);
            if (!String.IsNullOrEmpty(instanceName))
            {
                tracer.TraceVerbose(
                    Resources.ActivateFeatureCommand_TraceReferenceFound, this.CurrentElement.InstanceName, instanceName);

                // Get associated feature (if exists in solution)
                var featureInstance = GuidanceReference.GetResolvedReferences(this.CurrentElement, this.GuidanceManager).FirstOrDefault();
                if (featureInstance != null)
                {
                    tracer.TraceInformation(
                        Resources.ActivateFeatureCommand_TraceActivation, this.CurrentElement.InstanceName, instanceName);

                    this.GuidanceManager.ActivateGuidanceInstance(this.ServiceProvider, featureInstance);
                }
                else
                {
                    tracer.TraceWarning(
                        Resources.ActivateFeatureCommand_TraceGuidanceNotFound, instanceName);
                }
            }
            else
            {
                tracer.TraceWarning(
                    Resources.ActivateFeatureCommand_TraceNoReference, this.CurrentElement.InstanceName);
            }
        }
    }
}