using System;
using System.ComponentModel;

namespace Microsoft.VisualStudio.Patterning.Extensibility.Serialization
{
    /// <summary>
    /// A type converter that converts to and from a Json string serialization of a value 
    /// of the given <typeparamref name="TValue"/>.
    /// </summary>
    public class JsonExpandableTypeConverter<TValue> : JsonTypeConverter<TValue>
        where TValue : new()
    {
        /// <summary>
        /// Returns whether this object supports properties.
        /// </summary>
        /// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
        /// <returns>true if System.ComponentModel.TypeConverter.GetProperties(System.Object)
        ///     should be called to find the properties of this object; otherwise, false.</returns>
        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        /// <summary>
        /// Returns a collection of properties for the type of array specified by the
        ///     value parameter.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="value">An <see cref="T:System.Object"/> that specifies the type of array for which to get properties.</param>
        /// <param name="attributes">An array of type <see cref="T:System.Attribute"/> that is used as a filter.</param>
        /// <returns></returns>
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            return TypeDescriptor.GetProperties(value, attributes);
        }
    }
}
