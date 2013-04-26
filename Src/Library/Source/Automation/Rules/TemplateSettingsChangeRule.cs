using Microsoft.VisualStudio.Modeling;
using NuPattern.Diagnostics;
using NuPattern.Library.Properties;
using NuPattern.VisualStudio;

namespace NuPattern.Library.Automation
{
    /// <summary>
    /// Change rule for <see cref="TemplateSettings"/> domain class.
    /// </summary>
    [RuleOn(typeof(TemplateSettings), FireTime = TimeToFire.TopLevelCommit)]
    internal class TemplateSettingsChangeRule : ChangeRule
    {
        private static readonly ITracer tracer = Tracer.Get<TemplateSettingsChangeRule>();

        /// <summary>
        /// Handles the property change event for the settings.
        /// </summary>
        /// <param name="e">The event args.</param>
        public override void ElementPropertyChanged(ElementPropertyChangedEventArgs e)
        {
            Guard.NotNull(() => e, e);

            if (e.DomainProperty.Id == TemplateSettings.SyncNameDomainPropertyId)
            {
                var template = e.ModelElement as TemplateSettings;
                if (template != null && template.Extends != null)
                {
                    if (!e.ModelElement.Store.TransactionManager.CurrentTransaction.IsSerializing)
                    {
                        tracer.Shield(() =>
                        {
                            SyncNameExtension.EnsureSyncNameExtensionAutomation(template.Owner);
                        },
                        Resources.TemplateSettingsChangeRule_ErrorSyncNameFailed, template.Name);
                    }
                }
            }
        }
    }
}
