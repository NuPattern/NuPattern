using System;
using System.ComponentModel.Composition;
using System.Linq;
using NuPattern.Runtime.Guidance;

namespace NuPattern.Authoring.PatternToolkit.Guidance.Links
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
        public InstantiateHolLink(IGuidanceManager guidanceManager)
            : base(guidanceManager, CommandBindingName)
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
        public override bool CanExecute(IGuidanceExtension extension)
        {
            Guard.NotNull(() => extension, extension);

            return extension.GuidanceManager.InstalledGuidanceExtensions.Any(f => f.ExtensionId.Equals(HandsOnLabsToolkitInfo.Identifier, StringComparison.OrdinalIgnoreCase));
        }
    }
}