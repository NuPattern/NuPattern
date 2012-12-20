using System.Collections.Generic;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;

namespace NuPattern.HandsOnLabs.Guidance
{
    public partial class Feature
    {
        private const string PatternToolkitId = AuthoringToolkitInfo.VsixIdentifier;
        private const string WidgetPatternToolkitName = "WidgetToolkit";
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
	                        "NuPattern.Library.Commands.InstantiateSolutionElementCommand",
	                        new FixedValuePropertyBinding("ToolkitIdentifier", PatternToolkitId), 
                            new FixedValuePropertyBinding("InstanceName", WidgetPatternToolkitName))
	                    {
	                        Name = "InstantiatePatternToolkit"
	                    }
	                };
                }

                return this.commands;
            }
        }
    }
}
