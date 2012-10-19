using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.Patterning.Runtime;

namespace Microsoft.VisualStudio.Patterning.Library.Automation
{
	[RuleOn(typeof(PropertySettings), FireTime = TimeToFire.TopLevelCommit)]
	class PropertySettingsChangeRule : ChangeRule
	{
		public override void ElementPropertyChanged(ElementPropertyChangedEventArgs e)
		{
			Guard.NotNull(() => e, e);

			if (!e.ModelElement.Store.TransactionManager.CurrentTransaction.IsSerializing)
			{
				base.ElementPropertyChanged(e);
				var componentModel = e.ModelElement.Store.GetService<SComponentModel, IComponentModel>();
				var changeRules = componentModel.DefaultExportProvider.GetExports<ICommandChangeRule, ICommandChangeRuleMetadata>()
					.Where(r => r.Metadata.CommandType.ToString() == ((PropertySettings)e.ModelElement).CommandSettings.TypeId);

				foreach (var rule in changeRules)
				{
					rule.Value.Change(e);
				}
			}
		}
	}
}
