using System;
using System.ComponentModel;
using System.Linq;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Design;
using NuPattern.Extensibility;
using NuPattern.Extensibility.Binding;

namespace NuPattern.Library.Automation
{
    /// <summary>
    /// Defines a type descriptor provider over <see cref="MenuSettings"/>.
    /// </summary>
    public class MenuSettingsDescriptionProvider : ElementTypeDescriptionProvider
    {
        /// <summary>
        /// Overridables for the derived class to provide a custom type descriptor.
        /// </summary>
        /// <param name="parent">Parent custom type descriptor.</param>
        /// <param name="element">Element to be described.</param>
        protected override ElementTypeDescriptor CreateTypeDescriptor(ICustomTypeDescriptor parent, ModelElement element)
        {
            return new MenuSettingsTypeDescriptor(element);
        }

        /// <summary>
        /// Menu Settings Type Descriptor.
        /// </summary>
        private class MenuSettingsTypeDescriptor : ElementTypeDescriptor
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="MenuSettingsTypeDescriptor"/> class.
            /// </summary>
            public MenuSettingsTypeDescriptor(ModelElement modelElement)
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

                properties.ReplaceDescriptor<MenuSettings, Guid>(
                    x => x.CommandId,
                    d => new SettingsReferencePropertyDescriptor<IMenuSettings, ICommandSettings>(d, x => x.CommandId));
                properties.ReplaceDescriptor<MenuSettings, Guid>(
                    x => x.WizardId,
                    d => new SettingsReferencePropertyDescriptor<IMenuSettings, IWizardSettings>(d, x => x.WizardId));
                properties.ReplaceDescriptor<MenuSettings, string>(
                    x => x.Conditions,
                    d => new StringCollectionPropertyDescriptor<ConditionBindingSettings>(d));

                return new PropertyDescriptorCollection(properties.ToArray());
            }
        }
    }
}