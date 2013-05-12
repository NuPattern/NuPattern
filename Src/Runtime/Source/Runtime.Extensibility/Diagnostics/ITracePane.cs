
using System;
using System.Collections.Generic;

namespace NuPattern.Runtime.Diagnostics
{
    /// <summary>
    /// Defines a pane for tracing.
    /// </summary>
    public interface ITracePane
    {
        /// <summary>
        /// Gets the identifier of the trace pane.
        /// </summary>
        Guid TracePaneId { get; }

        /// <summary>
        /// Sets the names of the trace sources.
        /// </summary>
        /// <param name="sourceNames">The names of the trace sources.</param>
        void SetTraceSourceNames(IEnumerable<string> sourceNames);
    }
}
