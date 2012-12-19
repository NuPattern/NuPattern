using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;

namespace NuPattern.Authoring.Authoring.Guidance.Links
{
    /// <summary>
    /// Link launch point for instantiating the Using guidance.
    /// </summary>
    /// <remarks>This command is invoked using a link with this type of syntax: 
    /// launchpoint://{ToolkitInfo.Identifier}/InstantiateUsingGudiance</remarks>
    [CLSCompliant(false)]
    [LaunchPoint(Id = "InstantiateUsingGudiance")]
    public class InstantiateUsingGuidanceLink : GuidanceLinkBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InstantiateUsingGuidanceLink"/> class.
        /// </summary>
        [ImportingConstructor]
        public InstantiateUsingGuidanceLink(IFeatureManager featureManager)
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
                return "InstantiateUsingGuidanceCommandBinding";
            }
        }
    }
}