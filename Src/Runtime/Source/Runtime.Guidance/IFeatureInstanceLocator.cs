
namespace Microsoft.VisualStudio.TeamArchitect.PowerTools.Features
{
    /// <summary>
    /// Interface implemented by components that can locate a feature instance 
    /// for use by launch points.
    /// </summary>
    internal interface IFeatureInstanceLocator
    {
        /// <summary>
        /// Attemps to find the feature instance associated with the component type.
        /// </summary>
        /// <returns>The <see cref="IFeatureExtension"/> if the feature is found; <see langword="null"/> otherwise.</returns>
        IFeatureExtension LocateInstance();
    }
}
