using System.ComponentModel;

namespace NuPattern.Runtime.Bindings
{
    /// <summary>
    /// Represents the settings needed to create a runtime dynamic property binding.
    /// </summary>
    public interface IPropertyBindingSettings : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets the name of the property.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the optional fixed value of the property.
        /// </summary>
        string Value { get; set; }

        /// <summary>
        /// Gets or sets the optional value provider to dynamically evaluate the property value at runtime.
        /// </summary>
        IValueProviderBindingSettings ValueProvider { get; set; }
    }
}
