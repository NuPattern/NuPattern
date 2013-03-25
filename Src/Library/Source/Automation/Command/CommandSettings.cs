using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using NuPattern.Runtime;
using NuPattern.Runtime.Bindings;

namespace NuPattern.Library.Automation
{
    /// <summary>
    /// Double-derived class to allow easier code customization.
    /// </summary>
    [TypeDescriptionProvider(typeof(CommandSettingsDescriptionProvider))]
    partial class CommandSettings : IBindingSettings
    {
        private ObservableCollection<IPropertyBindingSettings> propertySettings;

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

        private ObservableCollection<IPropertyBindingSettings> GetPropertySettings()
        {
            var properties = string.IsNullOrEmpty(this.Properties) ?
               new ObservableCollection<IPropertyBindingSettings>() :
               BindingSerializer.Deserialize<ObservableCollection<IPropertyBindingSettings>>(this.Properties);
            properties.CollectionChanged += (sender, args) =>
                {
                    if (args.OldItems != null)
                    {
                        args.OldItems.OfType<IPropertyBindingSettings>()
                            .ToList()
                            .ForEach(binding => binding.PropertyChanged -= OnSaveRules);
                    }
                    if (args.NewItems != null)
                    {
                        args.NewItems.OfType<IPropertyBindingSettings>()
                            .ToList()
                            .ForEach(binding => binding.PropertyChanged += OnSaveRules);
                    }
                };

            properties
                .ForEach(binding => binding.PropertyChanged += OnSaveRules);

            return properties;
        }

        private void OnSaveRules(object sender, EventArgs args)
        {
            this.Properties = BindingSerializer.Serialize(this.propertySettings);
        }
    }
}