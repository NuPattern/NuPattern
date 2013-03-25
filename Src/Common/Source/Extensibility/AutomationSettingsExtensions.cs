using System;
using System.Collections.Generic;
using System.Linq;
using NuPattern.Runtime;

namespace NuPattern.Extensibility
{
    /// <summary>
    /// Usability extension methods to deal with automation extension settings.
    /// </summary>
    public static class AutomationSettingsExtensions
    {
        /// <summary>
        /// Attemps to resolve the given <paramref name="settingsReferenceId"/> to a configured automation 
        /// extension setting in the same container as the given <paramref name="automationSettings"/>.
        /// </summary>
        /// <param name="automationSettings">The automation setting that references another automation extension setting by id.</param>
        /// <param name="settingsReferenceId">Identifier of the referenced automation extension setting.</param>
        public static IAutomationSettingsSchema ResolveAutomationSettings(this IAutomationSettings automationSettings, Guid settingsReferenceId)
        {
            Guard.NotNull(() => automationSettings, automationSettings);
            Guard.NotNull(() => automationSettings.Owner, automationSettings.Owner);

            return automationSettings.Owner.AutomationSettings
                .FirstOrDefault(setting => setting.Id == settingsReferenceId);
        }

        /// <summary>
        /// Attemps to resolve the given <paramref name="settingsReferenceId"/> and 
        /// <typeparamref name="TSettings"/> type to a configured automation extension 
        /// setting on in the same container as the given <paramref name="automationSettings"/>.
        /// </summary>
        /// <typeparam name="TSettings">The type of the automation settings to look up.</typeparam>
        /// <param name="automationSettings">The automation extension setting that references another setting by id.</param>
        /// <param name="settingsReferenceId">Identifier of the referenced automation extension setting.</param>
        public static TSettings ResolveAutomationSettings<TSettings>(this IAutomationSettings automationSettings, Guid settingsReferenceId)
            where TSettings : IAutomationSettings
        {
            Guard.NotNull(() => automationSettings, automationSettings);
            Guard.NotNull(() => automationSettings.Owner, automationSettings.Owner);

            return automationSettings.Owner.AutomationSettings
                .SettingsOfType<TSettings>()
                .FirstOrDefault(setting => setting.Id == settingsReferenceId);
        }

        /// <summary>
        /// Retrieves the automation extension settings that are compatible with the 
        /// given <typeparamref name="TSettings"/>.
        /// </summary>
        public static IEnumerable<TSettings> SettingsOfType<TSettings>(this IEnumerable<IAutomationSettingsSchema> settingsSchema)
            where TSettings : IAutomationSettings
        {
            return settingsSchema.Select(schema => schema.As<TSettings>()).Where(setting => setting != null);
        }
    }
}
