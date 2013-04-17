using System;
using System.Diagnostics;
using NuPattern.Diagnostics;
using NuPattern.Reflection;

namespace NuPattern.Runtime.Guidance.Diagnostics
{
    /// <summary>
    /// Trace record that contains feature information.
    /// </summary>
    [DebuggerStepThrough]
    internal class FeatureTraceRecord : DictionaryTraceRecord
    {
        public FeatureTraceRecord(TraceEventType severity, string traceIdentifier, string featureId,
            string instanceName = null, Exception exception = null, string description = null,
            object extendedData = null)
            : base(severity, traceIdentifier, description, extendedData)
        {
            this.Exception = exception;
            if (!String.IsNullOrEmpty(featureId))
                this.FeatureId = featureId;
            if (!String.IsNullOrEmpty(instanceName))
                this.InstanceName = instanceName;
        }

        /// <summary>
        /// Cloning ctor.
        /// </summary>
        protected FeatureTraceRecord()
        {
        }

        protected override TraceRecord DoClone()
        {
            return new FeatureTraceRecord();
        }

        public string FeatureId
        {
            get { return (string)base.Data[Reflect<FeatureTraceRecord>.GetProperty(x => x.FeatureId).Name]; }
            private set { base.Data[Reflect<FeatureTraceRecord>.GetProperty(x => x.FeatureId).Name] = value; }
        }

        public string InstanceName
        {
            get
            {
                object name = null;
                base.Data.TryGetValue("InstanceName", out name);
                return (string)name;
            }
            private set
            {
                if (value != null)
                    base.Data[Reflect<FeatureTraceRecord>.GetProperty(x => x.InstanceName).Name] = value;
                else
                    base.Data.Remove(Reflect<FeatureTraceRecord>.GetProperty(x => x.InstanceName).Name);
            }
        }
    }
}