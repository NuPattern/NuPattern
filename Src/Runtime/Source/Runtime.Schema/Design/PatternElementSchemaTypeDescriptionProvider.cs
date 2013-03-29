using System;
using System.ComponentModel;
using System.Linq;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Design;
using NuPattern.ComponentModel;
using NuPattern.Runtime.Bindings;
using NuPattern.Runtime.Design;

namespace NuPattern.Runtime.Schema.Design
{
    /// <summary>
    /// PropertySchemaType descriptor provider that overrides PropertySchema.DefaultValue type to the PropertySchema.Type type.
    /// </summary>
    internal class PatternElementSchemaTypeDescriptorProvider : ElementTypeDescriptionProvider
    {
        /// <summary>
        /// Overridables for the derived class to provide a custom type descriptor.
        /// </summary>
        /// <param name="parent">Parent custom type descriptor.</param>
        /// <param name="element">Element to be described.</param>
        protected override ElementTypeDescriptor CreateTypeDescriptor(ICustomTypeDescriptor parent, ModelElement element)
        {
            return new PatternElementSchemaTypeDescriptor(parent, element);
        }
    }

    /// <summary>
    /// PropertySchema type descriptor.
    /// </summary>
    internal class PatternElementSchemaTypeDescriptor : CustomizableElementTypeDescriptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PatternElementSchemaTypeDescriptor"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="modelElement">The model element.</param>
        public PatternElementSchemaTypeDescriptor(ICustomTypeDescriptor parent, ModelElement modelElement)
            : base(parent, modelElement)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PatternElementSchemaTypeDescriptor"/> class.
        /// </summary>
        /// <param name="modelElement">The model element.</param>
        public PatternElementSchemaTypeDescriptor(ModelElement modelElement)
            : base(modelElement)
        {
        }

        public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            var properties = base.GetProperties(attributes).Cast<PropertyDescriptor>().ToList();

            // Add descriptor for validationrules collection
            properties.ReplaceDescriptor<PatternElementSchema, string>(
                p => p.ValidationRules,
                x => new StringCollectionPropertyDescriptor<ValidationBindingSettings>(x));

            return new PropertyDescriptorCollection(properties.ToArray());
        }
    }
}