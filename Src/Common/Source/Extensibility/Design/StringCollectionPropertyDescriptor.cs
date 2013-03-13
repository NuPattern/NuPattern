using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing.Design;
using System.Globalization;
using System.Linq;
using NuPattern.Extensibility.Binding;
using NuPattern.Extensibility.Properties;

namespace NuPattern.Extensibility
{
    /// <summary>
    /// Provides a <see cref="PropertyDescriptor"/> that manages a collection that is serialized as a string (JSON)
    /// </summary>
    /// <typeparam name="T">The type of the model to serialize/deserialize.</typeparam>
    [CLSCompliant(false)]
    public class StringCollectionPropertyDescriptor<T> : DelegatingPropertyDescriptor
    {
        /// <summary>
        /// Creates a new instance of the <see cref="StringCollectionPropertyDescriptor{T}"/> class.
        /// </summary>
        /// <param name="descriptor">The original underlying property descriptor.</param>
        public StringCollectionPropertyDescriptor(PropertyDescriptor descriptor)
            : base(descriptor, GetAttributes(descriptor))
        {
        }

        private static Attribute[] GetAttributes(PropertyDescriptor descriptor)
        {
            Guard.NotNull(() => descriptor, descriptor);

            return descriptor.Attributes.Cast<Attribute>().ToArray();
        }

        /// <summary>
        /// Gets a value indicating whether value change notifications for this property may originate from outside the property descriptor.
        /// </summary>
        /// <value></value>
        /// <returns>true if value change notifications may originate from outside the property descriptor; otherwise, false.</returns>
        public override bool SupportsChangeEvents
        {
            get { return true; }
        }

        /// <summary>
        /// When overridden in a derived class, gets the type of the property.
        /// </summary>
        /// <value></value>
        /// <returns>A <see cref="T:System.Type"/> that represents the type of the property.</returns>
        public override Type PropertyType
        {
            get { return this.IsReadOnly ? typeof(ReadOnlyCollection<T>) : typeof(Collection<T>); }
        }

        /// <summary>
        /// Gets the type converter for this property.
        /// </summary>
        public override TypeConverter Converter
        {
            get { return new StringCollectionTypeConverter(); }
        }

        /// <summary>
        /// Gets an editor of the specified type.
        /// </summary>
        public override object GetEditor(Type editorBaseType)
        {
            if (this.IsReadOnly)
            {
                return new UITypeEditor();
            }

            return new DesignCollectionEditor(this.PropertyType);
        }

        /// <summary>
        /// Gets the deserialized <see cref="Collection{T}"/> from the underlying serialized string.
        /// </summary>
        public override object GetValue(object component)
        {
            var value = base.GetValue(component) as string;
            var values = string.IsNullOrEmpty(value) ? new Collection<T>() : BindingSerializer.Deserialize<Collection<T>>(value);

            if (this.IsReadOnly)
            {
                return new ReadOnlyCollection<T>(values);
            }

            return values;
        }

        /// <summary>
        /// Saves the <see cref="Collection{T}"/> instances as a serialized string.
        /// </summary>
        public override void SetValue(object component, object value)
        {
            var values = value as Collection<T>;
            if (values != null)
            {
                var serializedValue = values.Count == 0 ? null : BindingSerializer.Serialize(values);
                base.SetValue(component, serializedValue);
            }
        }

        /// <summary>
        /// Clears the configured collection.
        /// </summary>
        public override void ResetValue(object component)
        {
            base.SetValue(component, null);
        }

        /// <summary>
        /// Determines whether the value can be reset.
        /// </summary>
        public override bool CanResetValue(object component)
        {
            return !this.IsReadOnly && !string.IsNullOrEmpty(base.GetValue(component) as string);
        }

        /// <summary>
        /// Provides friendly rendering of the collection.
        /// </summary>
        private class StringCollectionTypeConverter : TypeConverter
        {
            /// <summary>
            /// Returns whether this converter can convert the object to the specified type, using the specified context.
            /// </summary>
            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
                return destinationType == typeof(string) ? true : base.CanConvertTo(context, destinationType);
            }

            /// <summary>
            /// Converts the given value object to the specified type, using the specified context and culture information.
            /// </summary>
            public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
            {
                return destinationType == typeof(string) ?
                    Resources.StringCollectionPropertyDescriptor_ToString :
                    base.ConvertTo(context, culture, value, destinationType);
            }
        }
    }
}