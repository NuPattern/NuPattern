using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;

namespace NuPattern.Diagnostics
{
    /// <summary>
    /// Extensions over <see cref="ITraceSource"/>.
    /// </summary>
    [CLSCompliant(false)]
    public static class TraceSourceExtensions
    {
        /// <summary>
        /// Emits a trace record that can contain arbitrary data that will be rendered in full.
        /// </summary>
        /// <param name="tracer">The trace source used to emit the trace.</param>
        /// <param name="source">The object invoking the trace operation.</param>
        /// <param name="type">The type of event being traced.</param>
        /// <param name="record">The arbitrary data record that will be dumped to the trace listeners. Typically an anonymous object.</param>
        /// <param name="description">Optional description of what's being traced.</param>
        public static void TraceRecord(this ITraceSource tracer, object source, TraceEventType type, object record, string description = null)
        {
            Guard.NotNull(() => tracer, tracer);
            Guard.NotNull(() => source, source);
            Guard.NotNull(() => record, record);

            tracer.TraceData(type, 0, new DictionaryTraceRecord(type, source.GetType().FullName, description, record));
        }
    }
}
