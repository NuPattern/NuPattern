using System.ComponentModel;
using System.Globalization;
using System.Linq;
using NuPattern.Extensibility;
using NuPattern.Runtime;

namespace NuPattern.Library.Automation
{
    /// <content>
    /// Adds the implementation of <see cref="IPropertyBindingSettings"/>.
    /// </content>
    partial class PropertySettings : IPropertyBindingSettings
    {
        private const char NameDelimiter = '.';

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
            set { CreateNewValueProviderSettings(value); }
        }

        /// <summary>
        /// Gets the name of the property binding.
        /// </summary>
        string IPropertyBindingSettings.Name
        {
            get
            {
                return GetInternalPropertyName(this.Name);
            }
            set
            {
                this.Name = CreateInternalPropertyName(this, value);
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

        private void CreateNewValueProviderSettings(IValueProviderBindingSettings settings)
        {
            using (var transaction = this.Store.TransactionManager.BeginTransaction("Creating Value Provider"))
            {
                if (settings != null)
                {
                    if (this.ValueProvider == null)
                    {
                        this.Create<ValueProviderSettings>();
                    }

                    this.ValueProvider.TypeId = settings.TypeId;
                }
                else if (this.ValueProvider != null)
                {
                    this.ValueProvider.Delete();
                }

                transaction.Commit();
            }
        }

        /// <summary>
        /// Creates a name for the property in the VP hierarchy
        /// </summary>
        /// <remarks>
        /// A CommandSettings need to save all properties of all nested ValueProviders at same level in Store.
        /// Therefore, the name of the PropertySettings needs to be unique in the scope of the whole CommandSettings.
        /// </remarks>
        internal static string CreateInternalPropertyName(PropertySettings settings, string bindingPropertyName)
        {
            var parentValueProvider = settings.ParentProvider;

            if (parentValueProvider == null)
            {
                return bindingPropertyName;
            }

            return
                string.Format(CultureInfo.InvariantCulture, "{0}.{1}.{2}",
                              parentValueProvider.OwnerProperty.Name,
                              parentValueProvider.TypeId.Split(NameDelimiter).Last(),
                              bindingPropertyName);
        }

        /// <summary>
        /// Normalizes the built name from <see cref="CreateInternalPropertyName"/>.
        /// </summary>
        private static string GetInternalPropertyName(string name)
        {
            return name.Split(NameDelimiter).Last();
        }
    }
}