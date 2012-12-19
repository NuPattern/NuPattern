using System;
using NuPattern.Extensibility.Binding;

namespace NuPattern.Runtime.Schema
{
    /// <summary>
    /// A delegate class to help ValueProvider properties manage their binding values.
    /// </summary>
    internal class PropertyValueProviderBindingSettings
    {
        private IValueProviderBindingSettings valueProviderSettings;
        private Func<string> rawValueGetter;
        private Action<string> rawValueSetter;

        /// <summary>
        /// Creates a new instance of the <see cref="PropertyValueProviderBindingSettings"/> class.
        /// </summary>
        public PropertyValueProviderBindingSettings(Func<string> rawValueGetter, Action<string> rawValueSetter)
        {
            Guard.NotNull(() => rawValueGetter, rawValueGetter);
            Guard.NotNull(() => rawValueSetter, rawValueSetter);

            this.rawValueGetter = rawValueGetter;
            this.rawValueSetter = rawValueSetter;
        }

        /// <summary>
        /// Returns empty settings.
        /// </summary>
        /// <returns></returns>
        public static IValueProviderBindingSettings Empty()
        {
            return new PropertyValueProviderBindingSettings(() => string.Empty, delegate(string x) { }).Value;
        }

        /// <summary>
        /// Gets or sets the value
        /// </summary>
        public IValueProviderBindingSettings Value
        {
            get
            {
                return this.valueProviderSettings ?? (this.valueProviderSettings = this.CreateValueProviderSettings());
            }
            set
            {
                if (this.valueProviderSettings != value)
                {
                    if (this.valueProviderSettings != null)
                        this.valueProviderSettings.PropertyChanged -= OnSaveValue;

                    this.valueProviderSettings = value;

                    if (this.valueProviderSettings != null)
                    {
                        this.valueProviderSettings.PropertyChanged += OnSaveValue;
                        SaveValue();
                    }
                    else
                    {
                        this.rawValueSetter(string.Empty);
                    }
                }
            }
        }

        private void OnSaveValue(object sender, EventArgs args)
        {
            SaveValue();
        }

        private void SaveValue()
        {
            this.rawValueSetter(BindingSerializer.Serialize(this.valueProviderSettings));
        }

        private ValueProviderBindingSettings CreateValueProviderSettings()
        {
            var settings = string.IsNullOrEmpty(this.rawValueGetter()) ?
                new ValueProviderBindingSettings() :
                BindingSerializer.Deserialize<ValueProviderBindingSettings>(this.rawValueGetter());

            // Automatically serialize whenever something is changed in the binding.
            settings.PropertyChanged += OnSaveValue;

            return settings;
        }

    }
}
