using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Globalization;
using Microsoft.VisualStudio.Shell;
using NuPattern.ComponentModel;
using NuPattern.ComponentModel.Design;
using NuPattern.Runtime.Properties;

namespace NuPattern.Runtime.Bindings.Design
{
    /// <summary>
    /// Defines a wrapper over <see cref="PropertyBindingSettings"/> to be used in the property grid.
    /// </summary>
    [TypeDescriptionProvider(typeof(DesignPropertyTypeDescriptionProvider))]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class DesignProperty : INotifyPropertyChanged
    {
        /// <summary>
        /// Handles the property changed event
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = (sender, args) => { };

        private Type type;

        /// <summary>
        /// Creates a new instance of the <see cref="DesignProperty"/> class.
        /// </summary>
        public DesignProperty(IPropertyBindingSettings propertySettings, Type type, Attribute[] attributes)
            : this(propertySettings)
        {
            Guard.NotNull(() => type, type);
            Guard.NotNull(() => attributes, attributes);

            this.Type = type;
            this.Attributes = attributes;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="DesignProperty"/> class.
        /// </summary>
        public DesignProperty(IPropertyBindingSettings propertySettings)
        {
            Guard.NotNull(() => propertySettings, propertySettings);

            this.Settings = propertySettings;
            this.ValueConverter = new Lazy<TypeConverter>(() =>
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

        /// <summary>
        /// Gets or sets the design value provider.
        /// </summary>
        [DescriptionResource("DesignProperty_ValueProviderDescription", typeof(Resources))]
        [DisplayNameResource("DesignProperty_ValueProviderDisplayName", typeof(Resources))]
        [Editor(typeof(StandardValuesEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(DesignValueProviderTypeConverter))]
        public virtual DesignValueProvider ValueProvider
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

        /// <summary>
        /// Gets or sets the attributes for the property
        /// </summary>
        internal Attribute[] Attributes { get; set; }

        /// <summary>
        /// Gets or sets the property settings
        /// </summary>
        [Browsable(false)]
        public IPropertyBindingSettings Settings { get; private set; }

        /// <summary>
        /// Gets or sets the value converter.
        /// </summary>
        protected Lazy<TypeConverter> ValueConverter { get; set; }

        /// <summary>
        /// Gets or sets the type of the property
        /// </summary>
        [Browsable(false)]
        internal Type Type
        {
            get { return this.type; }
            set { this.type = Type.GetType(value.AssemblyQualifiedName) ?? TypeDescriptor.GetProvider(value).GetRuntimeType(value); }
        }

        /// <summary>
        /// Gets the value of the property.
        /// </summary>
        public virtual object GetValue()
        {
            var context = new SimpleTypeDescriptorContext { Instance = this.Settings, ServiceProvider = GetServiceProvider() };
            if (this.Settings.HasValue())
            {
                return this.ValueConverter.Value.CanConvertFrom(context, typeof(string))
                           ? this.ValueConverter.Value.ConvertFromString(context, this.Settings.Value)
                           : string.Empty;
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Sets the value of this property.
        /// </summary>
        public virtual void SetValue(object value)
        {
            var context = new SimpleTypeDescriptorContext { Instance = this.Settings, ServiceProvider = GetServiceProvider() };
            this.Settings.Value = this.ValueConverter.Value.CanConvertTo(context, typeof(string)) ? this.ValueConverter.Value.ConvertToString(context, value) : string.Empty;
        }

        /// <summary>
        /// Returns the string representation of the property value.
        /// </summary>
        /// <returns></returns>
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

        private IServiceProvider GetServiceProvider()
        {
            return ServiceProvider.GlobalProvider;
        }
    }
}
