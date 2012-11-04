using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.Patterning.Library.Properties;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;

namespace Microsoft.VisualStudio.Patterning.Library.Automation
{
	/// <summary>
	/// Change rule for <see cref="TemplateSettings"/> domain class.
	/// </summary>
	[RuleOn(typeof(TemplateSettings), FireTime = TimeToFire.TopLevelCommit)]
	public class TemplateSettingsChangeRule : ChangeRule
	{
		private static readonly ITraceSource tracer = Tracer.GetSourceFor<TemplateSettingsChangeRule>();

		/// <summary>
		/// Handles the property change event for the settings.
		/// </summary>
		/// <param name="e">The event args.</param>
		public override void ElementPropertyChanged(ElementPropertyChangedEventArgs e)
		{
			Guard.NotNull(() => e, e);

			if (e.DomainProperty.Id == TemplateSettings.SyncNameDomainPropertyId)
			{
				var settings = e.ModelElement as ITemplateSettings;
				if (settings != null)
				{
					if (!e.ModelElement.Store.TransactionManager.CurrentTransaction.IsSerializing)
					{
						tracer.Shield(() =>
						{
							SyncNameExtension.EnsureSyncNameExtensionAutomation(settings.Owner);
						},
						Resources.TemplateSettingsChangeRule_ChangeHiddenFailed, settings.TemplateUri);
					}
				}
			}
		}
	}
}
