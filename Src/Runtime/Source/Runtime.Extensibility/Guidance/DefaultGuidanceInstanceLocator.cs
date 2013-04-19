using System;
using System.Linq;

namespace NuPattern.Runtime.Guidance
{
    /// <summary>
    /// A strategy for locating the guidance extension associated with a component, 
    /// that looks for the first instantiated guidance extension of the same 
    /// type that owns the component.
    /// </summary>
    internal class DefaultGuidanceInstanceLocator : IGuidanceInstanceLocator
    {
        private IGuidanceManager guidanceManager;
        private Type componentType;

        public DefaultGuidanceInstanceLocator(IGuidanceManager guidanceManager, Type componentType)
        {
            this.guidanceManager = guidanceManager;
            this.componentType = componentType;
        }

        /// <summary>
        /// Attemps to find the guidance extension instance associated with the component type.
        /// </summary>
        /// <returns>The <see cref="IGuidanceExtension"/> if the guidance extension is found; <see langword="null"/> otherwise.</returns>
        public IGuidanceExtension LocateInstance()
        {
            var registration = guidanceManager.FindGuidanceExtension(componentType);

            // Guidance extension is not even installed or there isn't at least one instantiation of it.
            if (registration == null)
                return null;

            // If the active guidance extension is of the same type as the owning guidance extension, then 
            // use that one.
            if (guidanceManager.ActiveGuidanceExtension != null &&
                guidanceManager.ActiveGuidanceExtension.ExtensionId == registration.ExtensionId)
                return guidanceManager.ActiveGuidanceExtension;

            // Fallback to grabbing the first one we find of the same type, if any.
            return guidanceManager.InstantiatedGuidanceExtensions.FirstOrDefault(f => f.ExtensionId == registration.ExtensionId);
        }
    }
}
