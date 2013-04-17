using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NuPattern.Diagnostics
{
    [DebuggerStepThrough]
    internal class CompositeTraceSource : ITraceSource
    {
        private List<ITraceSource> sources;

        public CompositeTraceSource(params ITraceSource[] sources)
            : this(sources.ToList())
        {
        }

        public CompositeTraceSource(List<ITraceSource> sources)
        {
            this.sources = sources;
        }

        public IEnumerable<ITraceSource> InnerSources
        {
            get { return this.sources; }
        }

        public string Name
        {
            get { return string.Join(";", sources.Select(s => s.Name)); }
        }

        public void Flush()
        {
            this.sources.ForEach(source => source.Flush());
        }

        public void TraceData(TraceEventType eventType, int id, object data)
        {
            this.sources.ForEach(source => source.TraceData(eventType, id, data));
        }

        public void TraceEvent(TraceEventType eventType, int id)
        {
            this.sources.ForEach(source => source.TraceEvent(eventType, id));
        }

        public void TraceEvent(TraceEventType eventType, int id, string format, params object[] args)
        {
            this.sources.ForEach(source => source.TraceEvent(eventType, id, format, args));
        }

        public void TraceEvent(TraceEventType eventType, int id, string message)
        {
            this.sources.ForEach(source => source.TraceEvent(eventType, id, message));
        }

        public void TraceInformation(string message)
        {
            this.sources.ForEach(source => source.TraceInformation(message));
        }

        public void TraceInformation(string format, params object[] args)
        {
            this.sources.ForEach(source => source.TraceInformation(format, args));
        }

        public void TraceVerbose(string message)
        {
            this.sources.ForEach(source => source.TraceVerbose(message));
        }

        public void TraceVerbose(string format, params object[] args)
        {
            this.sources.ForEach(source => source.TraceVerbose(format, args));
        }

        public void TraceTransfer(int id, string message, Guid relatedActivityId)
        {
            this.sources.ForEach(source => source.TraceTransfer(id, message, relatedActivityId));
        }

        public void TraceError(Exception exception, string message)
        {
            sources.ForEach(source => source.TraceError(exception, message));
        }

        public void TraceError(Exception exception, string format, params object[] args)
        {
            this.sources.ForEach(source => source.TraceError(exception, format, args));
        }

        public void TraceError(string message)
        {
            this.sources.ForEach(source => source.TraceError(message));
        }

        public void TraceError(string format, params object[] args)
        {
            this.sources.ForEach(source => source.TraceError(format, args));
        }

        public void TraceWarning(string format, params object[] args)
        {
            this.sources.ForEach(source => source.TraceWarning(format, args));
        }

        public void TraceWarning(string message)
        {
            this.sources.ForEach(source => source.TraceWarning(message));
        }
    }
}