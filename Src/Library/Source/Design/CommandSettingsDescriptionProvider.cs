using System;
using System.ComponentModel;
using System.Linq;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Design;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using NuPattern.ComponentModel.Composition;
using NuPattern.Library.Automation;
using NuPattern.Reflection;
using NuPattern.Runtime;
using NuPattern.Runtime.Bindings.Design;

namespace NuPattern.Library.Design
{
    /// <summary>
    /// Defines a <see cref="TypeDescriptionProvider"/> over <see cref="CommandSettings"/>.
    /// </summary>
    internal class CommandSettingsDescriptionProvider : ElementTypeDescriptionProvider
    {
        /// <summary>
        /// Overridables for the derived class to provide a custom type descriptor.
        /// </summary>
        /// <param name="parent">Parent custom type descriptor.</param>
        /// <param name="element">Element to be described.</param>
        protected override ElementTypeDescriptor CreateTypeDescriptor(ICustomTypeDescriptor parent, ModelElement element)
        {
            return new CommandSettingsTypeDescriptor(parent, element);
        }

        /// <summary>
        /// Defines a type descriptor over <see cref="CommandSettings"/>.
        /// </summary>
        internal class CommandSettingsTypeDescriptor : ElementTypeDescriptor
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="CommandSettingsTypeDescriptor"/> class.
            /// </summary>
            /// <param name="parent">The parent.</param>
            /// <param name="selectedElement">The selected element.</param>
            public CommandSettingsTypeDescriptor(ICustomTypeDescriptor parent, ModelElement selectedElement)
                : base(parent, selectedElement)
            {
            }

            /// <summary>
            /// Returns the properties for this instance of a component using the attribute array as a filter.
            /// </summary>
            /// <param name="attributes">An array of type Attribute that is used as a filter.</param>
            /// <returns>
            /// An array of type Attribute that represents the properties for this component instance that match the given set of attributes.
            /// </returns>
            public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
            {
                var properties = base.GetProperties(attributes).Cast<PropertyDescriptor>().ToList();

                var commandSettings = (CommandSettings)this.ModelElement;
                if (commandSettings != null)
                {
                    var projectTypeProvider = commandSettings.Store.GetService<INuPatternProjectTypeProvider>();
                    var components = commandSettings.Store.GetService<IFeatureCompositionService>()
                                       .GetExports<IFeatureCommand, IFeatureComponentMetadata>();

                    // Remove the descriptor for the 'Properties 'property
                    properties.Remove(properties
                        .First(descriptor => descriptor.Name == Reflector<CommandSettings>.GetPropertyName(x => x.Properties)));

                    // Add CommandSettings properties
                    properties.AddRange(DesignComponentTypeDescriptor<IFeatureCommand, CommandSettings>.GetComponentProperties(
                        projectTypeProvider, components, commandSettings.TypeId));
                }

                return new PropertyDescriptorCollection(properties.ToArray());
            }
        }
    }
}