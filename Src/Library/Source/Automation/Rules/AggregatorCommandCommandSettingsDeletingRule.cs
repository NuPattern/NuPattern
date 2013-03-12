using System;
using System.Linq;
using Microsoft.VisualStudio.Modeling;
using NuPattern.Extensibility;
using NuPattern.Library.Commands;

namespace NuPattern.Library.Automation
{
    /// <summary>
    /// Deleting rule to keep command aggregator on sync
    /// </summary>
    [RuleOn(typeof(CommandSettings), FireTime = TimeToFire.TopLevelCommit)]
    public class AggregatorCommandCommandSettingsDeletingRule : DeletingRule
    {
        private const string Separator = ";";

        /// <summary>
        /// Handles the element deleting rule
        /// </summary>
        /// <param name="e"></param>
        public override void ElementDeleting(ElementDeletingEventArgs e)
        {
            Guard.NotNull(() => e, e);

            base.ElementDeleting(e);

            var commandSettings = e.ModelElement as CommandSettings;
            if (commandSettings != null)
            {
                // Find the aggregator command on same element as the command being deleted
                var element = commandSettings.Owner;
                var aggregatorCommandSettings = element.AutomationSettings
                        .Select(s => s.As<ICommandSettings>())
                        .Where(s =>
                            s != null &&
                            s.TypeId == typeof(AggregatorCommand).FullName);
                if (aggregatorCommandSettings.Any())
                {
                    // Rebuild settings for command
                    foreach (var setting in aggregatorCommandSettings)
                    {
                        var property = setting.Properties.FirstOrDefault(
                            p => p.Name == Reflector<AggregatorCommand>.GetPropertyName(x => x.CommandReferenceList));
                        if (property != null)
                        {
                            var ids = property.Value.Split(
                                new string[] { Separator }, StringSplitOptions.RemoveEmptyEntries);

                            property.Value =
                                string.Join(Separator, ids.Where(id => new Guid(id) != commandSettings.Id));
                        }
                    }
                }
            }
        }
    }
}