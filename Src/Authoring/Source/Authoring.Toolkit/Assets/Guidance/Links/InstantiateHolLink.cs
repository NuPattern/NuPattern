using System;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;

namespace NuPattern.Authoring.Authoring.Guidance.Links
{
    /// <summary>
    /// Link launch point for instantiating the Hands-On Lab guidance.
    /// </summary>
    /// <remarks>This command is invoked using a link with this type of syntax: 
    /// launchpoint://{ToolkitInfo.Identifier}/InstantiateHolToolkit</remarks>
    [CLSCompliant(false)]
    [LaunchPoint(Id = "InstantiateHolToolkit")]
    public class InstantiateHolLink : GuidanceLinkBase
    {
        /// <summary>
        /// Initializes a new instance of the InstantiateHol class.
        /// </summary>
        [ImportingConstructor]
        public InstantiateHolLink(IFeatureManager featureManager)
            : base(featureManager, CommandBindingName)
        {
        }

        /// <summary>
        /// Gets the binding name
        /// </summary>
        internal static string CommandBindingName
        {
            get
            {
                return "InstantiateHolCommandBinding";
            }
        }

        /// <summary>
        /// Determines if the link can execute
        /// </summary>
        public override bool CanExecute(IFeatureExtension feature)
        {
            Guard.NotNull(() => feature, feature);

            return feature.FeatureManager.InstalledFeatures.Any(f => f.FeatureId.Equals(HandsOnLabsToolkitInfo.Identifier, StringComparison.OrdinalIgnoreCase));
        }
    }
}