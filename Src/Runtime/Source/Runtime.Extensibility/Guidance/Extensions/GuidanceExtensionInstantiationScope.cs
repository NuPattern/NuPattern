namespace NuPattern.Runtime.Guidance.Extensions
{
    /// <summary>
    /// This scope is only active when the project that represents a guidance extension is being 
    /// instantiated via <see cref="IGuidanceManager"/> interface.
    /// </summary>
    internal class GuidanceExtensionInstantiationScope : InstantiationScope<GuidanceExtensionInstantiationScope>
    {
    }
}