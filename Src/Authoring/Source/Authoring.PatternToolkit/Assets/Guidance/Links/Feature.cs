using System.Collections.Generic;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using NuPattern.Authoring.PatternToolkit.Guidance.Links;
using NuPattern.Authoring.PatternToolkit.Properties;
using NuPattern.Library.Commands;

namespace NuPattern.Authoring.PatternToolkit.Guidance
{
    public partial class Feature
    {
        private CommandBinding[] commands;

        /// <summary>
        /// Gets the Commands property to provide a list of Feature Commands used in the guidance.
        /// </summary>
        public override IEnumerable<CommandBinding> Commands
        {
            get
            {
                if (this.commands == null)
                {
                    this.commands = new CommandBinding[]
                    {
                        new CommandBinding(
                            this.FeatureComposition, 
                            typeof(InstantiateSolutionElementCommand).FullName,
                            new FixedValuePropertyBinding(Reflect<InstantiateSolutionElementCommand>.GetProperty(t => t.ToolkitIdentifier).Name, HandsOnLabsToolkitInfo.Identifier), 
                            new FixedValuePropertyBinding(Reflect<InstantiateSolutionElementCommand>.GetProperty(t => t.InstanceName).Name, Resources.InstantiateHolLink_InstanceName))
                        {
                            Name = InstantiateHolLink.CommandBindingName
                        },
                        new CommandBinding(
                            this.FeatureComposition, 
                            typeof(ActivateOrInstantiateSharedFeatureCommand).FullName,
                            new FixedValuePropertyBinding(Reflect<ActivateOrInstantiateSharedFeatureCommand>.GetProperty(t => t.FeatureId).Name, RuntimeShellInfo.VsixIdentifier))
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