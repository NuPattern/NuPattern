using System;
using System.ComponentModel;
using System.Linq;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Design;
using NuPattern.Extensibility;

namespace NuPattern.Runtime.Schema
{
    /// <summary>
    /// Property descriptor for the Type property of a <see cref="PropertySchema"/>.
    /// </summary>
    internal class ValueTypesPropertyDescriptor : DelegatingPropertyDescriptor
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ValueTypesPropertyDescriptor"/> class.
        /// </summary>
        public ValueTypesPropertyDescriptor(PropertyDescriptor innerDescriptor)
            : base(innerDescriptor)
        {
        }

        /// <summary>
        /// Gets the type converter for this property
        /// </summary>
        public override TypeConverter Converter
        {
            get
            {
                return new ValueTypesTypeConverter();
            }
        }

        /// <summary>
        /// Gets the type of this property.
        /// </summary>
        public override Type PropertyType
        {
            get
            {
                return typeof(string);
            }
        }

        /// <summary>
        /// Whether the property can be reset.
        /// </summary>
        public override bool CanResetValue(object component)
        {
            return true;
        }

        /// <summary>
        /// Resets the property value.
        /// </summary>
        public override void ResetValue(object component)
        {
            this.SetValue(component, typeof(string).FullName);
        }

        /// <summary>
        /// Returns the editor for this descriptor.
        /// </summary>
        public override object GetEditor(Type editorBaseType)
        {
            return new StandardValuesEditor();
        }

        /// <summary>
        /// Gets the value to display for  current property value.
        /// </summary>
        public override object GetValue(object component)
        {
            var value = base.GetValue(component);

            // Convert the display name
            var valueString = value as string;
            if (valueString != null)
            {
                var valueType = PropertySchema.PropertyValueTypes
                    .FirstOrDefault(p => p.DataType.FullName.Equals(valueString, StringComparison.OrdinalIgnoreCase));
                if (valueType != null)
                {
                    return valueType.DisplayName;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Sets the underlying value for this property.
        /// </summary>
        public override void SetValue(object component, object value)
        {
            var propertyValue = value as PropertyValueType;
            if (propertyValue != null)
            {
                base.SetValue(component, propertyValue.DataType.FullName);
            }
            else
            {
                base.SetValue(component, value);
            }
        }
    }
}
