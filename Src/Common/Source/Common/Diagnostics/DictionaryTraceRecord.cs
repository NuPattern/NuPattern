using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;
using NuPattern.Reflection;

namespace NuPattern.Diagnostics
{
    /// <summary>
    /// Extended trace record that can contain data elements.
    /// </summary>
    /// <remarks>
    /// If element is <see cref="XmlReader"/>, 
    /// <see cref="XPathNavigator"/>, <see cref="XmlNode"/>, 
    /// <see cref="XDocument"/> or <see cref="XElement"/>, 
    /// the raw XML will be serialized, without emitting the 
    /// associated dictionary key. 
    /// <para>
    /// Otherwise, an attempt to serialize to XML will be 
    /// performed, defaulting to <see cref="Object.ToString"/> 
    /// if not possible.
    /// </para>
    /// If the value is a simple type (determined when <see cref="Type.GetTypeCode"/> 
    /// is not <see cref="TypeCode.Object"/>), the key and value are emitted. 
    /// Null values are not emitted.
    /// </remarks>
    [DebuggerStepThrough]
    public class DictionaryTraceRecord : TraceRecord
    {
        internal const string DictionaryXmlNamespace = "http://schemas.microsoft.com/2006/08/ServiceModel/DictionaryTraceRecord";

        private static Dictionary<Type, Delegate> XmlConverters;

        static DictionaryTraceRecord()
        {
            // Build an easy to access and fast performing list of 
            // delegates to convert primitive types to 
            // string in a generic fashion.
            XmlConverters = typeof(XmlConvert)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(method => method.Name.Equals(@"ToString", StringComparison.OrdinalIgnoreCase) && method.GetParameters().Length == 1)
                .ToDictionary(
                    method => method.GetParameters()[0].ParameterType,
                    method =>
                    {
                        var valueType = method.GetParameters()[0].ParameterType;
                        var parameter = Expression.Parameter(valueType, @"x");
                        return Expression.Lambda(
                            typeof(Func<,>).MakeGenericType(valueType, typeof(string)),
                            Expression.Call(method, parameter),
                            parameter
                        ).Compile();
                    });
        }

        /// <summary>
        /// Creates a new instance of the <see cref="DictionaryTraceRecord"/> class.
        /// </summary>
        public DictionaryTraceRecord(TraceEventType severity, string traceIdentifier,
            string description = null,
            object extendedData = null)
            : base(severity, traceIdentifier, description)
        {
            this.Data = new Dictionary<string, object>();

            if (extendedData != null)
            {
                if (extendedData.GetType().IsAnonymous())
                {
                    foreach (var prop in extendedData.GetType().GetProperties())
                    {
                        this.Data[prop.Name] = prop.GetValue(extendedData, null);
                    }
                }
                else
                {
                    this.ExtendedData = extendedData;
                }
            }
        }

        /// <summary>
        /// Cloning ctor.
        /// </summary>
        protected DictionaryTraceRecord()
        {
        }

        /// <summary>
        /// Clones the record.
        /// </summary>
        protected override TraceRecord DoClone()
        {
            return new DictionaryTraceRecord();
        }

        /// <summary>
        /// Copies the record.
        /// </summary>
        protected override void DoCopyTo(TraceRecord clone)
        {
            base.DoCopyTo(clone);
            ((DictionaryTraceRecord)clone).Data = new Dictionary<string, object>(this.Data);
        }

        /// <summary>
        /// Writes to the record.
        /// </summary>
        protected override void WriteTo(XmlWriter writer)
        {
            base.WriteTo(writer);

            writer.WriteStartElement("Data", DictionaryXmlNamespace);

            foreach (KeyValuePair<string, object> pair in this.Data.Where(item => item.Value != null))
            {
                if (pair.Value is XmlNode)
                {
                    ((XmlNode)pair.Value).WriteTo(writer);
                }
                else if (pair.Value is XmlReader)
                {
                    writer.WriteNode((XmlReader)pair.Value, false);
                }
                else if (pair.Value is XPathNavigator)
                {
                    ((XPathNavigator)pair.Value).WriteSubtree(writer);
                }
                else if (pair.Value is XElement)
                {
                    ((XElement)pair.Value).WriteTo(writer);
                }
                else if (pair.Value is XDocument)
                {
                    ((XDocument)pair.Value).Root.WriteTo(writer);
                }
                else
                {
                    if (Type.GetTypeCode(pair.Value.GetType()) == TypeCode.Object)
                    {
                        try
                        {
                            var rootNs = (from attr in pair.Value.GetType().GetCustomAttributes(typeof(XmlRootAttribute), true)
                                          select ((XmlRootAttribute)attr).Namespace)
                                         .FirstOrDefault();

                            rootNs = rootNs ?? "";

                            var itemSerializer = new XmlSerializer(pair.Value.GetType());
                            // Passing the XmlSerializerNamespaces avoids the typical generated xmlns prefixes
                            itemSerializer.Serialize(writer, pair.Value, new XmlSerializerNamespaces(
                                new[] { new XmlQualifiedName("", rootNs) }));
                        }
                        catch (Exception)
                        {
                            try
                            {
                                var dataSerializer = new DataContractSerializer(pair.Value.GetType());
                                dataSerializer.WriteObject(writer, pair.Value);
                            }
                            catch (Exception)
                            {
                                var dump = ObjectDumper.ToString(pair.Value, 5);
                                var ix = dump.LastIndexOf(Environment.NewLine);
                                if (ix > 0) dump = dump.Substring(0, ix);
                                writer.WriteElementString(pair.Key, DictionaryXmlNamespace, dump);
                            }
                        }
                    }
                    else
                    {
                        WriteKeyValue(writer, pair);
                    }
                }
            }

            writer.WriteEndElement();
        }

        private static void WriteKeyValue(XmlWriter writer, KeyValuePair<string, object> pair)
        {
            writer.WriteStartElement(pair.Key, DictionaryXmlNamespace);

            Delegate xmlConvert;
            if (XmlConverters.TryGetValue(pair.Value.GetType(), out xmlConvert))
                writer.WriteString((string)xmlConvert.DynamicInvoke(pair.Value));
            else
                writer.WriteString(pair.Value.ToString());

            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets the data or the record.
        /// </summary>
        public Dictionary<string, object> Data { get; private set; }

        /// <summary>
        /// Gets the extended data for the record.
        /// </summary>
        public object ExtendedData
        {
            get
            {
                object data = null;
                this.Data.TryGetValue(Reflect<DictionaryTraceRecord>.GetProperty(x => x.ExtendedData).Name, out data);
                return data;
            }
            private set
            {
                this.Data[Reflect<DictionaryTraceRecord>.GetProperty(x => x.ExtendedData).Name] = value;
            }
        }
    }
}
