using System;
using System.ComponentModel.Composition;
using NuPattern.Runtime.Guidance;
using NuPattern.Runtime.Guidance.LaunchPoints;

namespace NuPattern.Authoring.PatternToolkit.Guidance.Links
{
    /// <summary>
    /// Link launch point base class
    /// </summary>
    /// <remarks>This command is invoked using a link with this type of syntax: 
    /// launchpoint://{ToolkitInfo.Identifier}/{LinkName}</remarks>
    [CLSCompliant(false)]
    public abstract class GuidanceLinkBase : LinkLaunchPoint
    {
        private string bindingName;

        /// <summary>
        /// Initializes a new instance of the <see cref=" GuidanceLinkBase"/> class.
        /// </summary>
        [ImportingConstructor]
        protected GuidanceLinkBase(IGuidanceManager guidanceManager, string bindingName)
            : base(guidanceManager)
        {
            Guard.NotNull(() => bindingName, bindingName);

            this.bindingName = bindingName;
        }

        /// <summary>
        /// Determines if the link can be executed. Overrides the default implementation that checks
        /// if the current workflow node is in the enabled state.
        /// </summary>
        public override bool CanExecute(IGuidanceExtension feature)
        {
            return true;
        }

        /// <summary>
        /// Gets the binding name. Must match a CommandBinding.Name property returned by the Feature.Commands property.
        /// </summary>
        protected override string BindingName
        {
            get
            {
                return this.bindingName;
            }
        }
    }
}