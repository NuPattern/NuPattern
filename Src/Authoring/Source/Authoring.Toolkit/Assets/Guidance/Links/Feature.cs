using System.Collections.Generic;
using Microsoft.VisualStudio.Patterning.Authoring.Authoring.Guidance.Links;
using Microsoft.VisualStudio.Patterning.Authoring.Authoring.Properties;
using Microsoft.VisualStudio.Patterning.Library.Commands;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;

namespace Microsoft.VisualStudio.Patterning.Authoring.Authoring.Guidance
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
							new FixedValuePropertyBinding(Reflect<ActivateOrInstantiateSharedFeatureCommand>.GetProperty(t => t.FeatureId).Name, RuntimeShellInfo.Identifier))
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