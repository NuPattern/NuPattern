using System.Collections.Generic;
using NuPattern.Runtime.Bindings;
using NuPattern.Runtime.Guidance;

namespace NuPattern.Authoring.HandsOnLabs.Guidance
{
    public partial class GuidanceExtension
    {
        private const string PatternToolkitId = AuthoringToolkitInfo.VsixIdentifier;
        private const string WidgetPatternToolkitName = "WidgetToolkit";
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
