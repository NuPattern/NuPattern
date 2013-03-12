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
                if (!e.ModelElement.Store.TransactionManager.CurrentTransaction.IsSerializing)
                {
                    // Get the unfold commands on the element
                    var element = commandSettings.Owner;
                    var unfoldCommandSettings = element.AutomationSettings
                                                       .Select(s => s.As<ICommandSettings>())
                                                       .Where(s =>
                                                              s != null &&
                                                              s.TypeId == typeof(UnfoldVsTemplateCommand).FullName);
                    if (unfoldCommandSettings.Any())
                    {
                        unfoldCommandSettings.ForEach(cmd =>
                            {
                                tracer.Shield(() =>
                                    {
                                        if (e.DomainProperty.Id == CommandSettings.PropertiesDomainPropertyId)
                                        {
                                            SyncNameExtension.EnsureSyncNameExtensionAutomation(commandSettings.Owner);
                                        }
                                    }, Resources.UnfoldVsCommandChangeRule_ErrorSyncNameFailed, cmd.Name);
                            });
                    }
                }
            }
        }
    }
}