using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

namespace NuPattern.Diagnostics
{
    [DebuggerStepThrough]
    internal class TraceRecordDefaultTraceListener : DefaultTraceListener
    {
        // Most of this code is duplicated from TraceRecordTextListener, as we need 
        // to inherit from DefaultTraceListener.
        const string RecordFormat = "[{0}] {1}";
        const string DictionaryFormat = "{0}={1}, ";

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id)
        {
            SetIndent(eventType, () => base.TraceEvent(eventCache, source, eventType, id));
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
        {
            SetIndent(eventType, () => base.TraceEvent(eventCache, source, eventType, id, format, args));
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
        {
            SetIndent(eventType, () => base.TraceEvent(eventCache, source, eventType, id, message));
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
                return;

            this.IndentLevel++;
            foreach (var entry in dictionary.Data.Where(x => x.Value != null).OrderBy(x => x.Key))
            {
                if (Type.GetTypeCode(entry.Value.GetType()) == TypeCode.Object)
                {
                    base.WriteLine(String.Format(CultureInfo.CurrentCulture, DictionaryFormat, entry.Key, ""));
                    var writer = new StringWriter();
                    ObjectDumper.Write(entry.Value, 5, new IndentedTextWriter(writer, new string(' ', this.IndentLevel * this.IndentSize)));
                    base.WriteLine(writer.ToString());
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