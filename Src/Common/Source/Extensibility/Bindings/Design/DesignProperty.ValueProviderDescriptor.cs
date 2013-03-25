using System.ComponentModel;
using NuPattern.ComponentModel.Design;

namespace NuPattern.Extensibility.Bindings.Design
{
    /// <summary>
    /// Property descriptor for the ValueProvider property of a <see cref="DesignProperty"/>.
    /// </summary>
    internal class DesignPropertyValueProviderDescriptor : DelegatingPropertyDescriptor
    {
        /// <summary>
        /// Creates a new instance of the <see cref="DesignPropertyValueProviderDescriptor"/> class.
        /// </summary>
        /// <param name="innerDescriptor"></param>
        public DesignPropertyValueProviderDescriptor(PropertyDescriptor innerDescriptor)
            : base(innerDescriptor)
        {
        }

        /// <summary>
        /// Whether the properety can be reset.
        /// </summary>
        public override bool CanResetValue(object component)
        {
            var property = (DesignProperty)component;
            return property.ValueProvider != null;
        }

        /// <summary>
        /// Resets the property value.
        /// </summary>
        public override void ResetValue(object component)
        {
            var property = (DesignProperty)component;
            property.ValueProvider = null;
        }
    }
}
