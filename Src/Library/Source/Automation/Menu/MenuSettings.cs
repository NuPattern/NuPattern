using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.VisualStudio.Patterning.Extensibility.Binding;
using Microsoft.VisualStudio.Patterning.Runtime;

namespace Microsoft.VisualStudio.Patterning.Library.Automation
{
    /// <summary>
    /// Double-derived class to allow easier code customization.
    /// </summary>
    [TypeDescriptionProvider(typeof(MenuSettingsDescriptionProvider))]
    public partial class MenuSettings
    {
        private List<ConditionBindingSettings> conditionSettings;

        /// <summary>
        /// Gets the classification of these settings.
        /// </summary>
        public AutomationSettingsClassification Classification
        {
            get { return AutomationSettingsClassification.LaunchPoint; }
        }

        /// <summary>
        /// Gets the condition settings.
        /// </summary>
        public IEnumerable<IBindingSettings> ConditionSettings
        {
            get { return this.conditionSettings ?? (this.conditionSettings = this.GetConditionSettings()); }
        }

        /// <summary>
        /// Creates the runtime automation element for this setting element.
        /// </summary>
        public IAutomationExtension CreateAutomation(IProductElement owner)
        {
            return new MenuAutomation(owner, this);
        }

        private List<ConditionBindingSettings> GetConditionSettings()
        {
            if (string.IsNullOrEmpty(this.Conditions))
            {
                return new List<ConditionBindingSettings>();
            }

            return BindingSerializer.Deserialize<List<ConditionBindingSettings>>(this.Conditions);
        }
    }
}