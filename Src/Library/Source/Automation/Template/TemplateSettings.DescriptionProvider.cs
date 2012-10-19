using System;
using System.ComponentModel;
using System.Linq;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Design;
using Microsoft.VisualStudio.Patterning.Extensibility;

namespace Microsoft.VisualStudio.Patterning.Library.Automation
{
    /// <summary>
    /// Defines a type descriptor provider over <see cref="EventSettings"/>.
    /// </summary>
    public class TemplateSettingsDescriptionProvider : ElementTypeDescriptionProvider
    {
        /// <summary>
        /// Overridables for the derived class to provide a custom type descriptor.
        /// </summary>
        /// <param name="parent">Parent custom type descriptor.</param>
        /// <param name="element">Element to be described.</param>
        protected override ElementTypeDescriptor CreateTypeDescriptor(ICustomTypeDescriptor parent, ModelElement element)
        {
            return new TemplateSettingsTypeDescriptor(element);
        }

        /// <summary>
        /// Event Launch Point Settings Type Descriptor.
        /// </summary>
        private class TemplateSettingsTypeDescriptor : PropertyDescriptorTypeDescriptor
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="TemplateSettingsTypeDescriptor"/> class.
            /// </summary>
            public TemplateSettingsTypeDescriptor(ModelElement modelElement)
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

                properties.ReplaceDescriptor<ITemplateSettings, Guid>(
                    t => t.CommandId,
                    d => new SettingsReferencePropertyDescriptor<ITemplateSettings, ICommandSettings>(d, t => t.CommandId));
                properties.ReplaceDescriptor<ITemplateSettings, Guid>(
                    t => t.WizardId,
                    d => new SettingsReferencePropertyDescriptor<ITemplateSettings, IWizardSettings>(d, t => t.WizardId));

                return new PropertyDescriptorCollection(properties.ToArray());
            }
        }
    }
}