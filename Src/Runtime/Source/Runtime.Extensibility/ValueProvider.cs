
namespace NuPattern.Runtime
{
    /// <summary>
    /// A provider of a value
    /// </summary>
    [ValueProvider]
    public abstract class ValueProvider : IValueProvider
    {
        /// <summary>
        /// Evaluates the provider and returns the value.
        /// </summary>
        public abstract object Evaluate();
    }
}
