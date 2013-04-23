using System;
using System.Diagnostics;

namespace NuPattern.Diagnostics
{
    /// <summary>
    /// Allows filtering of traces based on a predicate delegate.
    /// </summary>
    public class DelegateTraceFilter : TraceFilter
    {
        Func<TraceEventCache, string, TraceEventType, int, bool> predicate;

        /// <summary>
        /// CReates a new instance of the <see cref="DelegateTraceFilter"/> class.
        /// </summary>
        /// <param name="predicate"></param>
        public DelegateTraceFilter(Func<TraceEventCache, string, TraceEventType, int, bool> predicate)
        {
            Guard.NotNull(() => predicate, predicate);

            this.predicate = predicate;
        }

        /// <summary>
        /// Determines whether to filter or to trace.
        /// </summary>
        public override bool ShouldTrace(TraceEventCache cache, string source, TraceEventType eventType, int id, string formatOrMessage, object[] args, object data1, object[] data)
        {
            return predicate(cache, source, eventType, id);
        }
    }
}
