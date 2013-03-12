using System.Collections.Generic;
using System.ComponentModel;
using NuPattern.Extensibility.Binding;
using NuPattern.Runtime;

namespace NuPattern.Library.Automation
{
    /// <summary>
    /// Double-derived class to allow easier code customization.
    /// </summary>
    [TypeDescriptionProvider(typeof(CommandSettingsDescriptionProvider))]
    public partial class CommandSettings : IBindingSettings
    {
        private List<IPropertyBindingSettings> propertySettings;

        /// <summary>
        /// Creates the runtime automation element for this setting element.
        /// </summary>
        public IAutomationExtension CreateAutomation(IProductElement owner)
        {
            return new CommandAutomation(owner, this);
        }

        /// <summary>
        /// Gets the classification of these settings.
        /// </summary>
        public AutomationSettingsClassification Classification
        {
            get { return AutomationSettingsClassification.General; }
        }

        /// <summary>
        /// Gets the properties for the binding. 
        /// </summary>
        IList<IPropertyBindingSettings> IBindingSettings.Properties
        {
            get { return this.propertySettings ?? (this.propertySettings = this.GetPropertySettings()); }
        }

        private List<IPropertyBindingSettings> GetPropertySettings()
        {
            if (string.IsNullOrEmpty(this.Properties))
            {
                return new List<IPropertyBindingSettings>();
            }

            return BindingSerializer.Deserialize<List<IPropertyBindingSettings>>(this.Properties);
        }
    }
}