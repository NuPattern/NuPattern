using System;

namespace NuPattern.Runtime.Bindings
{
    /// <summary>
    /// Provides a backing state and automatically-initializing property that serializes to a 
    /// string backing field using the <see cref="BindingSerializer"/>.
    /// </summary>
    public class BoundProperty
    {
        private string propertyName;
        private Func<string> getValue;
        private Action<string> setValue;
        private IPropertyBindingSettings settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundProperty"/> class.
        /// </summary>
        public BoundProperty(string propertyName, Func<string> getter, Action<string> setter)
        {
            Guard.NotNullOrEmpty(() => propertyName, propertyName);
            Guard.NotNull(() => getter, getter);
            Guard.NotNull(() => setter, setter);

            this.propertyName = propertyName;
            this.getValue = getter;
            this.setValue = setter;
        }

        /// <summary>
        /// Gets or sets the binding settings for the property. This property never returns <see langword="null"/>.
        /// </summary>
        public IPropertyBindingSettings Settings
        {
            get { return this.settings ?? (this.settings = this.CreateSettings()); }
            set
            {
                if (this.settings != value)
                {
                    if (this.settings != null)
                        this.settings.PropertyChanged -= OnSettingsChanged;

                    this.settings = value;

                    if (this.settings != null)
                    {
                        this.settings.PropertyChanged += OnSettingsChanged;
                        SaveSettings();
                    }
                    else
                    {
                        this.setValue(string.Empty);
                    }
                }
            }
        }

        private IPropertyBindingSettings CreateSettings()
        {
            IPropertyBindingSettings settings;

            if (string.IsNullOrEmpty(getValue()))
            {
                settings = new PropertyBindingSettings
                {
                    Name = this.propertyName,
                    Value = BindingSettings.Empty,
                    ValueProvider = null,
                };
            }
            else
            {
                // Be backwards compatible to ease migration.
                try
                {
                    settings = BindingSerializer.Deserialize<PropertyBindingSettings>(getValue());
                }
                catch (BindingSerializationException)
                {
                    // This would happen if the value was a raw value from before we had a binding.
                    // Consider it the property value.
                    settings = new PropertyBindingSettings
                    {
                        Name = this.propertyName,
                        Value = this.getValue(),
                    };

                    // Persist updated value.
                    this.setValue(BindingSerializer.Serialize(settings));
                }
            }

            // Automatically serialize whenever something is changed in the binding.
            settings.PropertyChanged += OnSettingsChanged;

            return settings;
        }

        private void OnSettingsChanged(object sender, EventArgs args)
        {
            SaveSettings();
        }

        private void SaveSettings()
        {
            if (!this.settings.IsConfigured())
            {
                this.setValue(BindingSettings.Empty);
            }
            else
            {
                this.setValue(BindingSerializer.Serialize(this.settings));
            }
        }
    }
}
