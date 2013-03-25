namespace NuPattern.Runtime
{
    /// <summary>
    /// Evaluates a property by name by probing the target object 
    /// via TypeDescriptor, runtime variable properties and reflection.
    /// </summary>
    public interface IPropertyEvaluator
    {
        /// <summary>
        /// Evaluates the property for the given target object.
        /// </summary>
        /// <returns>The property value or <see langword="null"/> if the property is not found</returns>
        object Evaluate(object target, string propertyName);
    }
}
