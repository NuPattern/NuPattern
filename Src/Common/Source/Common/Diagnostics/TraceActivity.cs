using System;
using System.Diagnostics;

namespace NuPattern.Diagnostics
{
    /// <summary>
    /// Creates a new activity boundary and tracer scope.
    /// </summary>
    /// <remarks>
    /// Activity tracing must be enabled for the trace source received in the
    /// constructor.
    /// </remarks>
    [DebuggerStepThrough]
    public class TraceActivity : IDisposable
    {
        string activityName;
        bool disposed;
        ITraceSource source;
        Guid oldId;
        Guid newId;

        /// <summary>
        /// Creates a new instance of the <see cref="TraceActivity"/> class.
        /// </summary>
        /// <param name="activityName"></param>
        /// <param name="source"></param>
        public TraceActivity(string activityName, ITraceSource source)
        {
            Guard.NotNullOrEmpty(() => activityName, activityName);
            Guard.NotNull(() => source, source);

            this.activityName = activityName;
            this.source = source;
            Initialize();
        }

        private void Initialize()
        {
            newId = Guid.NewGuid();
            oldId = Trace.CorrelationManager.ActivityId;

            if (oldId != Guid.Empty)
                source.TraceTransfer(0, null, newId);

            Trace.CorrelationManager.ActivityId = newId;
            source.TraceEvent(TraceEventType.Start, 0, activityName);
        }

        /// <summary>
        /// Destroys this instance of the class.
        /// </summary>
        ~TraceActivity()
        {
            Dispose(false);
        }

        /// <summary>
        /// Disposes of this instance
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes of this instance
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !disposed)
            {
                source.TraceEvent(TraceEventType.Stop, 0, activityName);

                if (oldId != Guid.Empty)
                    source.TraceTransfer(0, null, oldId);

                Trace.CorrelationManager.ActivityId = oldId;
            }

            disposed = true;
        }
    }
}
