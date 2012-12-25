using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using NuPattern.Extensibility;
using NuPattern.Extensibility.Binding;
using NuPattern.Runtime.Schema.Properties;

namespace NuPattern.Runtime.Schema
{
    /// <summary>
    /// Customizations for the PropertySchema class.
    /// </summary>
    [TypeDescriptionProvider(typeof(PropertySchemaTypeDescriptorProvider))]
    public partial class PropertySchema
    {
        private PropertyValueProviderBindingSettings valueProviderSettings;
        private BoundProperty defaultValueProperty;
        private IBindingSettings[] validationSettings;
        internal static readonly IEnumerable<PropertyValueType> PropertyValueTypes = new[]        
        { 
            new PropertyValueType{ DataType=typeof(String), DisplayName = Resources.PropertySchema_PropertyDataTypes_String}, 
            new PropertyValueType{ DataType=typeof(Boolean), DisplayName = Resources.PropertySchema_PropertyDataTypes_Boolean}, 
            new PropertyValueType{ DataType=typeof(DateTime), DisplayName = Resources.PropertySchema_PropertyDataTypes_DateTime}, 
            new PropertyValueType{ DataType=typeof(Int64), DisplayName = Resources.PropertySchema_PropertyDataTypes_Int}, 
            new PropertyValueType{ DataType=typeof(UInt64), DisplayName = Resources.PropertySchema_PropertyDataTypes_UInt}, 
            new PropertyValueType{ DataType=typeof(Double), DisplayName = Resources.PropertySchema_PropertyDataTypes_Double}, 
            new PropertyValueType{ DataType=typeof(Decimal), DisplayName = Resources.PropertySchema_PropertyDataTypes_Decimal}, 
            new PropertyValueType{ DataType=typeof(Guid), DisplayName = Resources.PropertySchema_PropertyDataTypes_Guid}, 
            new PropertyValueType{ DataType=typeof(Int16)}, 
            new PropertyValueType{ DataType=typeof(Int32)}, 
            new PropertyValueType{ DataType=typeof(UInt16)}, 
            new PropertyValueType{ DataType=typeof(UInt32)}, 
            new PropertyValueType{ DataType=typeof(Single)}, 
            new PropertyValueType{ DataType=typeof(Byte)}, 
            new PropertyValueType{ DataType=typeof(SByte)}, 
            new PropertyValueType{ DataType=typeof(Char)}, 
        };

        /// <summary>
        /// Gets the validation settings.
        /// </summary>
        public IEnumerable<IBindingSettings> ValidationSettings
        {
            get { return this.validationSettings ?? (this.validationSettings = this.GetValidationSettings()); }
            set
            {
                if (this.validationSettings != null)
                    this.validationSettings.ForEach(binding => binding.PropertyChanged -= OnSaveRules);

                if (value != null)
                {
                    this.validationSettings = value.ToArray();
                    MonitorValidationRules();
                    SaveValidationRules();
                }
                else
                {
                    this.validationSettings = new ValidationBindingSettings[0];
                }
            }
        }

        /// <summary>
        /// Gets or sets the default value settings.
        /// </summary>
        public IPropertyBindingSettings DefaultValue
        {
            get { return this.DefaultValueProperty.Settings; }
            set { this.DefaultValueProperty.Settings = value; }
        }

        /// <summary>
        /// Gets or sets the value provider settings.
        /// </summary>
        public IValueProviderBindingSettings ValueProvider
        {
            get
            {
                if (this.valueProviderSettings == null)
                {
                    this.valueProviderSettings = new PropertyValueProviderBindingSettings(() => this.RawValueProvider, r => this.RawValueProvider = r);
                }

                return this.valueProviderSettings.Value;
            }
            set
            {
                if (this.valueProviderSettings == null)
                {
                    this.valueProviderSettings = new PropertyValueProviderBindingSettings(() => this.RawValueProvider, r => this.RawValueProvider = r);
                }

                this.valueProviderSettings.Value = value;
            }
        }

        /// <summary>
        /// Returns the displayed text of the property
        /// </summary>
        /// <returns></returns>
        internal string GetDisplayText()
        {
            var type = this.Type;
            var propertyValue = PropertyValueTypes.Where(p => p.DataType.FullName.Equals(this.Type)).FirstOrDefault();
            type = (propertyValue != null) ? propertyValue.DisplayName : type;

            return string.Format(CultureInfo.CurrentCulture,
                Resources.PropertySchema_DisplayTextFormat,
                this.Name,
                type);
        }

        private BoundProperty DefaultValueProperty
        {
            get
            {
                return this.defaultValueProperty ??
                    (this.defaultValueProperty = new BoundProperty(
                        Reflector<IPropertySchema>.GetPropertyName(x => x.DefaultValue),
                        () => this.RawDefaultValue,
                        s => this.RawDefaultValue = s));
            }
        }

        private IBindingSettings[] GetValidationSettings()
        {
            if (string.IsNullOrEmpty(this.RawValidationRules))
            {
                return new IBindingSettings[0];
            }

            this.validationSettings = BindingSerializer.Deserialize<ValidationBindingSettings[]>(this.RawValidationRules);
            MonitorValidationRules();

            return this.validationSettings;
        }

        private void MonitorValidationRules()
        {
            // Subscribe to changes in the settings so that we can keep them in sync in the state.
            this.validationSettings
                .ForEach(binding => binding.PropertyChanged += OnSaveRules);
        }

        private void OnSaveRules(object sender, EventArgs args)
        {
            SaveValidationRules();
        }

        private void SaveValidationRules()
        {
            this.RawValidationRules = BindingSerializer.Serialize(this.validationSettings);
        }
    }
}