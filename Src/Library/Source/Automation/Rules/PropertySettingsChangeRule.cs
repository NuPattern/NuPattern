using System.Linq;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Modeling;
using NuPattern.Runtime;

namespace NuPattern.Library.Automation
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
                if (componentModel != null)
                {
                    var settings = (PropertySettings)e.ModelElement;
                    var typeId = (settings.CommandSettings != null) ? settings.CommandSettings.TypeId :
                        ((settings.ParentProvider != null) ? settings.ParentProvider.TypeId : string.Empty);
                    if (!string.IsNullOrEmpty(typeId))
                    {
                        // Fire all ICommandChangeRule rules when the ValueProvider changes
                        var changeRules = componentModel.DefaultExportProvider
                                                        .GetExports<ICommandChangeRule, ICommandChangeRuleMetadata>()
                                                        .Where(r => r.Metadata.CommandType.ToString() == typeId);
                        foreach (var rule in changeRules)
                        {
                            rule.Value.Change(e);
                        }
                    }
                }
            }
        }
    }
}
