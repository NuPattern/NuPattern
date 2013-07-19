
namespace NuPattern.Runtime.Guidance.ShortcutProviders
{
    /// <summary>
    /// Defines a provider of guidance
    /// </summary>
    internal interface IGuidanceProvider
    {
        /// <summary>
        /// Whether the guidance extension is registered
        /// </summary>
        /// <param name="guidanceExtensionId">The identifier of the guidance extension</param>
        bool IsRegistered(string guidanceExtensionId);

        /// <summary>
        /// Whether an instance of the guidance already exists
        /// </summary>
        /// <param name="guidanceExtensionId">The identifier of the guidance extension</param>
        /// <param name="instanceName">The name of the guidance instance</param>
        /// <returns></returns>
        bool InstanceExists(string guidanceExtensionId, string instanceName);

        /// <summary>
        /// Creates a new instance of a guidance extension
        /// </summary>
        /// <param name="guidanceExtensionId">The identifier of the guidance extension</param>
        /// <param name="instanceName">The name of the instance</param>
        void CreateInstance(string guidanceExtensionId, string instanceName);

        /// <summary>
        /// Activates an existing instance of guidance
        /// </summary>
        /// <param name="instanceName">The name of the instance</param>
        void ActivateInstance(string instanceName);

        /// <summary>
        /// Gets a unique instance name from all instances using the seeded name
        /// </summary>
        /// <param name="instanceName">The name of the instance to use as a seed</param>
        string GetUniqueInstanceName(string instanceName);

        /// <summary>
        /// Returns the default instance name for the guidance extension
        /// </summary>
        /// <param name="guidanceExtensionId">The identifier of the guidance extension</param>
        string GetDefaultInstanceName(string guidanceExtensionId);
    }
}
