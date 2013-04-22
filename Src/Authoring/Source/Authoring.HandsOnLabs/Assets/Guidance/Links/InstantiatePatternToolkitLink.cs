using System;
using System.ComponentModel.Composition;
using NuPattern.Runtime.Guidance;
using NuPattern.Runtime.Guidance.LaunchPoints;

namespace NuPattern.Authoring.HandsOnLabs.Guidance
{
    /// <summary>
    /// Link launch point for creating a new Pattern Toolkit project.
    /// </summary>
    /// <remarks>This command is invoked using a link with this type of syntax: 
    /// launchpoint://{HOLToolkitInfo.Identifier}/InstantiatePatternToolkit</remarks>
    [CLSCompliant(false)]
    [LaunchPoint(Id = "InstantiatePatternToolkit")]
    public class InstantiatePatternToolkitLink : LinkLaunchPoint
    {
        internal const string CommandBindingName = "InstantiatePatternToolkit";

        /// <summary>
        /// Creates a new instance of the <see cref="InstantiatePatternToolkitLink"/> class.
        /// </summary>
        [ImportingConstructor]
        public InstantiatePatternToolkitLink(IGuidanceManager guidanceManager)
            : base(guidanceManager)
        {
        }

        /// <summary>
        /// Gets the binding name.
        /// </summary>
        /// <remarks>
        /// Must match a CommandBinding.Name property returned by the GuidanceExtension.Commands property of another guidance extension.
        /// </remarks>
        protected override string BindingName
        {
            get { return CommandBindingName; }
        }

        /// <summary>
        /// Determines whether the link can be executed.
        /// </summary>
        public override bool CanExecute(IGuidanceExtension extension)
        {
            return true;
        }
    }
}