using Microsoft.VisualStudio.Patterning.Library.Commands;
using Microsoft.VisualStudio.Patterning.Extensibility;

namespace Microsoft.VisualStudio.Patterning.Library.Automation.Rules
{
	[CommandChangeRule(typeof(UnfoldVsTemplateCommand))]
	class UnfoldVsTemplateCommandChangeRule : ICommandChangeRule
	{
		public void Change(VisualStudio.Modeling.ElementPropertyChangedEventArgs e)
		{
			Guard.NotNull(() => e, e);

			var property = e.ModelElement as PropertySettings;
			if (property != null)
			{
				if (property.Name == Reflector<UnfoldVsTemplateCommand>.GetPropertyName(u => u.SyncName))
				{
					SyncNameExtension.EnsureSyncNameExtensionAutomation(property.CommandSettings.Owner);
				}
			}
		}
	}
}
