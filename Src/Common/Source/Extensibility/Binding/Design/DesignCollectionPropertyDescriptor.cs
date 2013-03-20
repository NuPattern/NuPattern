using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing.Design;
using System.Globalization;
using System.Linq;
using NuPattern.Extensibility.Properties;
using NuPattern.Runtime;

namespace NuPattern.Extensibility.Binding
{
    /// <summary>
    /// Provides a <see cref="PropertyDescriptor"/> that displays a <see cref="Collection{T}"/> property binding.
    /// </summary>
    public class DesignCollectionPropertyDescriptor<T> : PropertyDescriptor where T : class//, IBindingSettings
    {
        private PropertyDescriptor descriptor;

        /// <summary>
        /// Initializes a new instance of the <see cref="DesignCollectionPropertyDescriptor{T}"/> class.
        /// </summary>
        /// <param name="descriptor">The original underlying property descriptor.</param>
        public DesignCollectionPropertyDescriptor(PropertyDescriptor descriptor)
            : base(descriptor, descriptor.Attributes.Cast<Attribute>().ToArray())
        {
            this.descriptor = descriptor;
        }

        /// <summary>
        /// Gets the type of the Component.
        /// </summary>
        public override Type ComponentType
        {
            get { return typeof(T); }
        }

        /// <summary>
        /// Whether the property can be edited or not.
        /// </summary>
        public override bool IsReadOnly
        {
            get { return this.descriptor.IsReadOnly; }
        }

        /// <summary>
        /// Whether the current value should be serialized or not.
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public override bool ShouldSerializeValue(object component)
        {
            return this.descriptor.ShouldSerializeValue(component);
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
            get { return this.IsReadOnly ? typeof(ReadOnlyCollection<Collection<object>>) : this.descriptor.PropertyType; }
        }

        /// <summary>
        /// Clears the configured collection.
        /// </summary>
        public override void ResetValue(object component)
        {
            this.SetValue(component, new Collection<object>());
        }

        /// <summary>
        /// Determines whether the value can be reset.
        /// </summary>
        public override bool CanResetValue(object component)
        {
            var values = this.GetValue(component);
            if ((values != null)
                && (values as Collection<object>) != null)
            {
                return (values as Collection<object>).Any();
            }

            return false;
        }

        /// <summary>
        /// Gets the type converter for this property.
        /// </summary>
        public override TypeConverter Converter
        {
            get
            {
                // Use declared compatible converter or default design converter
                var baseConverter = this.descriptor.Converter;
                if (baseConverter != null
                    && baseConverter.GetType().IsOfGenericType(typeof(DesignCollectionConverter<>)))
                {
                    return baseConverter;
                }
                else
                {
                    return new DesignCollectionConverter(this.PropertyType);
                }
            }
        }

        /// <summary>
        /// Gets an editor of the specified type.
        /// </summary>
        public override object GetEditor(Type type)
        {
            if (this.IsReadOnly)
            {
                return new UITypeEditor();
            }

            // Use declared compatible editor or default design editor
            var baseEditor = this.descriptor.GetEditor(type);
            if (baseEditor is DesignCollectionEditor)
            {
                return baseEditor;
            }
            else
            {
                return new DesignCollectionEditor(this.PropertyType);
            }
        }

        /// <summary>
        /// Gets the deserialized <see cref="Collection{T}"/> from the underlying serialized string.
        /// </summary>
        public override object GetValue(object component)
        {
            var values = new Collection<object>();

            var propertySettings = GetPropertySettings(component);
            if (propertySettings != null)
            {
                var value = propertySettings.Value;
                if (!string.IsNullOrEmpty(value))
                {
                    // Exclude displayed caption
                    if (IsDisplayText(value))
                    {
                        values = new Collection<object>();
                    }
                    else
                    {
                        // Ask TypeConverter for deserialized value
                        var context = new SimpleTypeDescriptorContext { Instance = component, PropertyDescriptor = this };
                        if (this.Converter != null && this.Converter.CanConvertFrom(context, typeof(string)))
                        {
                            values = FromObjectToCollection<object>(this.Converter.ConvertFrom(context, CultureInfo.CurrentCulture, value));
                        }
                        else
                        {
                            values = new Collection<object>();
                        }
                    }
                }
            }

            if (this.IsReadOnly)
            {
                return new ReadOnlyCollection<object>(values);
            }

            return values;
        }

        /// <summary>
        /// Saves the new instances as a serialized string.
        /// </summary>
        public override void SetValue(object component, object value)
        {
            var values = value as ICollection<object>;
            if (values != null)
            {
                var serializedValue = values.Count == 0 ? null : BindingSerializer.Serialize(values);
                var property = GetPropertySettings(component);
                if (property != null)
                {
                    property.Value = serializedValue;
                }
            }
        }

        /// <summary>
        /// Retrieves the current property from storage.
        /// </summary>
        protected virtual IPropertyBindingSettings GetPropertySettings(object component)
        {
            var settings = component as T;
            return (settings != null) ? DesignPropertyDescriptor.EnsurePropertySettings(component, this.Name, this.PropertyType) : null;
        }

        private static bool IsDisplayText(string value)
        {
            return value.Equals(Resources.DesignCollectionPropertyDescriptor_ToString, StringComparison.Ordinal);
        }

        /// <summary>
        /// Converts object to <see cref="Collection{TCollection}"/>.
        /// </summary>
        public static Collection<TCollection> FromObjectToCollection<TCollection>(object collectionObject)
        {
            var enumerable = (IEnumerable)collectionObject;
            if (enumerable != null)
            {
                return new Collection<TCollection>(enumerable.Cast<TCollection>().ToList());
            }

            return new Collection<TCollection>();
        }

        /// <summary>
        /// Converts from <see cref="Collection{TCollection}"/> to <see cref="Collection{Object}"/>.
        /// </summary>
        public static Collection<object> ToObjectCollection<TCollection>(Collection<TCollection> collection)
        {
            return new Collection<object>(collection.Cast<object>().ToList());
        }
    }
}
