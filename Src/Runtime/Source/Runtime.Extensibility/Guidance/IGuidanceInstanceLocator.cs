
namespace NuPattern.Runtime.Guidance
{
    /// <summary>
    /// Interface implemented by components that can locate a guidance extension instance 
    /// for use by launch points.
    /// </summary>
    internal interface IGuidanceInstanceLocator
    {
        /// <summary>
        /// Attemps to find the guidance extension instance associated with the component type.
        /// </summary>
        /// <returns>The <see cref="IGuidanceExtension"/> if the guidance extension is found; <see langword="null"/> otherwise.</returns>
        IGuidanceExtension LocateInstance();
    }
}
