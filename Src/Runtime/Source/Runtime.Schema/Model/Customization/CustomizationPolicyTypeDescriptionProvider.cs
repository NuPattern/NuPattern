using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Design;
using NuPattern.Reflection;

namespace NuPattern.Runtime.Schema
{
    /// <summary>
    /// Provides a custom type descriptor for the <see cref="CustomizationPolicySchema" /> class. 
    /// </summary>
    internal class CustomizationPolicyTypeDescriptionProvider : ElementTypeDescriptionProvider
    {
        /// <summary>
        /// Returns an instance of a type descriptor for the given instance of the class.
        /// </summary>
        protected override ElementTypeDescriptor CreateTypeDescriptor(ICustomTypeDescriptor parent, ModelElement element)
        {
            if (element is CustomizationPolicySchema)
            {
                return new CustomizationPolicyTypeDescriptor(parent, element);
            }

            return base.CreateTypeDescriptor(parent, element);
        }
    }

    /// <summary>
    /// Provides the custom type descriptor for <see cref="CustomizableElementSchema"/> class, 
    /// that displays customization properties depending on the state of the customizable element.
    /// </summary>
    internal class CustomizationPolicyTypeDescriptor : ElementTypeDescriptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomizationPolicyTypeDescriptor"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="modelElement">The model element.</param>
        public CustomizationPolicyTypeDescriptor(ICustomTypeDescriptor parent, ModelElement modelElement)
            : base(parent, modelElement)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomizationPolicyTypeDescriptor"/> class.
        /// </summary>
        /// <param name="modelElement">The model element.</param>
        public CustomizationPolicyTypeDescriptor(ModelElement modelElement)
            : base(modelElement)
        {
        }

        /// <summary>
        /// Returns the properties for customization that reflect the current state of the class.
        /// </summary>
        public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            var properties = base.GetProperties(attributes).Cast<PropertyDescriptor>();
            var policy = (CustomizationPolicySchema)this.ModelElement;

            return ApplyDesignPolicy(policy, properties);
        }

        /// <summary>
        /// Applies the design-time policy to the given policy properties.
        /// </summary>
        private static PropertyDescriptorCollection ApplyDesignPolicy(CustomizationPolicySchema policy, IEnumerable<PropertyDescriptor> properties)
        {
            if (policy.Owner.IsCustomizationEnabled)
            {
                // Add the available customizable settings.
                properties = properties.Concat(
                    policy.Settings
                        .Select(setting =>
                            new ElementPropertyDescriptor(
                                setting,
                                setting.GetDomainClass().FindDomainProperty(Reflector<CustomizableSettingSchema>.GetProperty(elem => elem.Value).Name, true),
                                new Attribute[] 
                            {
                                //// Makes the property appear with the designated caption and description.
                                new DisplayNameAttribute(setting.Caption), 
                                new DescriptionAttribute(setting.Description), 
                                //// Makes the property bold if it's modified from its default value, that is, IsModified=true.
                                new DefaultValueAttribute(setting.DefaultValue),
                                new ReadOnlyAttribute(IsSettingReadOnly(setting)),
                            })));

                //// IsModified is already UI readonly in the DSL
            }
            else
            {
                //// Remove IsModified
                properties = properties.Where(property => property.Name != Reflector<CustomizationPolicySchema>.GetProperty(elem => elem.IsModified).Name);
            }

            return new PropertyDescriptorCollection(properties.ToArray());
        }

        private static bool IsSettingReadOnly(CustomizableSettingSchema setting)
        {
            if (setting.Policy.Owner.IsCustomizationPolicyModifyable == false)
            {
                return true;
            }
            else
            {
                if (setting.IsEnabled == false)
                {
                    return true;
                }
            }

            return false;
        }
    }
}