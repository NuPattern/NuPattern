using System.Collections.ObjectModel;
using System.ComponentModel;

namespace NuPattern.Library.Automation
{
    /// <summary>
    /// Provides a <see cref="PropertyDescriptor"/> that displays a <see cref="Collection{T}"/> property binding.
    /// </summary>
    public class DesignCollectionPropertyDescriptor<T> : NuPattern.Extensibility.Binding.DesignCollectionPropertyDescriptor<CommandSettings> where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DesignCollectionPropertyDescriptor{T}"/> class.
        /// </summary>
        /// <param name="descriptor">The original underlying property descriptor.</param>
        public DesignCollectionPropertyDescriptor(PropertyDescriptor descriptor)
            : base(descriptor)
        {
        }

        /// <summary>
        /// Retrieves the current property from storage.
        /// </summary>
        protected override Runtime.IPropertyBindingSettings GetPropertySettings(object component)
        {
            var settings = component as T;
            return (settings != null) ? NuPattern.Library.Automation.DesignPropertyDescriptor.EnsurePropertySettings(component, this.Name, this.PropertyType) : null;
        }

    }
}
