using NuPattern.ComponentModel.Composition;

namespace NuPattern.Runtime
{
    /// <summary>
    /// Specifies that the component is a provided <see cref="ICondition"/>.
    /// </summary>
    public class ConditionAttribute : FeatureComponentAttribute
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ConditionAttribute"/> class.
        /// </summary>
        public ConditionAttribute()
            : base(typeof(ICondition))
        {
        }
    }
}