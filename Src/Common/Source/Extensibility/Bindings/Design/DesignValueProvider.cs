using System.ComponentModel;
using NuPattern.Runtime.Bindings;

namespace NuPattern.Extensibility.Bindings.Design
{
    /// <summary>
    /// Defines a value provider wrapper for the property grid.
    /// </summary>
    [TypeDescriptionProvider(typeof(DesignValueProviderTypeDescriptionProvider))]
    public class DesignValueProvider
    {
        /// <summary>
        /// Creates a new instance of the <see cref="DesignValueProvider"/> class.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="valueProvider">The value provider.</param>
        public DesignValueProvider(DesignProperty property, IValueProviderBindingSettings valueProvider)
            : this(property, (valueProvider != null && !string.IsNullOrEmpty(valueProvider.TypeId)) ? valueProvider.TypeId : string.Empty)
        {
            this.ValueProvider = valueProvider;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="DesignValueProvider"/> class.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="name">The name of the property.</param>
        public DesignValueProvider(DesignProperty property, string name)
        {
            Guard.NotNull(() => property, property);
            Guard.NotNull(() => name, name);

            this.DesignProperty = property;
            this.Name = name;
        }

        /// <summary>
        /// Gets the property
        /// </summary>
        internal DesignProperty DesignProperty { get; private set; }

        /// <summary>
        /// Gets the value provider.
        /// </summary>
        internal IValueProviderBindingSettings ValueProvider { get; set; }

        /// <summary>
        /// Gets the name of the value provider type.
        /// </summary>
        internal string Name { get; private set; }
    }
}