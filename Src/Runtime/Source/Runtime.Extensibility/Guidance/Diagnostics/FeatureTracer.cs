using System;
using System.Diagnostics;
using System.Globalization;
using System.Xml.XPath;
using NuPattern.Diagnostics;

namespace NuPattern.Runtime.Guidance.Diagnostics
{
    /// <summary>
    /// Feature-specific tracer that emits feature identifier and name 
    /// for all traces.
    /// </summary>
    /// <remarks>
    /// Converts all traces into calls to <see cref="ITraceSource.TraceData"/>, 
    /// passing a <see cref="TraceRecord"/> converted to an <see cref="XPathNavigator"/> 
    /// by calling <see cref="TraceRecord.ToXml"/>. The standard 
    /// <see cref="XmlWriterTraceListener"/> expects an <see cref="XPathNavigator"/> 
    /// in order to write a full XML node.
    /// <para>
    /// Other listeners can intercept calls to 
    /// <see cref="TraceListener.TraceData(TraceEventCache,string,TraceEventType,int,object)"/> 
    /// and cast the data to TraceRecordXPathNavigator, which exposes the 
    /// source <see cref="TraceRecord"/> via its TraceRecordXPathNavigator.Record 
    /// property.
    /// </para>
    /// </remarks>
    [DebuggerStepThrough]
    public static class FeatureTracer
    {
        /// <summary>
        /// Returns a trace source for the given feature
        /// </summary>
        public static ITraceSource GetSourceFor(object component, string featureId, string instanceName = null)
        {
            Guard.NotNull(() => component, component);

            return GetSourceFor(Tracer.GetSourceFor(component.GetType()), featureId, instanceName);
        }

        /// <summary>
        /// Returns a trace source for the given feature
        /// </summary>
        public static ITraceSource GetSourceFor<T>(string featureId, string instanceName = null)
        {
            return GetSourceFor(Tracer.GetSourceFor<T>(), featureId, instanceName);
        }

        internal static ITraceSource GetSourceFor(ITraceSource innerSource, string featureId, string instanceName = null)
        {
            return new FeatureTraceSource(featureId, instanceName, innerSource);
        }

        [DebuggerStepThrough]
        internal class FeatureTraceSource : ITraceSource
        {
            string featureId;
            string instanceName;

            public FeatureTraceSource(string featureId, string instanceName, ITraceSource innerSource)
            {
                this.featureId = featureId;
                this.instanceName = instanceName;
                this.InnerSource = innerSource;
            }

            internal ITraceSource InnerSource { get; private set; }

            private void Trace(TraceEventType eventType, int id, string messageOrFormat, params object[] args)
            {
                InnerSource.TraceData(eventType, id,
                    new FeatureTraceRecord(eventType, id.ToString(), featureId,
                        instanceName: this.instanceName,
                        description: messageOrFormat == null ? eventType.ToString() : String.Format(CultureInfo.CurrentCulture, messageOrFormat, args))
                    );
            }

            public void TraceData(TraceEventType eventType, int id, object data)
            {
                var record = data as TraceRecord;
                if (record == null)
                {
                    InnerSource.TraceData(eventType, id,
                        new FeatureTraceRecord(eventType, id.ToString(), featureId,
                            instanceName: this.instanceName,
                            extendedData: data,
                            description: "TraceData")
                        );
                }
                else
                {
                    InnerSource.TraceData(eventType, id, record);
                }
            }

            public string Name
            {
                get { return InnerSource.Name; }
            }

            public void Flush()
            {
                InnerSource.Flush();
            }

            public void TraceEvent(TraceEventType eventType, int id)
            {
                Trace(eventType, id, null);
            }

            public void TraceEvent(TraceEventType eventType, int id, string format, params object[] args)
            {
                Trace(eventType, id, format, args);
            }

            public void TraceEvent(TraceEventType eventType, int id, string message)
            {
                Trace(eventType, id, message);
            }

            public void TraceInformation(string message)
            {
                Trace(TraceEventType.Information, 0, message);
            }

            public void TraceInformation(string format, params object[] args)
            {
                Trace(TraceEventType.Information, 0, format, args);
            }

            public void TraceVerbose(string message)
            {
                Trace(TraceEventType.Verbose, 0, message);
            }

            public void TraceVerbose(string format, params object[] args)
            {
                Trace(TraceEventType.Verbose, 0, format, args);
            }

            public void TraceTransfer(int id, string message, Guid relatedActivityId)
            {
                InnerSource.TraceTransfer(id, message, relatedActivityId);
            }

            public void TraceError(string format, params object[] args)
            {
                Trace(TraceEventType.Error, 0, format, args);
            }

            public void TraceError(string message)
            {
                Trace(TraceEventType.Error, 0, message);
            }

            public void TraceError(Exception exception, string format, params object[] args)
            {
                TraceError(exception, String.Format(format, args));
            }

            public void TraceError(Exception exception, string message)
            {
                string logmessage = message + Environment.NewLine + exception.Message;

                InnerSource.TraceData(TraceEventType.Error, 0,
                    new FeatureTraceRecord(TraceEventType.Error, "0", featureId,
                        instanceName: this.instanceName,
                        description: logmessage,
                        exception: exception));
            }

            public void TraceWarning(string format, params object[] args)
            {
                Trace(TraceEventType.Warning, 0, format, args);
            }

            public void TraceWarning(string message)
            {
                Trace(TraceEventType.Warning, 0, message);
            }

        }
    }
}