using System.Collections.Generic;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;

namespace Microsoft.VisualStudio.Patterning.HandsOnLabs.Guidance
{
    public partial class Feature
    {
        private const string PatternToolkitId = "9f6dc301-6f66-4d21-9f9c-b37412b162f6";
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
	                        "Microsoft.VisualStudio.Patterning.Library.Commands.InstantiateSolutionElementCommand",
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
