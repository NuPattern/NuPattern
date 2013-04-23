using System.Collections.Generic;
using System.ComponentModel;

namespace NuPattern.Runtime.Bindings
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
        IList<IPropertyBindingSettings> Properties { get; }
    }
}
