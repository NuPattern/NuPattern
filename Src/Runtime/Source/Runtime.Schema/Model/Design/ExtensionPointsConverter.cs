using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;

namespace NuPattern.Runtime.Schema
{
    /// <summary>
    /// ExtensionPoints type converter
    /// </summary>
    public class ExtensionPointsConverter : TypeConverter
    {
        private const char Separator = '|';

        private IEnumerable<IExtensionPointSchema> extensionPoints = null;
        private Func<IInstalledToolkitInfo, bool> filter;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtensionPointsConverter"/> class.
        /// </summary>
        public ExtensionPointsConverter()
            : this(ext => true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtensionPointsConverter"/> class.
        /// </summary>
        /// <param name="filter">The filter.</param>
        public ExtensionPointsConverter(Func<IInstalledToolkitInfo, bool> filter)
        {
            this.filter = filter;
        }

        /// <summary>
        /// Returns whether this converter can convert an object of the given type to the type of this converter, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="sourceType">A <see cref="Type"/> that represents the type you want to convert from.</param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        /// Converts the given object to the type of this converter, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="culture">The <see cref="CultureInfo"/> to use as the current culture.</param>
        /// <param name="value">The <see cref="object"/> to convert.</param>
        /// <returns>
        /// An <see cref="object"/> that represents the converted value.
        /// </returns>
        /// <exception cref="NotSupportedException">The conversion cannot be performed. </exception>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var valueString = value as string;

            if (valueString != null)
            {
                if (valueString.Length > 0)
                {
                    var values = valueString.Split(Separator);

                    if (this.extensionPoints == null)
                    {
                        SetExtensionPoints(context);
                    }

                    var @return = values.Select(val =>
                            this.extensionPoints.FirstOrDefault(
                                ext => ext.RequiredExtensionPointId.Equals(val, StringComparison.OrdinalIgnoreCase))).ToList();

                    if (this.GetStandardValues(null).Cast<IExtensionPointSchema>().Intersect(@return).Count() == @return.Count())
                    {
                        return @return;
                    }
                }

                return null;
            }

            return base.ConvertFrom(context, culture, value);
        }

        /// <summary>
        /// Returns whether this converter can convert the object to the specified type, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="destinationType">A <see cref="Type"/> that represents the type you want to convert to.</param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return true;
            }

            return base.CanConvertTo(context, destinationType);
        }

        /// <summary>
        /// Converts the given value object to the specified type, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="culture">A <see cref="CultureInfo"/>. If null is passed, the current culture is assumed.</param>
        /// <param name="value">The <see cref="Object"/> to convert.</param>
        /// <param name="destinationType">The <see cref="Type"/> to convert the <paramref name="value"/> parameter to.</param>
        /// <returns>
        /// An <see cref="T:System.Object"/> that represents the converted value.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="destinationType"/> parameter is null.</exception>
        /// <exception cref="NotSupportedException">The conversion cannot be performed.</exception>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                var values = value as List<IExtensionPointSchema>;

                if (values != null)
                {
                    return string.Join(Separator.ToString(), values.Select(val => val.RequiredExtensionPointId).ToArray());
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        /// <summary>
        /// Returns whether this object supports a standard set of values that can be picked from a list, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
        /// <returns>
        /// true if <see cref="TypeConverter.GetStandardValues()"/> should be called to find a common set of values the object supports; otherwise, false.
        /// </returns>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        /// <summary>
        /// Returns a collection of standard values for the data type this type converter is designed for when provided with a format context.
        /// </summary>
        /// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context that can be used to extract additional information about the environment from which this converter is invoked. This parameter or properties of this parameter can be null.</param>
        /// <returns>
        /// A <see cref="TypeConverter.StandardValuesCollection"/> that holds a standard set of valid values, or null if the data type does not support a standard set of values.
        /// </returns>
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            if (this.extensionPoints == null)
            {
                SetExtensionPoints(context);
            }

            return new StandardValuesCollection(this.extensionPoints.ToList());
        }

        private void SetExtensionPoints(ITypeDescriptorContext context)
        {
            var patternManager = context.GetService<IPatternManager>();
            this.extensionPoints =
                patternManager.InstalledToolkits.Where(this.filter)
                .SelectMany(f => f.Schema.FindAll<IExtensionPointSchema>());
        }
    }
}