using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Design;
using NuPattern.Runtime.Schema.Properties;

namespace NuPattern.Runtime.Schema
{
    /// <summary>
    /// Provides a TypeConverter for standard data types.
    /// </summary>
    public class ValueTypesTypeConverter : TypeConverter
    {
        /// <summary>
        /// Converts from a string representation (i.e. System.Double or "Floating Point Number") to the full type name.
        /// </summary>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            Guard.NotNull(() => value, value);
            Guard.NotNull(() => destinationType, destinationType);

            if (destinationType == typeof(string))
            {
                var propertyValue = value as PropertyValueType;
                if (propertyValue != null)
                {
                    return propertyValue.DisplayName;
                }
                else
                {
                    var valueString = value as string;
                    if (valueString != null)
                    {
                        return valueString;
                    }
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        /// <summary>
        /// Determines that this coverter has standard types.
        /// </summary>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        /// <summary>
        /// Determines that this converter only allows selection of one of its standard values.
        /// </summary>
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }

        /// <summary>
        /// Returns the standard values for this type.
        /// </summary>
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(PropertySchema.PropertyValueTypes.Select(p => CreateStandardValue(p)).ToArray());
        }

        private static StandardValue CreateStandardValue(PropertyValueType propertyValue)
        {
            return new StandardValue(propertyValue.DisplayName, propertyValue,
                string.Format(CultureInfo.CurrentCulture, Resources.ValueTypeTypeConverter_Description, propertyValue.DataType.FullName),
                group: propertyValue.Category);
        }
    }
}