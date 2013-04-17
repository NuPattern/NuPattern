using NuPattern.ComponentModel.Composition;

namespace NuPattern.Runtime
{
    /// <summary>
    /// Specifies that the component is a provided <see cref="IValueProvider"/>.
    /// </summary>
    public class ValueProviderAttribute : FeatureComponentAttribute
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ValueProviderAttribute"/> class.
        /// </summary>
        public ValueProviderAttribute()
            : base(typeof(IValueProvider))
        {
        }
    }
}
