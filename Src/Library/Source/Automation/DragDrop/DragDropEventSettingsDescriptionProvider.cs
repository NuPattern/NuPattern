using System;
using System.ComponentModel;
using System.Linq;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Design;
using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.Patterning.Extensibility.Binding;
using Microsoft.VisualStudio.Patterning.Library.Properties;

namespace Microsoft.VisualStudio.Patterning.Library.Automation
{
    /// <summary>
    /// Defines a type descriptor provider over <see cref="DragDropSettings"/>.
    /// </summary>
	public class DragDropEventSettingsDescriptionProvider: ElementTypeDescriptionProvider
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

                properties.ReplaceDescriptor<DragDropSettings, Guid>(
                    x => x.CommandId,
                    d => new SettingsReferencePropertyDescriptor<IDragDropSettings, ICommandSettings>(d, x => x.CommandId));
                properties.ReplaceDescriptor<DragDropSettings, Guid>(
                    x => x.WizardId,
                    d => new SettingsReferencePropertyDescriptor<IDragDropSettings, IWizardSettings>(d, x => x.WizardId));
                properties.ReplaceDescriptor<DragDropSettings, string>(
                    x => x.DropConditions,
                    d => new CollectionPropertyDescriptor<ConditionBindingSettings>(d, Resources.SettingsDescriptionProvider_ConditionsEditorCaption));

                return new PropertyDescriptorCollection(properties.ToArray());
            }
        }
	}
}
