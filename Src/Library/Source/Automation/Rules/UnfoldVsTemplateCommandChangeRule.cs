using System.Linq;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using NuPattern.Extensibility;
using NuPattern.Library.Commands;
using NuPattern.Library.Properties;

namespace NuPattern.Library.Automation
{
    /// <summary>
    /// Change rule to keep command solution item name in sync
    /// </summary>
    [RuleOn(typeof(CommandSettings), FireTime = TimeToFire.TopLevelCommit)]
    public class UnfoldVsTemplateCommandChangeRule : ChangeRule
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<UnfoldVsTemplateCommandChangeRule>();

        /// <summary>
        /// Handles the property change event for the settings.
        /// </summary>
        /// <param name="e">The event args.</param>
        public override void ElementPropertyChanged(ElementPropertyChangedEventArgs e)
        {
            Guard.NotNull(() => e, e);

            base.ElementPropertyChanged(e);

            var commandSettings = e.ModelElement as CommandSettings;
            if (commandSettings != null)
            {
                if (e.DomainProperty.Id == CommandSettings.PropertiesDomainPropertyId)
                {
                    if (!e.ModelElement.Store.TransactionManager.CurrentTransaction.IsSerializing)
                    {
                        // Find all unfold commands on the same element that changed
                        var element = commandSettings.Owner;
                        var unfoldCommands = element.AutomationSettings
                                                    .Select(s => s.As<ICommandSettings>())
                                                    .Where(s => s != null && s.TypeId == typeof(UnfoldVsTemplateCommand).FullName);
                        if (unfoldCommands.Any())
                        {
                            unfoldCommands.ToList().ForEach(cmd =>
                                {
                                    tracer.Shield(() =>
                                        {
                                            SyncNameExtension.EnsureSyncNameExtensionAutomation(commandSettings.Owner);
                                        }, Resources.UnfoldVsCommandChangeRule_ErrorSyncNameFailed, cmd.Name);
                                });
                        }
                    }
                }
            }
        }
    }
}