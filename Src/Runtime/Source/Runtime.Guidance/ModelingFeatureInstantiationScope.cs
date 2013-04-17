namespace Microsoft.VisualStudio.TeamArchitect.PowerTools.Features
{
    /// <summary>
    /// This scope is only active when the modeling project that represents a feature is being 
    /// instantiated via IFeatureManager API. The modeling feature instantiation code 
    /// </summary>
    internal class ModelingFeatureInstantiationScope : InstantiationScope<ModelingFeatureInstantiationScope>
    {
    }
}