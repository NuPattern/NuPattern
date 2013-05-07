using System;
using System.Collections.Generic;

namespace NuPattern.Runtime.Diagnostics
{
    /// <summary>
    /// Defines a manager for trace output windows
    /// </summary>
    public interface ITraceOutputWindowManager
    {
        /// <summary>
        /// Creates a new trace pane.
        /// </summary>
        /// <param name="traceId">The identifier of the trace pane</param>
        /// <param name="title">The title to display for the  trace pane</param>
        /// <param name="traceSourceNames">The names of the sources to display in this trace window</param>
        ITracePane CreateTracePane(Guid traceId, string title, IEnumerable<string> traceSourceNames);

        /// <summary>
        /// Returns the trace panes.
        /// </summary>
        IEnumerable<ITracePane> TracePanes { get; }
    }
}
