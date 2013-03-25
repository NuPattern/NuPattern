using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Design;
using NuPattern.ComponentModel;
using NuPattern.ComponentModel.Design;
using NuPattern.Reflection;
using NuPattern.Runtime.Bindings;
using NuPattern.Runtime.Design;
using NuPattern.Runtime.Schema.Properties;

namespace NuPattern.Runtime.Schema
{
    /// <summary>
    /// PropertySchemaType descriptor provider that overrides PropertySchema.DefaultValue type to the PropertySchema.Type type.
    /// </summary>
    internal class PropertySchemaTypeDescriptorProvider : ElementTypeDescriptionProvider
    {
        /// <summary>
        /// Overridables for the derived class to provide a custom type descriptor.
        /// </summary>
        /// <param name="parent">Parent custom type descriptor.</param>
        /// <param name="element">Element to be described.</param>
        protected override ElementTypeDescriptor CreateTypeDescriptor(ICustomTypeDescriptor parent, ModelElement element)
        {
            return new PropertySchemaTypeDescriptor(parent, element);
        }

        /// <summary>
        /// PropertySchema type descriptor.
        /// </summary>
        private class PropertySchemaTypeDescriptor : CustomizableElementTypeDescriptor
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="PropertySchemaTypeDescriptor"/> class.
            /// </summary>
            /// <param name="parent">The parent.</param>
            /// <param name="modelElement">The model element.</param>
            public PropertySchemaTypeDescriptor(ICustomTypeDescriptor parent, ModelElement modelElement)
                : base(parent, modelElement)
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="PropertySchemaTypeDescriptor"/> class.
            /// </summary>
            /// <param name="modelElement">The model element.</param>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
            public PropertySchemaTypeDescriptor(ModelElement modelElement)
                : base(modelElement)
            {
            }

            public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
            {
                var properties = base.GetProperties(attributes).Cast<PropertyDescriptor>().ToList();

                // Add descriptor for Type
                properties.ReplaceDescriptor<PropertySchema, string>(
                    p => p.Type,
                    x => new ValueTypesPropertyDescriptor(x));

                // Add descriptor for validation rules collection
                properties.ReplaceDescriptor<PropertySchema, string>(
                    p => p.RawValidationRules,
                    x => new StringCollectionPropertyDescriptor<ValidationBindingSettings>(x));

                PrepareDefaultValue(properties);

                var descriptors = properties.ToDictionary(prop => prop.Name);

                var property = (PropertySchema)this.ModelElement;
                if (property.IsInheritedFromBase)
                {
                    // Add read-only attribute to the Type property of the element
                    var typeProperty = descriptors[Reflector<PropertySchema>.GetProperty(elem => elem.Type).Name];
                    descriptors[typeProperty.Name] = new DelegatingPropertyDescriptor(typeProperty, new ReadOnlyAttribute(true));
                }

                return new PropertyDescriptorCollection(descriptors.Values.ToArray());
            }

            private void PrepareDefaultValue(System.Collections.Generic.List<PropertyDescriptor> properties)
            {
                var propertySchema = (PropertySchema)this.ModelElement;
                var propertyName = Reflector<PropertySchema>.GetPropertyName(p => p.RawDefaultValue);
                var propertyType = Type.GetType(propertySchema.Type);

                if (propertyType == null)
                {
                    // DefaultValue property cannot be edited, so we make it read only.
                    properties.ReplaceDescriptor<PropertySchema, string>(
                        p => p.RawDefaultValue,
                        x => new DelegatingPropertyDescriptor(x,
                            new ReadOnlyAttribute(true),
                            new DescriptionAttribute(string.Format(
                                CultureInfo.CurrentCulture,
                                Resources.PropertySchema_FailedToLoadPropertyType,
                                propertySchema.Type,
                                propertySchema.Name))));
                }
                else
                {
                    properties.ReplaceDescriptor<PropertySchema, string>(
                        p => p.RawDefaultValue,
                        x => new DefaultValuePropertyDescriptor(propertyName, propertySchema, propertyType, x.Attributes.OfType<Attribute>().ToArray()));
                }
            }
        }
    }
}