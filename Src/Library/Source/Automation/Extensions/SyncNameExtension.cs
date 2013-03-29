using System;
using System.Linq;
using NuPattern.Library.Commands;
using NuPattern.Library.Events;
using NuPattern.Library.Properties;
using NuPattern.Reflection;
using NuPattern.Runtime;

namespace NuPattern.Library.Automation
{
    /// <summary>
    /// Customizations for the <see cref="SyncNameExtension"/> class.
    /// </summary>
    internal static class SyncNameExtension
    {
        /// <summary>
        /// Ensures the associated commands and launchpoint automation are created and configured correctly.
        /// </summary>
        internal static void EnsureSyncNameExtensionAutomation(IPatternElementSchema pattern)
        {
            // Configure the sync command and menu.
            var syncCommand = pattern.EnsureCommandAutomation<SynchArtifactNameCommand>(
                Resources.SyncNameExtension_SyncNameCommandName,
                () => ShouldSyncName(pattern));

            syncCommand.SetPropertyValue<SynchArtifactNameCommand, string>(s => s.ReferenceTag, SynchArtifactNameCommand.FilteredReferenceTagValue);

            // Configure the sync event.
            pattern.EnsureEventLaunchPoint<IOnElementPropertyChangedEvent>(
                Resources.SyncNameExtension_SyncNameEventName,
                syncCommand,
                true,
                () => ShouldSyncName(pattern));
        }

        private static bool ShouldSyncName(IPatternElementSchema pattern)
        {
            return pattern.GetAutomationSettings<ITemplateSettings>().Any(t => t.SyncName)
                || pattern.GetAutomationSettings<ICommandSettings>().Any(c => c.TypeId == typeof(UnfoldVsTemplateCommand).ToString()
                    && c.Properties.Any(p => p.Name == Reflector<UnfoldVsTemplateCommand>.GetPropertyName(u => u.SyncName) && string.Equals(p.Value, true.ToString(), StringComparison.OrdinalIgnoreCase)))
                || pattern.GetAutomationSettings<ICommandSettings>().Any(c => c.TypeId == typeof(GenerateProductCodeCommand).ToString()
                    && c.Properties.Any(p => p.Name == Reflector<GenerateProductCodeCommand>.GetPropertyName(u => u.SyncName) && string.Equals(p.Value, true.ToString(), StringComparison.OrdinalIgnoreCase)));
        }

    }
}
