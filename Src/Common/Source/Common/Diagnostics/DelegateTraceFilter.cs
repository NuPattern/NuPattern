using System;
using System.Diagnostics;

namespace NuPattern.Diagnostics
{
    /// <summary>
    /// Allows filtering of traces based on a predicate delegate.
    /// </summary>
    internal class DelegateTraceFilter : TraceFilter
    {
        Func<TraceEventCache, string, TraceEventType, int, bool> predicate;

        public DelegateTraceFilter(Func<TraceEventCache, string, TraceEventType, int, bool> predicate)
        {
            Guard.NotNull(() => predicate, predicate);

            this.predicate = predicate;
        }

        public override bool ShouldTrace(TraceEventCache cache, string source, TraceEventType eventType, int id, string formatOrMessage, object[] args, object data1, object[] data)
        {
            return predicate(cache, source, eventType, id);
        }
    }
}
