using System;
using System.Linq;
using Microsoft.VisualStudio.Patterning.Library.Commands;
using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.Modeling;

namespace Microsoft.VisualStudio.Patterning.Library.Automation
{
    /// <summary>
    /// Deleting rule to keep command aggregator on sync
    /// </summary>
    [RuleOn(typeof(CommandSettings), FireTime = TimeToFire.TopLevelCommit)]
    public class AggregatorCommandCommandSettingsDeletingRule : DeletingRule
    {
        private const string Separator = ";";

        /// <summary>
        /// public virtual method for the client to have his own user-defined delete rule class
        /// </summary>
        /// <param name="e"></param>
        public override void ElementDeleting(ElementDeletingEventArgs e)
        {
			Guard.NotNull(() => e, e);

            base.ElementDeleting(e);

            var commandSettings = e.ModelElement as CommandSettings;

            var element = commandSettings.Owner;

            var typeId = typeof(AggregatorCommand).FullName;

            var aggregatorCommandSettings = element.AutomationSettings
                    .Select(s => s.As<ICommandSettings>())
                    .Where(s => 
                        s != null && 
                        s.TypeId == typeId);

            if(aggregatorCommandSettings.Any())
            {
                foreach (var setting in aggregatorCommandSettings)
                {
                    var property = setting.Properties.FirstOrDefault(
                        p => p.Name == Reflector<AggregatorCommand>.GetPropertyName(x => x.CommandReferenceList));

                    if (property != null)
                    {
                        var propertySettings = property as PropertySettings;

                        var ids = propertySettings.Value.ToString().Split(
                            new string[] { Separator }, StringSplitOptions.RemoveEmptyEntries);

                        propertySettings.Value =
                            string.Join(Separator, ids.Where(id => new Guid(id) != commandSettings.Id)); 
                    }
                }
            }
        }
    }
}