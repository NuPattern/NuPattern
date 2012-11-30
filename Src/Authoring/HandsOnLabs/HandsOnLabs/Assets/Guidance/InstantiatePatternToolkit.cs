using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;

namespace Microsoft.VisualStudio.Patterning.HandsOnLabs.Guidance
{
    /// <summary>
    /// Link launch point for instantiating the Hands-On Lab guidance.
    /// </summary>
    /// <remarks>This command is invoked using a link with this type of syntax: 
    /// launchpoint://{vsixid}/InstantiatePatternToolkit</remarks>
    [CLSCompliant(false)]
    [LaunchPoint(Id = "InstantiatePatternToolkit")]
    public class InstantiatePatternToolkit : LinkLaunchPoint
    {
        /// <summary>
        /// Initializes a new instance of the InstantiateHol class.
        /// </summary>
        [ImportingConstructor]
        public InstantiatePatternToolkit(IFeatureManager featureManager)
            : base(featureManager)
        {
        }

        /// <summary>
        /// Determines if the link can be executed. Overrides the default implementation that checks
        /// if the current workflow node is in the enabled state.
        /// </summary>
        public override bool CanExecute(IFeatureExtension feature)
        {
            return true;
        }
        
        /// <summary>
        /// Gets the binding name. Must match a CommandBinding.Name property returned by the Feature.Commands property.
        /// </summary>
        protected override string BindingName
        {
            get { return "InstantiatePatternToolkit"; }
        }
    }
}