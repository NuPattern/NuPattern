using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NuPattern.Runtime
{
    /// <summary>
    /// Represents the settings needed to create a runtime dynamic binding object.
    /// </summary>
    public interface IBindingSettings : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the identifier for the runtime implementation type of the binding.
        /// </summary>
        string TypeId { get; set; }

        /// <summary>
        /// Gets the optional property bindings.
        /// </summary>
        IEnumerable<IPropertyBindingSettings> Properties { get; }

        /// <summary>
        /// Adds a new property to the property bindings collection.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="propertyType">The type of the property.</param>
        /// <returns>The newly created property</returns>
        IPropertyBindingSettings AddProperty(string name, Type propertyType);

        /// <summary>
        /// Removes all properties in the property binding collection.
        /// </summary>
        void ClearProperties();
    }
}
