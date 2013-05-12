using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using NuPattern.Library.Automation;
using NuPattern.Reflection;
using NuPattern.Runtime;
using NuPattern.Runtime.Bindings;
using NuPattern.Runtime.Bindings.Design;

namespace NuPattern.Library.Commands
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
            object value) where TFeatureCommand : ICommand
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

        /// <summary>
        /// Gets the value of the property.
        /// </summary>
        public static T GetPropertyValue<T>(this ICommandSettings settings, string propertyName)
        {
            Guard.NotNull(() => settings, settings);
            Guard.NotNullOrEmpty(() => propertyName, propertyName);

            var converter = TypeDescriptor.GetConverter(typeof(T));
            var prop = (IPropertyBindingSettings)settings.Properties.FirstOrDefault(p => p.Name == propertyName);
            if (prop != null)
            {
                return (T)converter.ConvertFromString(prop.Value);
            }

            return default(T);
        }

        /// <summary>
        /// Sets the value of the property.
        /// </summary>
        public static void SetPropertyValue<T>(this ICommandSettings settings, string propertyName, T value)
        {
            Guard.NotNull(() => settings, settings);
            Guard.NotNullOrEmpty(() => propertyName, propertyName);

            var converter = TypeDescriptor.GetConverter(typeof(T));
            var prop = (IPropertyBindingSettings)settings.Properties.FirstOrDefault(p => p.Name == propertyName);
            if (prop != null)
            {
                prop.Value = converter.ConvertToString(value);
            }
        }
    }
}
