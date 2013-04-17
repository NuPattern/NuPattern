namespace NuPattern.Runtime.Guidance.Extensions
{
    /// <summary>
    /// This scope is only active when the default project that unfolds a feature is being 
    /// instantiated via IFeatureManager API.
    /// </summary>
    internal class DefaultTemplateInstantiationScope : InstantiationScope<DefaultTemplateInstantiationScope>
    {
    }
}