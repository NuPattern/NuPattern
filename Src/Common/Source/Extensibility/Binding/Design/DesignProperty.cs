using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Globalization;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Design;
using NuPattern.Extensibility.Properties;
using NuPattern.Runtime;

namespace NuPattern.Extensibility.Binding
{
    /// <summary>
    /// Defines a wrapper over <see cref="PropertyBindingSettings"/> to be used in the property grid.
    /// </summary>
    [TypeDescriptionProvider(typeof(DesignPropertyTypeDescriptionProvider))]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    internal class DesignProperty : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = (sender, args) => { };

        private Lazy<TypeConverter> valueConverter;
        private Type type;

        public DesignProperty(IPropertyBindingSettings propertySettings)
        {
            this.Settings = propertySettings;
            this.valueConverter = new Lazy<TypeConverter>(() =>
            {
                var converter = this.Attributes.FindCustomTypeConverter();
                if (converter == null ||
                    // The custom converter must be capable of converting to AND from string 
                    // for proper interaction with the property grid.
                    !converter.CanConvertFrom(typeof(string)) ||
                    !converter.CanConvertTo(typeof(string)))
                {
                    converter = TypeDescriptor.GetConverter(this.Type);
                }

                return converter;
            });
        }

        [DescriptionResource("DesignProperty_ValueProviderDescription", typeof(Resources))]
        [DisplayNameResource("DesignProperty_ValueProviderDisplayName", typeof(Resources))]
        [Editor(typeof(StandardValuesEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(DesignValueProviderTypeConverter))]
        public DesignValueProvider ValueProvider
        {
            get
            {
                return this.Settings.ValueProvider != null ? new DesignValueProvider(this, this.Settings.ValueProvider) : null;
            }
            set
            {
                this.Settings.ValueProvider = (value != null) ? value.ValueProvider : null;
                this.NotifyChanged(this.PropertyChanged, x => x.ValueProvider);
            }
        }

        internal Attribute[] Attributes { get; set; }

        internal IPropertyBindingSettings Settings { get; private set; }

        internal Type Type
        {
            get { return this.type; }
            set { this.type = Type.GetType(value.AssemblyQualifiedName) ?? TypeDescriptor.GetProvider(value).GetRuntimeType(value); }
        }

        public object GetValue()
        {
            return this.Settings.HasValue()
                ? this.valueConverter.Value.ConvertFromString(this.Settings.Value)
                : null;
        }

        public void SetValue(object value)
        {
            this.Settings.Value = this.valueConverter.Value.ConvertToString(value);
        }

        public override string ToString()
        {
            if (this.Settings.HasValue())
            {
                return this.Settings.Value;
            }

            if (this.Settings.HasValueProvider())
            {
                return string.Format(CultureInfo.CurrentCulture, Resources.DesignProperty_ToStringValueProviderFormat,
                    this.Settings.ValueProvider.TypeId);
            }

            return string.Empty;
        }
    }
}
