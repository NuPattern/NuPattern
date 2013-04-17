namespace Microsoft.VisualStudio.TeamArchitect.PowerTools.Features
{
    /// <summary>
    /// A provider of a value
    /// </summary>
    public interface IValueProvider
    {
        /// <summary>
        /// Evaluates the provider and returns the value.
        /// </summary>
        object Evaluate();
    }
}