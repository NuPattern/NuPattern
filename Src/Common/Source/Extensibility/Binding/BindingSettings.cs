using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Design;
using NuPattern.Extensibility.Properties;
using NuPattern.Runtime;

namespace NuPattern.Extensibility.Binding
{
    /// <summary>
    /// Default implementation of <see cref="IBindingSettings"/>.
    /// </summary>
    [DataContract]
    public class BindingSettings : IBindingSettings
    {
        /// <summary>
        /// Represents an empty binding
        /// </summary>
        public static readonly string Empty = string.Empty;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = (sender, args) => { };

        private string typeId;
        private ObservableCollection<IPropertyBindingSettings> properties;

        /// <summary>
        /// Initializes a new instance of the <see cref="BindingSettings"/> class.
        /// </summary>
        public BindingSettings()
        {
            this.properties = new ObservableCollection<IPropertyBindingSettings>();
            this.properties.CollectionChanged += OnPropertiesChanged;
            ((INotifyPropertyChanged)this.properties).PropertyChanged += OnNestedChanged;
        }

        /// <summary>
        /// Gets or sets the identifier for the runtime implementation type of the binding.
        /// </summary>
        [DataMember]
        [Editor(typeof(StandardValuesEditor), typeof(UITypeEditor))]
        [DescriptionResource("BindingSettings_TypeIdDescription", typeof(Resources))]
        [DisplayNameResource("BindingSettings_TypeIdDisplayName", typeof(Resources))]
        [RefreshProperties(RefreshProperties.All)]
        public virtual string TypeId
        {
            get { return this.typeId; }
            set
            {
                if (this.IsConfigured() && !this.typeId.Equals(value, StringComparison.OrdinalIgnoreCase))
                {
                    this.Properties.Clear();
                }

                this.typeId = value;
                this.NotifyChanged(this.PropertyChanged, x => x.TypeId);
            }
        }

        /// <summary>
        /// Gets the optional property bindings.
        /// </summary>
        [DataMember]
        public IList<IPropertyBindingSettings> Properties
        {
            get { return this.properties; }
        }

        /// <summary>
        /// Gets the property changed handler to use to call  NotifyChanged extension method.
        /// </summary>
        protected PropertyChangedEventHandler PropertyChangedHandler
        {
            get { return this.PropertyChanged; }
        }

        private void OnPropertiesChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.OldItems != null)
            {
                foreach (var removed in args.OldItems.OfType<INotifyPropertyChanged>())
                {
                    removed.PropertyChanged -= OnNestedChanged;
                }
            }

            if (args.NewItems != null)
            {
                foreach (var added in args.NewItems.OfType<INotifyPropertyChanged>())
                {
                    added.PropertyChanged += OnNestedChanged;
                }
            }
        }

        private void OnNestedChanged(object sender, PropertyChangedEventArgs e)
        {
            this.NotifyChanged(this.PropertyChanged, x => x.Properties);
        }
    }
}