using NuPattern.Extensibility;
using NuPattern.Library.Commands;

namespace NuPattern.Library.Automation.Rules
{
	[CommandChangeRule(typeof(UnfoldVsTemplateCommand))]
	class UnfoldVsTemplateCommandChangeRule : ICommandChangeRule
	{
		public void Change(Microsoft.VisualStudio.Modeling.ElementPropertyChangedEventArgs e)
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
