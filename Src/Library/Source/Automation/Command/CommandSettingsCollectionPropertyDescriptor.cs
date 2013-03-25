using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using NuPattern.Runtime.Design;

namespace NuPattern.Library.Automation
{
    /// <summary>
    /// Provides a <see cref="PropertyDescriptor"/> that displays a <see cref="Collection{T}"/> property binding.
    /// </summary>
    internal class CommandSettingsCollectionPropertyDescriptor : StringCollectionPropertyDescriptor<CommandSettings>
    {
        private PropertyDescriptor descriptor;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandSettingsCollectionPropertyDescriptor"/> class.
        /// </summary>
        /// <param name="descriptor">The original underlying property descriptor.</param>
        public CommandSettingsCollectionPropertyDescriptor(PropertyDescriptor descriptor)
            : base(descriptor)
        {
            this.descriptor = descriptor;
        }

        /// <summary>
        /// Gets the type of the Component.
        /// </summary>
        public override Type ComponentType
        {
            get { return typeof(CommandSettings); }
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

        ///// <summary>
        ///// Clears the configured collection.
        ///// </summary>
        //public override void ResetValue(object component)
        //{
        //    this.SetValue(component, new Collection<object>());
        //}

        ///// <summary>
        ///// Determines whether the value can be reset.
        ///// </summary>
        //public override bool CanResetValue(object component)
        //{
        //    var values = this.GetValue(component);
        //    if ((values != null)
        //        && (values as Collection<object>) != null)
        //    {
        //        return (values as Collection<object>).Any();
        //    }

        //    return false;
        //}

        ///// <summary>
        ///// Gets the type converter for this property.
        ///// </summary>
        //public override TypeConverter Converter
        //{
        //    get { return new DesignCollectionTypeConverter(); }
        //}

        ///// <summary>
        ///// Gets the deserialized <see cref="Collection{T}"/> from the underlying serialized string.
        ///// </summary>
        //public override object GetValue(object component)
        //{
        //    var values = new Collection<object>();

        //    var property = GetPropertySettings(component);
        //    if (property != null)
        //    {
        //        var value = property.Value;
        //        if (!string.IsNullOrEmpty(value))
        //        {
        //            // Exclude displayed caption
        //            values = IsDisplayText(value) ? new Collection<object>() : ConvertObjectToObjectCollection(BindingSerializer.Deserialize(value, this.descriptor.PropertyType));
        //        }
        //    }

        //    if (this.IsReadOnly)
        //    {
        //        return new ReadOnlyCollection<object>(values);
        //    }

        //    return values;
        //}

        ///// <summary>
        ///// Saves the new instances as a serialized string.
        ///// </summary>
        //public override void SetValue(object component, object value)
        //{
        //    var values = value as ICollection<object>;
        //    if (values != null)
        //    {
        //        var serializedValue = values.Count == 0 ? null : BindingSerializer.Serialize(values);
        //        var property = GetPropertySettings(component);
        //        if (property != null)
        //        {
        //            property.Value = serializedValue;
        //        }
        //    }
        //}

        ///// <summary>
        ///// Retrieves the current property from storage.
        ///// </summary>
        //protected virtual IPropertyBindingSettings GetPropertySettings(object component)
        //{
        //    var settings = component as CommandSettings;
        //    return (settings != null) ? DesignPropertyDescriptor.EnsurePropertySettings(component, this.Name, this.PropertyType) : null;
        //}

        //private static bool IsDisplayText(string value)
        //{
        //    return value.Equals(Resources.DesignCollectionPropertyDescriptor_ToString, StringComparison.Ordinal);
        //}

        //private static Collection<object> ConvertObjectToObjectCollection(object result)
        //{
        //    var values = new Collection<object>();

        //    var collection = result as ICollection;
        //    if (collection != null)
        //    {
        //        values.AddRange(collection.Cast<object>());
        //    }

        //    return values;
        //}

        ///// <summary>
        ///// Provides friendly rendering of the collection at design time.
        ///// </summary>
        //private class DesignCollectionTypeConverter : TypeConverter
        //{
        //    /// <summary>
        //    /// Whether this converter can convert the object to the destinationType, using the specified context.
        //    /// </summary>
        //    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        //    {
        //        return destinationType == typeof(InstanceDescriptor) || base.CanConvertTo(context, destinationType);
        //    }

        //    /// <summary>
        //    /// Converts the given object value to the destinationType type, using the specified context and culture information.
        //    /// </summary>
        //    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        //    {
        //        return destinationType == typeof(string) ?
        //            Resources.DesignCollectionPropertyDescriptor_ToString :
        //            base.ConvertTo(context, culture, value, destinationType);
        //    }

        //    /// <summary>
        //    /// Converts the given value to the object value, using the specified context and culture information. 
        //    /// </summary>
        //    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        //    {
        //        // Need this to copy value back to descriptor
        //        if (value is Collection<object>)
        //        {
        //            return value as Collection<object>;
        //        }

        //        return base.ConvertFrom(context, culture, value);
        //    }
        //}
    }
}
