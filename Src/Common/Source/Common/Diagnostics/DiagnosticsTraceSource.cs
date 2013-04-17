using System;
using System.Diagnostics;
using System.Globalization;

namespace NuPattern.Diagnostics
{
    /// <summary>
    /// Implements <see cref="ITraceSource"/> over a diagnostics <see cref="TraceSource"/>.
    /// </summary>
    [DebuggerStepThrough]
    internal class DiagnosticsTraceSource : ITraceSource
    {
        public DiagnosticsTraceSource(TraceSource innerSource)
        {
            this.InnerSource = innerSource;
        }

        public TraceSource InnerSource { get; private set; }

        public string Name
        {
            get { return this.InnerSource.Name; }
        }

        public void Flush()
        {
            this.InnerSource.Flush();
        }

        public void TraceData(TraceEventType eventType, int id, object data)
        {
            this.InnerSource.TraceData(eventType, id, data);
        }

        public void TraceEvent(TraceEventType eventType, int id)
        {
            this.InnerSource.TraceEvent(eventType, id);
        }

        public void TraceEvent(TraceEventType eventType, int id, string format, params object[] args)
        {
            this.InnerSource.TraceEvent(eventType, id);
        }

        public void TraceEvent(TraceEventType eventType, int id, string message)
        {
            this.InnerSource.TraceEvent(eventType, id, message);
        }

        public void TraceInformation(string message)
        {
            this.InnerSource.TraceInformation(message);
        }

        public void TraceInformation(string format, params object[] args)
        {
            this.InnerSource.TraceInformation(format, args);
        }

        public void TraceVerbose(string message)
        {
            this.InnerSource.TraceEvent(TraceEventType.Verbose, 0, message);
        }

        public void TraceVerbose(string format, params object[] args)
        {
            this.InnerSource.TraceEvent(TraceEventType.Verbose, 0, format, args);
        }

        public void TraceTransfer(int id, string message, Guid relatedActivityId)
        {
            this.InnerSource.TraceTransfer(id, message, relatedActivityId);
        }

        public void TraceError(Exception exception, string message)
        {
            string logmessage = message + Environment.NewLine + exception.ToString();

            this.InnerSource.TraceEvent(TraceEventType.Error, 0, logmessage);
        }

        public void TraceError(Exception exception, string format, params object[] args)
        {
            //string logmessage = format + Environment.NewLine + exception.ToString();
            //this.InnerSource.TraceEvent(TraceEventType.Error, 0, logmessage, args);

            string logmessage = string.Format(CultureInfo.CurrentCulture, format, args) + Environment.NewLine + exception.ToString();
            this.InnerSource.TraceEvent(TraceEventType.Error, 0, logmessage);
        }

        public void TraceError(string message)
        {
            this.InnerSource.TraceEvent(TraceEventType.Error, 0, message);
        }

        public void TraceError(string format, params object[] args)
        {
            this.InnerSource.TraceEvent(TraceEventType.Error, 0, format, args);
        }

        public void TraceWarning(string format, params object[] args)
        {
            this.InnerSource.TraceEvent(TraceEventType.Warning, 0, format, args);
        }

        public void TraceWarning(string message)
        {
            this.InnerSource.TraceEvent(TraceEventType.Warning, 0, message);
        }
    }
}
