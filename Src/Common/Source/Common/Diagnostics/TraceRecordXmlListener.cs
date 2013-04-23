using System.Diagnostics;
using System.IO;

namespace NuPattern.Diagnostics
{
    /// <summary>
    /// Turns traced data <see cref="TraceRecord"/> into 
    /// its xml representation before calling the base 
    /// <see cref="XmlWriterTraceListener"/>.
    /// </summary>
    [DebuggerStepThrough]
    internal class TraceRecordXmlListener : XmlWriterTraceListener
    {
        public TraceRecordXmlListener(Stream stream, string name = null) : base(stream, name) { }
        public TraceRecordXmlListener(TextWriter writer, string name = null) : base(writer, name) { }
        public TraceRecordXmlListener(string filename, string name = null) : base(filename, name) { }

        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
        {
            var record = data as TraceRecord;
            if (record != null)
                base.TraceData(eventCache, source, eventType, id, record.ToXml());
            else
                base.TraceData(eventCache, source, eventType, id, data);
        }
    }
}
