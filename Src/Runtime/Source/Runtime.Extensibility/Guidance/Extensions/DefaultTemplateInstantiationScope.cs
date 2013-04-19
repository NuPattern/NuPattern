namespace NuPattern.Runtime.Guidance.Extensions
{
    /// <summary>
    /// This scope is only active when the default project that unfolds a feature is being 
    /// instantiated via <see cref="IGuidanceManager"/> interface.
    /// </summary>
    internal class DefaultTemplateInstantiationScope : InstantiationScope<DefaultTemplateInstantiationScope>
    {
    }
}