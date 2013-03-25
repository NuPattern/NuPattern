using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using NuPattern.Extensibility.Serialization;

namespace NuPattern.Runtime
{
    /// <summary>
    /// A <see cref="TextWriterTraceListener"/> that properly indents and 
    /// transforms data records to JSON strings.
    /// </summary>
    /// <devdoc>
    /// We still need this replacement for FB listener as we use Json for 
    /// serialization, rather than ObjectDumper :(
    /// </devdoc>
    internal class TraceRecordTextListener : TextWriterTraceListener
    {
        private const string RecordFormat = "[{0}] {1}";
        private const string DictionaryFormat = "{0}={1}";

        /// <summary>
        /// Initializes a new instance of the <see cref="TraceRecordTextListener"/> class.
        /// </summary>
        public TraceRecordTextListener(TextWriter writer)
            : base(writer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TraceRecordTextListener"/> class.
        /// </summary>
        public TraceRecordTextListener(TextWriter writer, string name)
            : base(writer, name)
        {
        }

        /// <summary>
        /// Writes trace and event information to the listener specific output.
        /// </summary>
        /// <param name="eventCache">A <see cref="T:System.Diagnostics.TraceEventCache"/> object that contains the current process ID, thread ID, and stack trace information.</param>
        /// <param name="source">A name used to identify the output, typically the name of the application that generated the trace event.</param>
        /// <param name="eventType">One of the <see cref="T:System.Diagnostics.TraceEventType"/> values specifying the type of event that has caused the trace.</param>
        /// <param name="id">A numeric identifier for the event.</param>
        /// <PermissionSet>
        /// 	<IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// 	<IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode"/>
        /// </PermissionSet>
        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id)
        {
            this.SetIndent(eventType, () => base.TraceEvent(eventCache, source, eventType, id));
        }

        /// <summary>
        /// Writes trace information, a formatted array of objects and event information to the listener specific output.
        /// </summary>
        /// <param name="eventCache">A <see cref="T:System.Diagnostics.TraceEventCache"/> object that contains the current process ID, thread ID, and stack trace information.</param>
        /// <param name="source">A name used to identify the output, typically the name of the application that generated the trace event.</param>
        /// <param name="eventType">One of the <see cref="T:System.Diagnostics.TraceEventType"/> values specifying the type of event that has caused the trace.</param>
        /// <param name="id">A numeric identifier for the event.</param>
        /// <param name="format">A format string that contains zero or more format items, which correspond to objects in the <paramref name="args"/> array.</param>
        /// <param name="args">An object array containing zero or more objects to format.</param>
        /// <PermissionSet>
        /// 	<IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// 	<IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode"/>
        /// </PermissionSet>
        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
        {
            this.SetIndent(eventType, () => base.TraceEvent(eventCache, source, eventType, id, format, args));
        }

        /// <summary>
        /// Writes trace information, a message, and event information to the listener specific output.
        /// </summary>
        /// <param name="eventCache">A <see cref="T:System.Diagnostics.TraceEventCache"/> object that contains the current process ID, thread ID, and stack trace information.</param>
        /// <param name="source">A name used to identify the output, typically the name of the application that generated the trace event.</param>
        /// <param name="eventType">One of the <see cref="T:System.Diagnostics.TraceEventType"/> values specifying the type of event that has caused the trace.</param>
        /// <param name="id">A numeric identifier for the event.</param>
        /// <param name="message">A message to write.</param>
        /// <PermissionSet>
        /// 	<IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// 	<IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode"/>
        /// </PermissionSet>
        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
        {
            this.SetIndent(eventType, () => base.TraceEvent(eventCache, source, eventType, id, message));
        }

        /// <summary>
        /// Writes trace information, a data object and event information to the listener specific output.
        /// </summary>
        /// <param name="eventCache">A <see cref="T:System.Diagnostics.TraceEventCache"/> object that contains the current process ID, thread ID, and stack trace information.</param>
        /// <param name="source">A name used to identify the output, typically the name of the application that generated the trace event.</param>
        /// <param name="eventType">One of the <see cref="T:System.Diagnostics.TraceEventType"/> values specifying the type of event that has caused the trace.</param>
        /// <param name="id">A numeric identifier for the event.</param>
        /// <param name="data">The trace data to emit.</param>
        /// <PermissionSet>
        /// 	<IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// 	<IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode"/>
        /// </PermissionSet>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Diagnostics.TraceListener.WriteLine(System.String)", Justification = "CLR API")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Diagnostics.TextWriterTraceListener.WriteLine(System.String)", Justification = "CLR API")]
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

                this.WriteLine(String.Format(CultureInfo.CurrentCulture, RecordFormat, eventType, record.Description));
                if (record.Exception != null)
                {
                    this.WriteLine(String.Format(CultureInfo.CurrentCulture, DictionaryFormat, "Exception", record.Exception));
                }

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
                        this.WriteLine(String.Format(CultureInfo.CurrentCulture, DictionaryFormat, entry.Key, string.Empty));

                        var serialized = JsonConvert.SerializeObject(
                            entry.Value,
                            Formatting.None,
                            new JsonSerializerSettings
                            {
                                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                                TypeNameHandling = TypeNameHandling.None,
                            });

                        foreach (var line in serialized.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            this.WriteLine(line);
                        }
                    }
                    else
                    {
                        this.WriteLine(String.Format(CultureInfo.CurrentCulture, DictionaryFormat, entry.Key, entry.Value));
                    }
                }

                this.IndentLevel--;
            }
        }

        private void SetIndent(TraceEventType eventType, Action action)
        {
            if (eventType == TraceEventType.Stop)
            {
                base.IndentLevel--;
                base.NeedIndent = base.IndentLevel > 0;
            }

            action();

            if (eventType == TraceEventType.Start)
            {
                base.IndentLevel++;
                base.NeedIndent = true;
            }
        }
    }
}
