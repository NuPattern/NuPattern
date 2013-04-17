using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

namespace NuPattern.Diagnostics
{
    [DebuggerStepThrough]
    internal class TraceRecordTextListener : TextWriterTraceListener
    {
        private const string RecordFormat = "[{0}] {1}";
        private const string DictionaryFormat = "{0}={1}, ";

        public TraceRecordTextListener(TextWriter writer, string name = null)
            : base(writer, name)
        {
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id)
        {
            this.SetIndent(eventType, () => base.TraceEvent(eventCache, source, eventType, id));
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
        {
            this.SetIndent(eventType, () => base.TraceEvent(eventCache, source, eventType, id, format, args));
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
        {
            this.SetIndent(eventType, () => base.TraceEvent(eventCache, source, eventType, id, message));
        }

        private void SetIndent(TraceEventType eventType, Action action)
        {
            if (eventType == TraceEventType.Stop)
            {
                this.IndentLevel--;
                this.NeedIndent = this.IndentLevel > 0;
            }
            action();
            if (eventType == TraceEventType.Start)
            {
                this.IndentLevel++;
                this.NeedIndent = true;
            }
        }

        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
        {
            if (this.Filter == null || this.Filter.ShouldTrace(eventCache, source, eventType, id, null, null, data, null))
            {
                var record = data as TraceRecord;
                if (record == null)
                {
                    base.TraceData(eventCache, source, eventType, id, data);
                    return;
                }

                base.WriteLine(String.Format(CultureInfo.CurrentCulture, RecordFormat, eventType, record.Description));
                if (record.Exception != null)
                    base.WriteLine(String.Format(CultureInfo.CurrentCulture, DictionaryFormat, "Exception", record.Exception));

                var dictionary = data as DictionaryTraceRecord;
                if (dictionary == null)
                {
                    return;
                }

                this.IndentLevel++;
                foreach (var entry in dictionary.Data.Where(x => x.Value != null).OrderBy(x => x.Key))
                {
                    if (Type.GetTypeCode(entry.Value.GetType()) == TypeCode.Object)
                    {
                        base.WriteLine(String.Format(CultureInfo.CurrentCulture, DictionaryFormat, entry.Key, ""));
                        ObjectDumper.Write(entry.Value, 5, new IndentedTextWriter(base.Writer, new string(' ', this.IndentLevel * this.IndentSize)));
                    }
                    else
                    {
                        base.WriteLine(String.Format(CultureInfo.CurrentCulture, DictionaryFormat, entry.Key, entry.Value));
                    }
                }

                this.IndentLevel--;
            }
        }
    }
}
