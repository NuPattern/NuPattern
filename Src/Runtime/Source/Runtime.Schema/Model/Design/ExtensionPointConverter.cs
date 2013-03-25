using System;
using System.ComponentModel;
using System.Linq;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Design;

namespace NuPattern.Runtime.Schema
{
    /// <summary>
    /// ExtensionPoint type converter
    /// </summary>
    internal class ExtensionPointConverter : TypeConverter
    {
        private Func<IExtensionPointSchema, bool> filter;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtensionPointConverter"/> class.
        /// </summary>
        public ExtensionPointConverter()
            : this(ext => true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtensionPointConverter"/> class.
        /// </summary>
        /// <param name="filter">The filter.</param>
        public ExtensionPointConverter(Func<IExtensionPointSchema, bool> filter)
        {
            this.filter = filter;
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
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context that can be used to extract additional information about the environment from which this converter is invoked. This parameter or properties of this parameter can be null.</param>
        /// <returns>
        /// A <see cref="T:System.ComponentModel.TypeConverter.StandardValuesCollection"/> that holds a standard set of valid values, or null if the data type does not support a standard set of values.
        /// </returns>
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            var patternManager = context.GetService<IPatternManager>();
            var extensionPoints =
                patternManager.InstalledToolkits.SelectMany(f => f.Schema.FindAll<IExtensionPointSchema>())
                .Where(this.filter)
                .Select(ext => CreateStandardValue(ext));

            return new StandardValuesCollection(extensionPoints.ToList());
        }

        private static StandardValue CreateStandardValue(IExtensionPointSchema extensionPoint)
        {
            var descriptionAtt =
                TypeDescriptor.GetAttributes(extensionPoint)
                    .OfType<DescriptionAttribute>()
                    .Select(att => att.Description)
                    .FirstOrDefault();

            descriptionAtt = descriptionAtt ?? string.Empty;

            var displayNameAtt =
                TypeDescriptor.GetAttributes(extensionPoint)
                    .OfType<DisplayNameAttribute>()
                    .Select(att => att.DisplayName)
                    .FirstOrDefault();

            displayNameAtt = displayNameAtt ?? TypeDescriptor.GetConverter(extensionPoint).ConvertToString(extensionPoint);

            return new StandardValue(displayNameAtt, extensionPoint, descriptionAtt);
        }
    }
}