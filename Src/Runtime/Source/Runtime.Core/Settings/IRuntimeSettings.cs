
namespace NuPattern.Runtime.Settings
{
    /// <summary>
    /// Defines settings for the runtime.
    /// </summary>
    internal interface IRuntimeSettings
    {
        /// <summary>
        /// Gets the tracing settings
        /// </summary>
        ITracingSettings Tracing { get; }
    }
}
