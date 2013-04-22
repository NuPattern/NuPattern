using System.Collections.Generic;
using NuPattern.Authoring.PatternToolkit.Guidance.Links;
using NuPattern.Authoring.PatternToolkit.Properties;
using NuPattern.Library.Commands;
using NuPattern.Reflection;
using NuPattern.Runtime.Bindings;
using NuPattern.Runtime.Guidance;

namespace NuPattern.Authoring.PatternToolkit.Guidance
{
    public partial class GuidanceExtension
    {
        private ICommandBinding[] commands;

        /// <summary>
        /// Gets the Commands property to provide a list of Feature Commands used in the guidance.
        /// </summary>
        public override IEnumerable<ICommandBinding> Commands
        {
            get
            {
                if (this.commands == null)
                {
                    this.commands = new CommandBinding[]
                    {
                        new CommandBinding(
                            this.GuidanceComposition, 
                            typeof(InstantiateSolutionElementCommand).FullName,
                            new FixedValuePropertyBinding(Reflect<InstantiateSolutionElementCommand>.GetProperty(t => t.ToolkitIdentifier).Name, HandsOnLabsToolkitInfo.Identifier), 
                            new FixedValuePropertyBinding(Reflect<InstantiateSolutionElementCommand>.GetProperty(t => t.InstanceName).Name, Resources.InstantiateHolLink_InstanceName))
                        {
                            Name = InstantiateHolLink.CommandBindingName
                        },
                        new CommandBinding(
                            this.GuidanceComposition, 
                            typeof(ActivateOrInstantiateSharedGuidanceWorkflowCommand).FullName,
                            new FixedValuePropertyBinding(Reflect<ActivateOrInstantiateSharedGuidanceWorkflowCommand>.GetProperty(t => t.ExtensionId).Name, RuntimeShellInfo.VsixIdentifier))
                        {
                            Name = InstantiateUsingGuidanceLink.CommandBindingName
                        },
                    };
                }

                return this.commands;
            }
        }
    }
}