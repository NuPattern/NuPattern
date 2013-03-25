
using System.Diagnostics;

namespace NuPattern.Runtime.Settings
{
    /// <summary>
    /// Defines settings for a conifguration of a <see cref="TraceSource"/>.
    /// </summary>
    public interface ITraceSourceSetting
    {
        /// <summary>
        /// Gets the name of the tracing source.
        /// </summary>
        string SourceName { get; set; }

        /// <summary>
        /// Gets the logging level for this trace source.
        /// </summary>
        SourceLevels LoggingLevel { get; set; }
    }
}
