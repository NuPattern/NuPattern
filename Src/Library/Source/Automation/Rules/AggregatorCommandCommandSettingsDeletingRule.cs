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

            var deletingCommand = e.ModelElement as CommandSettings;
            if (deletingCommand != null && deletingCommand.Extends != null)
            {
                // Find all aggregator commands on same element as the command being deleted
                var element = deletingCommand.Owner;
                var aggregatorCommands = element.AutomationSettings
                        .Select(s => s.As<ICommandSettings>())
                        .Where(s => s != null && s.TypeId == typeof(AggregatorCommand).FullName && s.Id != deletingCommand.Id);
                if (aggregatorCommands.Any())
                {
                    // Update references for each aggregator command
                    aggregatorCommands.ForEach(cmdSettings =>
                        {
                            // Get the referenced aggregated commands
                            var property = TypeDescriptor.GetProperties(cmdSettings).Cast<PropertyDescriptor>()
                                .FirstOrDefault(p => p.Name == Reflector<AggregatorCommand>.GetPropertyName(x => x.CommandReferenceList));
                            if (property != null)
                            {
                                var references = DesignCollectionPropertyDescriptor<CommandSettings>.FromObjectToCollection<CommandReference>(property.GetValue(cmdSettings));
                                if (references != null)
                                {
                                    // Remove reference if references the command being deleted.
                                    var referenceToRemove = references.FirstOrDefault(r => r.CommandId == deletingCommand.Id);
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