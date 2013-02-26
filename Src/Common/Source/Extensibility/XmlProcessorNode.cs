using System;
using System.Xml;

namespace NuPattern.Extensibility
{
    /// <summary>
    /// An XML node used by the <see cref="XmlProcessor"/>.
    /// </summary>
    internal class XmlProcessorNode : IXmlProcessorNode
    {
        private XmlNode node;

        internal XmlProcessorNode(XmlNode node)
        {
            Guard.NotNull(() => node, node);

            this.node = node;
        }

        /// <summary>
        /// Gets hte parent of the node.
        /// </summary>
        public IXmlProcessorNode Parent
        {
            get
            {
                if (this.node.ParentNode != null)
                {
                    return new XmlProcessorNode(node.ParentNode);
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Removes the node from the XML document.
        /// </summary>
        public void Remove()
        {
            var parent = this.node.ParentNode;
            if (parent != null)
            {
                parent.RemoveChild(this.node);
            }
        }

        /// <summary>
        /// Gets or sets the value of the node.
        /// </summary>
        public string Value
        {
            get
            {
                switch (this.node.NodeType)
                {
                    case XmlNodeType.Element:
                        return this.node.InnerText;

                    default:
                        return this.node.Value;
                }
            }
            set
            {
                switch (this.node.NodeType)
                {
                    case XmlNodeType.Element:
                        if (!this.node.InnerText.Equals(value, StringComparison.Ordinal))
                        {
                            this.node.InnerText = value;
                        }
                        break;

                    default:
                        if (!this.node.Value.Equals(value, StringComparison.Ordinal))
                        {
                            this.node.Value = value;
                        }
                        break;
                }
            }
        }
    }
}
