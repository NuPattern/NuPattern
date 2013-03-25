
namespace NuPattern.Runtime
{
    /// <summary>
    /// Defines settings for the runtime.
    /// </summary>
    public interface IRuntimeSettings
    {
        /// <summary>
        /// Gets the tracing settings
        /// </summary>
        ITracingSettings Tracing { get; }
    }
}
