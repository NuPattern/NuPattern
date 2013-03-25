using System;
using System.ComponentModel;
using System.Linq;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Design;
using NuPattern.ComponentModel;
using NuPattern.Extensibility.Bindings;
using NuPattern.Extensibility.Design;

namespace NuPattern.Library.Automation
{
    /// <summary>
    /// Defines a type descriptor provider over <see cref="EventSettings"/>.
    /// </summary>
    internal class EventSettingsDescriptionProvider : ElementTypeDescriptionProvider
    {
        /// <summary>
        /// Overridables for the derived class to provide a custom type descriptor.
        /// </summary>
        /// <param name="parent">Parent custom type descriptor.</param>
        /// <param name="element">Element to be described.</param>
        protected override ElementTypeDescriptor CreateTypeDescriptor(ICustomTypeDescriptor parent, ModelElement element)
        {
            return new EventSettingsTypeDescriptor(element);
        }

        /// <summary>
        /// Event Launch Point Settings Type Descriptor.
        /// </summary>
        private class EventSettingsTypeDescriptor : ElementTypeDescriptor
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="EventSettingsTypeDescriptor"/> class.
            /// </summary>
            public EventSettingsTypeDescriptor(ModelElement modelElement)
                : base(modelElement)
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

                properties.ReplaceDescriptor<EventSettings, Guid>(
                    x => x.CommandId,
                    d => new SettingsReferencePropertyDescriptor<IEventSettings, ICommandSettings>(d, x => x.CommandId));
                properties.ReplaceDescriptor<EventSettings, Guid>(
                    x => x.WizardId,
                    d => new SettingsReferencePropertyDescriptor<IEventSettings, IWizardSettings>(d, x => x.WizardId));
                properties.ReplaceDescriptor<EventSettings, string>(
                    x => x.Conditions,
                    d => new StringCollectionPropertyDescriptor<ConditionBindingSettings>(d));

                return new PropertyDescriptorCollection(properties.ToArray());
            }
        }
    }
}