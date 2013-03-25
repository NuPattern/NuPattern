using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using NuPattern.Extensibility;
using NuPattern.Runtime;

namespace NuPattern.Library.Automation
{
    /// <summary>
    /// Extensions to the <see cref=" IPatternElementSchema"/> class.
    /// </summary>
    internal static class PatternElementSchemaExtensions
    {
        /// <summary>
        /// Creates the settings for associating guidance, that are also not customizable.
        /// </summary>
        public static TSettings CreateSystemAutomationSettings<TSettings>(this IPatternElementSchema container, string settingsName)
            where TSettings : ExtensionElement, IAutomationSettings
        {
            return container.CreateAutomationSettings<TSettings>(settingsName, schema =>
            {
                schema.IsCustomizable = CustomizationState.False;
                schema.IsSystem = true;
            });
        }

        /// <summary>
        /// Ensures specified command is either: present and configured correctly on the element, or removed from element, based on the evaluation of the given function. 
        /// </summary>
        public static CommandSettings EnsureCommandAutomation<TCommand>(this IPatternElementSchema container, string instanceName, Func<bool> exists) where TCommand : IFeatureCommand
        {
            var settings = container.GetAutomationSettings<CommandSettings>(instanceName);
            if (exists() == true)
            {
                // Create a new instance of not already exists.
                if (settings == null)
                {
                    settings = PatternElementSchemaExtensions.CreateSystemAutomationSettings<CommandSettings>(container, instanceName);
                }

                // Update properties
                if (settings != null)
                {
                    settings.TypeId = typeof(TCommand).FullName;
                }
            }
            else
            {
                // Delete existing command
                if (settings != null)
                {
                    settings.Extends.Delete();
                    return null;
                }
            }

            return settings;
        }

        /// <summary>
        /// Ensures specified event is either: present and configured correctly on the element, or removed from element, based on the evaluation of the given function. 
        /// </summary>
        public static EventSettings EnsureEventLaunchPoint<TEvent>(this IPatternElementSchema container, string instanceName, CommandSettings command, bool filterForCurrentElement, Func<bool> exists) where TEvent : IObservableEvent
        {
            return EnsureEventLaunchPoint(container, typeof(TEvent).FullName, instanceName, command, filterForCurrentElement, exists);
        }

        /// <summary>
        /// Ensures specified event is either: present and configured correctly on the element, or removed from element, based on the evaluation of the given function. 
        /// </summary>
        public static EventSettings EnsureEventLaunchPoint(this IPatternElementSchema container, string eventTypeName, string instanceName, CommandSettings command, bool filterForCurrentElement, Func<bool> exists)
        {
            var settings = container.GetAutomationSettings<EventSettings>(instanceName);
            if (exists() == true)
            {
                // Create a new instance of not already exists.
                if (settings == null)
                {
                    settings = PatternElementSchemaExtensions.CreateSystemAutomationSettings<EventSettings>(container, instanceName);
                }

                // Update properties
                if (settings != null)
                {
                    settings.EventId = eventTypeName;
                    settings.FilterForCurrentElement = filterForCurrentElement;
                    if (command != null)
                    {
                        settings.CommandId = command.Id;
                    }
                }
            }
            else
            {
                // Delete existing instance
                if (settings != null)
                {
                    settings.Extends.Delete();
                    return null;
                }
            }

            return settings;
        }

        /// <summary>
        /// Ensures specified menu is either: present and configured correctly on the element, or removed from element, based on the evaluation of the given function. 
        /// </summary>
        public static MenuSettings EnsureMenuLaunchPoint(this IPatternElementSchema container, string instanceName, CommandSettings command, string menuText, string iconPath, Func<bool> exists)
        {
            var settings = container.GetAutomationSettings<MenuSettings>(instanceName);
            if (exists() == true)
            {
                // Create a new instance of not already exists.
                if (settings == null)
                {
                    settings = PatternElementSchemaExtensions.CreateSystemAutomationSettings<MenuSettings>(container, instanceName);
                }

                // Update properties
                if (settings != null)
                {
                    settings.Text = menuText;
                    settings.SortOrder = 10000; // This to make it large enough to sink to bottom of other menus
                    settings.Icon = FormatIconPath(iconPath);
                    if (command != null)
                    {
                        settings.CommandId = command.Id;
                    }
                }
            }
            else
            {
                // Delete existing instance
                if (settings != null)
                {
                    settings.Extends.Delete();
                    return null;
                }
            }

            return settings;
        }

        /// <summary>
        /// Ensures specified variable property is either: present and configured correctly on the element, or removed from element, based on the evaluation of the given function. 
        /// </summary>
        public static IPropertySchema EnsureVariablePropertyForAutomation<TTypeConverter>(this IPatternElementSchema container, string instanceName, string description, string displayName, string category, Func<bool> exists) where TTypeConverter : TypeConverter
        {
            var instantiateProperty = container.Properties.FirstOrDefault<IPropertySchema>(prop => prop.Name == instanceName);
            if (exists() == true)
            {
                // Create a new instance of not already exists.
                if (instantiateProperty == null)
                {
                    instantiateProperty = container.CreatePropertySchema(prop =>
                    {
                        prop.Name = instanceName;
                        prop.IsVisible = true;
                        prop.IsReadOnly = false;
                        prop.IsCustomizable = CustomizationState.False;
                        prop.IsSystem = true;
                    });
                }

                // Update properties
                if (instantiateProperty != null)
                {
                    instantiateProperty.DisplayName = displayName;
                    instantiateProperty.Description = description;
                    instantiateProperty.Category = category;
                    var converterType = typeof(TTypeConverter);
                    instantiateProperty.TypeConverterTypeName = converterType.FullName + ", " + converterType.Assembly.FullName.Split(',')[0];
                }
            }
            else
            {
                // Delete existing instance
                if (instantiateProperty != null)
                {
                    container.DeletePropertySchema(instantiateProperty);
                    return null;
                }
            }

            return instantiateProperty;
        }

        private static string FormatIconPath(string iconPath)
        {
            if (!string.IsNullOrEmpty(iconPath))
            {
                return string.Format(CultureInfo.CurrentCulture,
                    "pack://application:,,,/{0};component/{1}", typeof(PatternElementSchemaExtensions).Assembly.GetName().Name, iconPath);
            }

            else
            {
                return null;
            }
        }
    }
}
