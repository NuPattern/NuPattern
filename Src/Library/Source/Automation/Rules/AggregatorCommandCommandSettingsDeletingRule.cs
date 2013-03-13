using System.ComponentModel;
using System.Linq;
using Microsoft.VisualStudio.Modeling;
using NuPattern.Extensibility;
using NuPattern.Extensibility.Binding;
using NuPattern.Library.Commands;
using NuPattern.Library.Design;

namespace NuPattern.Library.Automation
{
    /// <summary>
    /// Deleting rule to keep command aggregator on sync
    /// </summary>
    [RuleOn(typeof(CommandSettings), FireTime = TimeToFire.TopLevelCommit)]
    public class AggregatorCommandCommandSettingsDeletingRule : DeletingRule
    {
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
                // Find all aggregator commands on same element as the command being deleted
                var element = commandSettings.Owner;
                var aggregatorCommands = element.AutomationSettings
                        .Select(s => s.As<ICommandSettings>())
                        .Where(s => s != null && s.TypeId == typeof(AggregatorCommand).FullName);
                if (aggregatorCommands.Any())
                {
                    // Update references for each aggregator command
                    aggregatorCommands.ForEach(cmdSettings =>
                        {
                            var property = TypeDescriptor.GetProperties(cmdSettings).Cast<PropertyDescriptor>()
                                .FirstOrDefault(p => p.Name == Reflector<AggregatorCommand>.GetPropertyName(x => x.CommandReferenceList));
                            if (property != null)
                            {
                                var references = DesignCollectionPropertyDescriptor<CommandSettings>.FromObjectToCollection<CommandReference>(property.GetValue(cmdSettings));
                                if (references != null)
                                {
                                    // Remove reference if references the command being deleted.
                                    var referenceToRemove = references.FirstOrDefault(r => r.CommandId == commandSettings.Id);
                                    if (referenceToRemove != null)
                                    {
                                        references.Remove(referenceToRemove);
                                        property.SetValue(cmdSettings, DesignCollectionPropertyDescriptor<CommandSettings>.ToObjectCollection<CommandReference>(references));
                                    }
                                }
                            }
                        });
                }
            }
        }
    }
}