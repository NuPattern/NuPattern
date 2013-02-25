using System;
using System.ComponentModel;
using System.Linq;
using NuPattern.Runtime;

namespace NuPattern.Library.Automation
{
    /// <content>
    /// Adds the implementation of <see cref="IPropertyBindingSettings"/>.
    /// </content>
    public partial class PropertySettings : IPropertyBindingSettings
    {
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = (sender, args) => { };

        /// <summary>
        /// Gets the optional value provider to dynamically evaluate the property value at runtime.
        /// </summary>
        IValueProviderBindingSettings IPropertyBindingSettings.ValueProvider
        {
            get { return this.ValueProvider; }
            set
            {
                // Creating value provider for the DSL binding has to be done via the DSL API.
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        string IPropertyBindingSettings.Name
        {
            get
            {
                // We need to project the properties without our value-provider 
                // concatenated full name as built by DesignValueProviderTypeDescriptionProvider.GetPropertyName.
                // dots are invalid in a binding name, so it's safe to query for it and to the split.
                return this.Name.Split('.').Last();
            }
            set
            {
                this.Name = value;
            }
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
    }
}