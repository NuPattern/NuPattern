using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace NuPattern.Diagnostics
{
    /// <summary>
    /// A record for tracing
    /// </summary>
    [DebuggerStepThrough]
    public class TraceRecord : ICloneable
    {
        /// <summary>
        /// XML Namespace
        /// </summary>
        public const string RecordXmlNamespace = "http://schemas.microsoft.com/2004/10/E2ETraceEvent/TraceRecord";

        /// <summary>
        /// Creates a new instance of the <see cref="TraceRecord"/> class.
        /// </summary>
        public TraceRecord(TraceEventType severity, string traceIdentifier, string description = null)
        {
            Guard.NotNullOrEmpty(() => traceIdentifier, traceIdentifier);

            this.Severity = severity;
            this.TraceIdentifier = traceIdentifier;
            this.Description = description;
            this.AppDomain = System.AppDomain.CurrentDomain.FriendlyName;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="TraceRecord"/> class.
        /// </summary>
        public TraceRecord(TraceEventType severity, string traceIdentifier, Exception exception)
            : this(severity, traceIdentifier, description: exception.Message)
        {
            Guard.NotNull(() => exception, exception);

            this.Exception = exception;
        }

        /// <summary>
        /// Gets or sets the Severity of the record.
        /// </summary>
        public virtual TraceEventType Severity { get; protected set; }

        /// <summary>
        /// Gets or sets the identifier of the record.
        /// </summary>
        public virtual string TraceIdentifier { get; protected set; }

        /// <summary>
        /// Gets or sets the description of the record.
        /// </summary>
        public virtual string Description { get; protected set; }

        /// <summary>
        /// Gets or sets the <see cref="AppDomain"/> of the record.
        /// </summary>
        public virtual string AppDomain { get; protected set; }

        /// <summary>
        /// Gets or sets the optional <see cref="Exception"/> of the record.
        /// </summary>
        public virtual Exception Exception { get; protected set; }

        #region ICloneable

        /// <summary>
        /// Parameterless constructor for cloning.
        /// </summary>
        protected TraceRecord()
        {
        }

        /// <summary>
        /// Clones the record.
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            var clone = DoClone();
            DoCopyTo(clone);
            return clone;
        }

        /// <summary>
        /// Clones the record.
        /// </summary>
        /// <returns></returns>
        protected virtual TraceRecord DoClone()
        {
            return new TraceRecord();
        }

        /// <summary>
        /// Copies the record to another.
        /// </summary>
        /// <param name="clone"></param>
        protected virtual void DoCopyTo(TraceRecord clone)
        {
            clone.AppDomain = this.AppDomain;
            clone.Description = this.Description;
            clone.Exception = this.Exception;
            clone.Severity = this.Severity;
            clone.TraceIdentifier = this.TraceIdentifier;
        }

        #endregion

        /// <summary>
        /// Turns the trace record into its XML representation, 
        /// needed by the <see cref="XmlWriterTraceListener"/> to 
        /// emit the standard event trace information.
        /// </summary>
        /// <remarks>
        /// Default behavior of this class is to write the standard trace 
        /// information, alongside any <see cref="DictionaryTraceRecord.ExtendedData"/> and 
        /// <see cref="Exception"/> if any.
        /// <para>
        /// Specific record classes can override this default behavior.
        /// </para>
        /// </remarks>
        public virtual XPathNavigator ToXml()
        {
            var document = new XDocument();
            using (var writer = document.CreateWriter())
            {
                writer.WriteStartElement("TraceRecord", RecordXmlNamespace);
                writer.WriteAttributeString("Severity", this.Severity.ToString());
                writer.WriteElementString("TraceIdentifier", this.TraceIdentifier);

                if (!String.IsNullOrEmpty(this.Description))
                    writer.WriteElementString("Description", this.Description);

                writer.WriteElementString("AppDomain", this.AppDomain);

                if (this.Exception != null)
                {
                    writer.WriteStartElement("Exception", RecordXmlNamespace);
                    AddExceptionToTraceString(writer, this.Exception);
                    writer.WriteEndElement();
                }

                WriteTo(writer);
                writer.WriteEndElement();
            }

            return document.CreateNavigator();
        }

        #region Copied from System.ServiceModel.Diagnostics.DiagnosticTrace

        private void AddExceptionToTraceString(XmlWriter xml, Exception exception)
        {
            xml.WriteElementString("ExceptionType", exception.GetType().AssemblyQualifiedName);
            xml.WriteElementString("Message", exception.Message);
            xml.WriteElementString("StackTrace", StackTraceString(exception));
            xml.WriteElementString("ExceptionString", exception.ToString());
            Win32Exception winException = exception as Win32Exception;
            if (winException != null)
            {
                xml.WriteElementString("NativeErrorCode",
                    winException.NativeErrorCode.ToString("X", CultureInfo.InvariantCulture));
            }
            if ((exception.Data != null) && (exception.Data.Count > 0))
            {
                xml.WriteStartElement("DataItems");
                foreach (object key in exception.Data.Keys)
                {
                    xml.WriteStartElement("Data");
                    xml.WriteElementString("Key", key.ToString());
                    xml.WriteElementString("Value", exception.Data[key].ToString());
                    xml.WriteEndElement();
                }
                xml.WriteEndElement();
            }
            if (exception.InnerException != null)
            {
                xml.WriteStartElement("InnerException");
                AddExceptionToTraceString(xml, exception.InnerException);
                xml.WriteEndElement();
            }
        }

        private string StackTraceString(Exception exception)
        {
            string stackTrace = exception.StackTrace;
            if (!string.IsNullOrEmpty(stackTrace))
            {
                return stackTrace;
            }

            StackFrame[] frames = new StackTrace(false).GetFrames();
            int skipFrames = 0;
            // We could cleanup the powertools stack frames here...
            //foreach (StackFrame frame in frames)
            //{
            //    string name = frame.GetMethod().Name;
            //    // Determine cleanup of ASMX stack frames
            //    if (false 
            //        /*name != null && (
            //        name == "StackTraceString" || name == "AddExceptionToTraceString" || 
            //        name == "WriteXml" || name == "ToXml")*/
            //        )
            //    {
            //        //skipFrames++;
            //    }
            //    else
            //    {
            //        break;
            //    }
            //}

            StackTrace trace = new StackTrace(skipFrames, false);
            return trace.ToString();
        }

        #endregion

        /// <summary>
        /// Writes to the record.
        /// </summary>
        /// <param name="writer"></param>
        protected virtual void WriteTo(XmlWriter writer)
        {
        }
    }
}