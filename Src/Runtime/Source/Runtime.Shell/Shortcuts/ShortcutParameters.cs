using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace NuPattern.Runtime.Shell.Shortcuts
{
    /// <summary>
    /// Parameters for a shortcut
    /// </summary>
    /// <remarks>
    /// Must be public for XML serialization.
    /// </remarks>
    [Serializable]
    public class ShortcutParameters : Dictionary<string, string>, IXmlSerializable
    {
        private const string ItemElementName = "parameter";
        private const string KeyAttributeName = "key";
        private const string ValueAttributeName = "value";

        /// <summary>
        /// Creates a new instance of the <see cref="ShortcutParameters"/> class.
        /// </summary>
        public ShortcutParameters()
            : base()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ShortcutParameters"/> class.
        /// </summary>
        protected ShortcutParameters(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Gets the schema for this class
        /// </summary>
        public XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Reads the XML from the instance.
        /// </summary>
        public void ReadXml(XmlReader reader)
        {
            var wasEmpty = reader.IsEmptyElement;
            reader.Read();
            if (wasEmpty)
            {
                return;
            }

            while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
            {
                reader.MoveToAttribute(KeyAttributeName);
                var key = reader.Value;
                reader.MoveToAttribute(ValueAttributeName);
                var value = reader.Value;

                if (!string.IsNullOrEmpty(key))
                {
                    this.Add(key, value);
                }

                reader.ReadStartElement();
            }

            reader.ReadEndElement();
        }

        /// <summary>
        /// Writes the XML for the instance
        /// </summary>
        public void WriteXml(XmlWriter writer)
        {
            foreach (var key in this.Keys)
            {
                writer.WriteStartElement(ItemElementName);
                writer.WriteAttributeString(KeyAttributeName, key);
                writer.WriteAttributeString(ValueAttributeName, this[key]);
                writer.WriteEndElement();
            }
        }
    }
}
