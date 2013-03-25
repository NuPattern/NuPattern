using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Design;
using NuPattern.ComponentModel.Design;
using NuPattern.Reflection;
using NuPattern.Runtime.Design;

namespace NuPattern.Runtime.Schema
{
    /// <summary>
    /// Provides a custom type descriptor for the <see cref="NamedElementSchema" /> class. 
    /// </summary>
    internal class NamedElementTypeDescriptionProvider : ElementTypeDescriptionProvider
    {
        /// <summary>
        /// Returns an instance of a type descriptor for the given instance of the class.
        /// </summary>
        protected override ElementTypeDescriptor CreateTypeDescriptor(ICustomTypeDescriptor parent, ModelElement element)
        {
            if (element is NamedElementSchema)
            {
                return new NamedElementTypeDescriptor(parent, element);
            }

            return base.CreateTypeDescriptor(parent, element);
        }
    }
    /// <summary>
    /// Provides the custom type descriptor for <see cref="NamedElementSchema"/> class.
    /// </summary>
    internal class NamedElementTypeDescriptor : PropertyDescriptorTypeDescriptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NamedElementTypeDescriptor"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="modelElement">The model element.</param>
        public NamedElementTypeDescriptor(ICustomTypeDescriptor parent, ModelElement modelElement)
            : base(parent, modelElement)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedElementTypeDescriptor"/> class.
        /// </summary>
        /// <param name="modelElement">The model element.</param>
        public NamedElementTypeDescriptor(ModelElement modelElement)
            : base(modelElement)
        {
        }

        /// <summary>
        /// Returns the properties for customization that reflect the current state of the class.
        /// </summary>
        public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            var properties = base.GetProperties(attributes);
            var element = (NamedElementSchema)this.ModelElement;

            // Add tracking property descriptors
            element.ReplaceTrackingPropertyDescriptors(properties);

            return ApplyDesignPolicy(element, properties.Cast<PropertyDescriptor>());
        }

        /// <summary>
        /// Applies the design-time policy to the given element properties.
        /// </summary>
        private static PropertyDescriptorCollection ApplyDesignPolicy(NamedElementSchema element, IEnumerable<PropertyDescriptor> properties)
        {
            var descriptors = properties.ToDictionary(property => property.Name);

            if (element.IsInheritedFromBase)
            {
                // Add read-only attribute to the Name property of the element
                var nameProperty = descriptors[Reflector<CustomizableElementSchema>.GetProperty(elem => elem.Name).Name];
                descriptors[nameProperty.Name] = new DelegatingPropertyDescriptor(nameProperty, new ReadOnlyAttribute(true));
            }

            return new PropertyDescriptorCollection(descriptors.Values.ToArray());
        }
    }
}