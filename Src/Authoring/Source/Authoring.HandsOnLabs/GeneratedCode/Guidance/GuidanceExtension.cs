using System.Collections.Generic;
using NuPattern.Library.Commands;
using NuPattern.Reflection;
using NuPattern.Runtime.Bindings;
using NuPattern.Runtime.Guidance;

namespace NuPattern.Authoring.HandsOnLabs.Guidance
{
    public partial class GuidanceExtension
    {
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
                            typeof(InstantiateSolutionElementCommand).FullName,
                            new FixedValuePropertyBinding(Reflect<InstantiateSolutionElementCommand>.GetProperty(t => t.ToolkitIdentifier).Name, AuthoringToolkitInfo.VsixIdentifier), 
                            new FixedValuePropertyBinding(Reflect<InstantiateSolutionElementCommand>.GetProperty(t => t.InstanceName).Name, WidgetPatternToolkitName)) 
                        {
                            Name = InstantiatePatternToolkitLink.CommandBindingName
                        }
                    };
                }

                return this.commands;
            }
        }
    }
}
