namespace NuPattern.Runtime.Guidance.Extensions
{
    /// <summary>
    /// This scope is only active when the modeling project that represents a feature is being 
    /// instantiated via IFeatureManager API.
    /// </summary>
    internal class FeatureInstantiationScope : InstantiationScope<FeatureInstantiationScope>
    {
    }
}