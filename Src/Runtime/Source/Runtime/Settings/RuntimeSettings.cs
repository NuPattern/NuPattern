
namespace NuPattern.Runtime.Settings
{
    /// <summary>
    /// Represents the settings for the runtime.
    /// </summary>
    internal class RuntimeSettings : IRuntimeSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeSettings"/> class.
        /// </summary>
        public RuntimeSettings()
        {
            this.Tracing = new TracingSettings();
        }

        /// <summary>
        /// Gets the tracing settings.
        /// </summary>
        public ITracingSettings Tracing { get; private set; }
    }
}
