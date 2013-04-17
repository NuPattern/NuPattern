using System;
using System.Linq;

namespace NuPattern.Runtime.Guidance
{
    /// <summary>
    /// A strategy for locating the feature associated with a component, 
    /// that looks for the first instantiated feature of the same 
    /// type that owns the component.
    /// </summary>
    internal class DefaultFeatureInstanceLocator : IFeatureInstanceLocator
    {
        private IFeatureManager featureManager;
        private Type componentType;

        public DefaultFeatureInstanceLocator(IFeatureManager featureManager, Type componentType)
        {
            this.featureManager = featureManager;
            this.componentType = componentType;
        }

        /// <summary>
        /// Attemps to find the feature instance associated with the component type.
        /// </summary>
        /// <returns>The <see cref="IFeatureExtension"/> if the feature is found; <see langword="null"/> otherwise.</returns>
        public IFeatureExtension LocateInstance()
        {
            var registration = featureManager.FindFeature(componentType);

            // Feature is not even installed or there isn't at least one instantiation of it.
            if (registration == null)
                return null;

            // If the active feature is of the same type as the owning feature, then 
            // use that one.
            if (featureManager.ActiveFeature != null &&
                featureManager.ActiveFeature.FeatureId == registration.FeatureId)
                return featureManager.ActiveFeature;

            // Fallback to grabbing the first one we find of the same type, if any.
            return featureManager.InstantiatedFeatures.FirstOrDefault(f => f.FeatureId == registration.FeatureId);
        }
    }
}
