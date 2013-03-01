using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using NuPattern.Extensibility;
using NuPattern.Runtime;

namespace NuPattern.Library.Automation
{
    /// <content>
    /// Adds the implementation of <see cref="IValueProviderBindingSettings"/>.
    /// </content>
    public partial class ValueProviderSettings : IValueProviderBindingSettings
    {
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = (sender, args) => { };

        /// <summary>
        /// Gets the optional property bindings.
        /// </summary>
        IEnumerable<IPropertyBindingSettings> IBindingSettings.Properties
        {
            get
            {
                return new ReadOnlyCollection<IPropertyBindingSettings>(this.Properties
                    .Cast<IPropertyBindingSettings>()
                    .ToList());
            }
        }

        /// <summary>
        /// Adds a new property binding to the collection
        /// </summary>
        public IPropertyBindingSettings AddProperty(string name, Type propertyType)
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

        private PropertyChangeManager propertyChanges;

        /// <summary>
        /// Gets the manager for property change events.
        /// </summary>
        protected PropertyChangeManager PropertyChanges
        {
            get
            {
                if (this.propertyChanges == null)
                {
                    this.propertyChanges = new PropertyChangeManager(this);
                }

                return this.propertyChanges;
            }
        }

        private PropertySettings CreateNewPropertySetting(string name, Type propertyType)
        {
            using (var tx = this.Store.TransactionManager.BeginTransaction("Configuring Property Settings"))
            {
                var commandSettings = this.OwnerProperty.CommandSettings;

                // Create and add the property setting to ValueProviderSettings
                var settings = commandSettings.Create<PropertySettings>();
                this.Properties.Add(settings);

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
