using System;
using System.ComponentModel;
using System.Linq;
using NuPattern.ComponentModel;
using NuPattern.Runtime.Guidance;
using NuPattern.Runtime.Properties;

namespace NuPattern.Runtime.Design
{
    /// <summary>
    /// Type converter thats shows list of installed guidance extensions.
    /// </summary>
    public class GuidanceExtensionsTypeConverter : TypeConverter
    {
        /// <summary>
        /// Returns whether this object supports a standard set of values that can be picked from a list, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        /// <summary>
        /// Returns whether the collection of standard values returned from <see cref="M:System.ComponentModel.TypeConverter.GetStandardValues"/> is an exclusive list of possible values, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return false;
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
            var guidanceManager = context.GetService<IGuidanceManager>();

            var values = guidanceManager.InstantiatedGuidanceExtensions
                .Select(e => new StandardValue(e.InstanceName, e.InstanceName, GetDescription(guidanceManager, e)))
                .Concat(new[] { new StandardValue(Resources.GuidanceExtensionsTypeConverter_None, string.Empty) })
                .ToArray();

            return new StandardValuesCollection(values);
        }

        private static string GetDescription(IGuidanceManager guidanceManager, IGuidanceExtension extension)
        {
            return guidanceManager.InstalledGuidanceExtensions
                .Where(reg => reg.ExtensionId.Equals(extension.ExtensionId, StringComparison.OrdinalIgnoreCase))
                .Select(reg => reg.ExtensionManifest.Header.Description)
                .FirstOrDefault();
        }
    }
}