using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using NuPattern.Extensibility;
using NuPattern.Runtime;

namespace NuPattern.Library.Automation
{
    /// <summary>
    /// Double-derived class to allow easier code customization.
    /// </summary>
    [TypeDescriptionProvider(typeof(CommandSettingsTypeDescriptionProvider))]
    public partial class CommandSettings : IBindingSettings
    {
        /// <summary>
        /// Creates the runtime automation element for this setting element.
        /// </summary>
        public IAutomationExtension CreateAutomation(IProductElement owner)
        {
            return new CommandAutomation(owner, this);
        }

        /// <summary>
        /// Gets the binding properties.
        /// </summary>
        IEnumerable<IPropertyBindingSettings> IBindingSettings.Properties
        {
            get
            {
                return new ReadOnlyCollection<IPropertyBindingSettings>(this.Properties
                    .Where(prop => prop.ParentProvider == null)
                    .Cast<IPropertyBindingSettings>()
                    .ToList());
            }
        }

        /// <summary>
        /// Adds a new property to the properties collection.
        /// </summary>
        /// <param name="name">The name of the property</param>
        /// <param name="propertyType">The type of the property</param>
        /// <returns>The newly created property</returns>
        IPropertyBindingSettings IBindingSettings.AddProperty(string name, Type propertyType)
        {
            return CreateNewPropertySetting(name, propertyType);
        }

        /// <summary>
        /// Removes all properties in the property binding collection.
        /// </summary>
        void IBindingSettings.ClearProperties()
        {
            this.Properties.Clear();
        }

        /// <summary>
        /// Gets the classification of these settings.
        /// </summary>
        public AutomationSettingsClassification Classification
        {
            get { return AutomationSettingsClassification.General; }
        }

        private PropertySettings CreateNewPropertySetting(string name, Type propertyType)
        {
            using (var tx = this.Store.TransactionManager.BeginTransaction("Configuring Property Settings"))
            {
                var settings = this.Create<PropertySettings>();

                // Set the Binding Name
                ((IPropertyBindingSettings)settings).Name = name;

                if (propertyType.IsValueType)
                {
                    // We need to provide a default value
                    settings.Value = Activator.CreateInstance(propertyType).ToString();
                }

                tx.Commit();

                return settings;
            }
        }
    }
}