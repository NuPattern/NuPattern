using System;
using System.ComponentModel;
using System.Linq.Expressions;
using NuPattern.Extensibility;
using NuPattern.Extensibility.Binding;

namespace NuPattern.Library.Automation
{
    /// <summary>
    /// Extensions for <see cref="CommandSettings"/>.
    /// </summary>
    internal static class CommandSettingsExtensions
    {
        /// <summary>
        /// Sets the specified property of the given CommandSettings to the specified value.
        /// </summary>
        public static void SetPropertyValue<TFeatureCommand, TProperty>(
            this CommandSettings commandSettings,
            Expression<Func<TFeatureCommand, TProperty>> expression,
            object value) where TFeatureCommand : Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.IFeatureCommand
        {
            var descriptors = TypeDescriptor.GetProperties(commandSettings);
            var descriptor = descriptors[Reflector<TFeatureCommand>.GetPropertyName(expression)];
            if (descriptor != null)
            {
                var designProperty = descriptor.GetValue(commandSettings) as DesignProperty;
                if (designProperty != null)
                {
                    designProperty.SetValue(value);
                }
                else if (!descriptor.IsReadOnly)
                {
                    // Try setting actual value directly
                    descriptor.SetValue(commandSettings, value);
                }
            }
        }
    }
}