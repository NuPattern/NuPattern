using System;
using System.Diagnostics;
using NuPattern.Diagnostics;
using NuPattern.Reflection;

namespace NuPattern.Runtime.Guidance.Diagnostics
{
    /// <summary>
    /// Trace record that contains guidance extension information.
    /// </summary>
    [DebuggerStepThrough]
    internal class GuidanceExtensionTraceRecord : DictionaryTraceRecord
    {
        public GuidanceExtensionTraceRecord(TraceEventType severity, string traceIdentifier, string guidanceId,
            string instanceName = null, Exception exception = null, string description = null,
            object extendedData = null)
            : base(severity, traceIdentifier, description, extendedData)
        {
            this.Exception = exception;
            if (!String.IsNullOrEmpty(guidanceId))
                this.ExtensionId = guidanceId;
            if (!String.IsNullOrEmpty(instanceName))
                this.InstanceName = instanceName;
        }

        /// <summary>
        /// Cloning ctor.
        /// </summary>
        protected GuidanceExtensionTraceRecord()
        {
        }

        protected override TraceRecord DoClone()
        {
            return new GuidanceExtensionTraceRecord();
        }

        public string ExtensionId
        {
            get { return (string)base.Data[Reflect<GuidanceExtensionTraceRecord>.GetProperty(x => x.ExtensionId).Name]; }
            private set { base.Data[Reflect<GuidanceExtensionTraceRecord>.GetProperty(x => x.ExtensionId).Name] = value; }
        }

        public string InstanceName
        {
            get
            {
                object name = null;
                base.Data.TryGetValue(Reflector<GuidanceExtensionTraceRecord>.GetPropertyName(x => x.InstanceName), out name);
                return (string)name;
            }
            private set
            {
                if (value != null)
                    base.Data[Reflect<GuidanceExtensionTraceRecord>.GetProperty(x => x.InstanceName).Name] = value;
                else
                    base.Data.Remove(Reflect<GuidanceExtensionTraceRecord>.GetProperty(x => x.InstanceName).Name);
            }
        }
    }
}