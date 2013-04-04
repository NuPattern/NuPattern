using System.ComponentModel;
using System.Runtime.Serialization;
using NuPattern.ComponentModel;

namespace NuPattern.Runtime.Bindings
{
    /// <summary>
    /// Default implementation of <see cref="IPropertyBindingSettings"/>.
    /// </summary>
    [DataContract]
    public class PropertyBindingSettings : IPropertyBindingSettings
    {
        /// <summary>
        /// Raised when the value of the property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = (sender, args) => { };

        private string name;
        private string propertyValue;
        private IValueProviderBindingSettings valueProvider;

        /// <summary>
        /// Gets or sets the name of the property.
        /// </summary>
        [DataMember]
        public string Name
        {
            get { return this.name; }
            set { this.name = value; this.NotifyChanged(this.PropertyChanged, x => x.Name); }
        }

        /// <summary>
        /// Gets or sets the optional fixed value of the property.
        /// </summary>
        [DataMember]
        public string Value
        {
            get { return this.propertyValue; }
            set { this.propertyValue = value; this.NotifyChanged(this.PropertyChanged, x => x.Value); }
        }

        /// <summary>
        /// Gets or sets the optional value provider to dynamically evaluate the property value at runtime.
        /// </summary>
        [DataMember]
        public IValueProviderBindingSettings ValueProvider
        {
            get { return this.valueProvider; }
            set
            {
                if (this.valueProvider != value)
                {
                    this.SubscribeNested(this.PropertyChanged, this.OnNestedChanged, this.valueProvider, value);
                    this.valueProvider = value;
                    this.NotifyChanged(this.PropertyChanged, x => x.ValueProvider);
                }
            }
        }

        internal void OnNestedChanged(object sender, PropertyChangedEventArgs e)
        {
            this.NotifyChanged(this.PropertyChanged, x => x.ValueProvider);
        }
    }
}
