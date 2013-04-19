using System;
using System.Collections.Generic;
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

    /// <summary>
    /// Wrapper for ICommandSettings used in chaining.
    /// </summary>
    internal interface ICommandSettings<T> : ICommandSettings
    {
    }

    /// <summary>
    /// Wrapper for ICommandSettings to be used in chaining. This class should not be used directly.
    /// </summary>
    internal class CommandSettingsDelegator<T> : ICommandSettings, ICommandSettings<T>
    {
        private ICommandSettings internalSettings;

        /// <summary>
        /// Wrapper constructor
        /// </summary>
        /// <param name="internalSettings">Settings to be wrapped</param>
        public CommandSettingsDelegator(ICommandSettings internalSettings)
        {
            Guard.NotNull(() => internalSettings, internalSettings);

            this.internalSettings = internalSettings;
            internalSettings.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(internalSettings_PropertyChanged);
        }

        void internalSettings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(sender, e);
            }
        }

        /// <summary>
        /// Wrapper for SuscribeChanged
        /// </summary>
        public IDisposable SubscribeChanged(Expression<Func<ICommandSettings, object>> propertyExpression, Action<ICommandSettings> callbackAction)
        {
            return SubscribeChanged(propertyExpression, callbackAction);
        }

        /// <summary>
        /// Wrapper for TypeId
        /// </summary>
        public string TypeId
        {
            get { return internalSettings.TypeId; }
            set { internalSettings.TypeId = value; }
        }

        /// <summary>
        /// Wrapper for Properties
        /// </summary>
        public IList<IPropertyBindingSettings> Properties
        {
            get { return internalSettings.Properties; }
        }

        /// <summary>
        /// Wrapper for PropertyChanged event
        /// </summary>
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Wrapper for Id
        /// </summary>
        public Guid Id
        {
            get { return internalSettings.Id; }
        }

        /// <summary>
        /// Wrapper for Name
        /// </summary>
        public string Name
        {
            get { return internalSettings.Name; }
        }

        /// <summary>
        /// Wrapper for Owner
        /// </summary>
        public Runtime.IPatternElementSchema Owner
        {
            get { return internalSettings.Owner; }
        }

        /// <summary>
        /// Wrapper for CreateAutomation
        /// </summary>
        public Runtime.IAutomationExtension CreateAutomation(Runtime.IProductElement owner)
        {
            return internalSettings.CreateAutomation(owner);
        }

        /// <summary>
        /// Wrapper for Classification
        /// </summary>
        public Runtime.AutomationSettingsClassification Classification
        {
            get { return internalSettings.Classification; }
        }
    }
}
