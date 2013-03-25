using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Extensibility;
using NuPattern.Runtime;

namespace NuPattern.Extensibility
{
    /// <summary>
    /// Extension methods over <see cref="PatternElementSchemaExtensions"/> class.
    /// </summary>
    public static class PatternElementSchemaExtensions
    {
        /// <summary>
        /// Creates a new settings and adds it to the schema element. The <typeparamref name="TSettings"/> is the 
        /// type of the extension element 
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Not Appplicable")]
        public static TSettings CreateAutomationSettings<TSettings>(this IPatternElementSchema container, string settingsName,
            Action<IAutomationSettingsSchema> initializer = null)
            where TSettings : class, IAutomationSettings
        {
            TSettings automationSettings = default(TSettings);

            container.CreateAutomationSettingsSchema(aes =>
            {
                var aesMel = (ModelElement)aes;
                var classInfo = aesMel.Store.DomainDataDirectory.DomainClasses.Where(c => typeof(TSettings).IsAssignableFrom(c.ImplementationClass)).FirstOrDefault();
                automationSettings = aesMel.AddExtension(classInfo) as TSettings;

                aes.Name = settingsName;
                aes.AutomationType = classInfo.DisplayName;
                aes.Classification = automationSettings.Classification;

                if (initializer != null)
                    initializer(aes);
            });

            return automationSettings;
        }

        /// <summary>
        /// Gets the automation settings by name.
        /// </summary>
        /// <typeparam name="TSettings">The type of the settings.</typeparam>
        /// <param name="element">The automation settings container schema.</param>
        /// <param name="settingsName">Name of the settings.</param>
        public static TSettings GetAutomationSettings<TSettings>(this IPatternElementSchema element, string settingsName)
            where TSettings : IAutomationSettings
        {
            Guard.NotNull(() => element, element);
            Guard.NotNull(() => settingsName, settingsName);

            return (from cs in element.AutomationSettings
                    where cs.Name.Equals(settingsName, StringComparison.OrdinalIgnoreCase)
                    let setting = cs.As<TSettings>()
                    where setting != null
                    select (TSettings)setting)
                .FirstOrDefault();
        }

        /// <summary>
        /// Gets the settings.
        /// </summary>
        /// <typeparam name="TSettings">The type of the settings.</typeparam>
        /// <param name="element">The container.</param>
        public static IEnumerable<TSettings> GetAutomationSettings<TSettings>(this IPatternElementSchema element) where TSettings : IAutomationSettings
        {
            Guard.NotNull(() => element, element);

            return element.AutomationSettings.Select(s => s.As<TSettings>()).Where(t => t != null);
        }
    }
}