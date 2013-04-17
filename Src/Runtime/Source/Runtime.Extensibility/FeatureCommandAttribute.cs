using NuPattern.ComponentModel.Composition;

namespace NuPattern.Runtime
{
    /// <summary>
    /// Specifies that the component is a provided <see cref="IFeatureCommand"/>.
    /// </summary>
    public class FeatureCommandAttribute : FeatureComponentAttribute
    {
        /// <summary>
        /// Creates a new instance of the <see cref="FeatureCommandAttribute"/> class.
        /// </summary>
        public FeatureCommandAttribute()
            : base(typeof(IFeatureCommand))
        {
        }
    }
}