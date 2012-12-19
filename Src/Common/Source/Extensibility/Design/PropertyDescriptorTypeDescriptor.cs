using System;
using System.ComponentModel;
using System.Linq;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Design;

namespace NuPattern.Extensibility
{
    /// <summary>
    /// Provides a type descriptor for all elements that use <see cref="PropertyDescriptorAttribute"/>.
    /// </summary>
    public abstract class PropertyDescriptorTypeDescriptor : ElementTypeDescriptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyDescriptorTypeDescriptor"/> class.
        /// </summary>
        /// <param name="element">The model element.</param>
        public PropertyDescriptorTypeDescriptor(ModelElement element)
            : base(element)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyDescriptorTypeDescriptor"/> class.
        /// </summary>
        /// <param name="element">The model element.</param>
        /// <param name="parent">The parent descriptor</param>
        public PropertyDescriptorTypeDescriptor(ICustomTypeDescriptor parent, ModelElement element)
            : base(parent, element)
        {
        }

        /// <summary>
        /// Returns the properties that reflect the current element
        /// </summary>
        public override System.ComponentModel.PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            var properties = base.GetProperties(attributes).Cast<PropertyDescriptor>().ToDictionary(prop => prop.Name);

            // Wraps any property descriptor attributes properties 
            var processed = properties.Values.Select(p => p.ProcessPropertyDescriptorAttribute(this.ModelElement)).ToArray();

            return new PropertyDescriptorCollection(processed);
        }
    }
}
