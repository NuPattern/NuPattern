using System.Collections.Generic;
using System.Diagnostics;

namespace NuPattern.Runtime.Settings
{
    /// <summary>
    /// Defines settings for the runtime.
    /// </summary>
    internal interface ITracingSettings
    {
        /// <summary>
        /// Gets or sets the logging level for the catch-all root trace source ("NuPattern").
        /// </summary>
        SourceLevels RootSourceLevel { get; set; }

        /// <summary>
        /// Gets the trace sources configurations.
        /// </summary>
        ICollection<ITraceSourceSetting> TraceSources { get; }
    }
}
