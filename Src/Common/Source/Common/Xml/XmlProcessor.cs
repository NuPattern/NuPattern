using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;
using NuPattern.Properties;

namespace NuPattern.Xml
{
    /// <summary>
    /// Provides an implmentation of an XML processor.
    /// </summary>
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export(typeof(IXmlProcessor))]
    internal class XmlProcessor : IXmlProcessor
    {
        private XmlDocument document;
        private string xmlFileName;

        /// <summary>
        /// Creates a new instance of the <see cref="XmlProcessor"/> class.
        /// </summary>
        public XmlProcessor()
        {
            this.InitializeDocument();
        }

        /// <summary>
        /// Loads the XML document
        /// </summary>
        /// <param name="fileName">The full path to the XML file to load</param>
        public void LoadDocument(string fileName)
        {
            Guard.NotNullOrEmpty(() => fileName, fileName);

            try
            {
                this.document.Load(fileName);
            }
            catch (Exception ex)
            {
                throw new XmlException(
                    string.Format(CultureInfo.CurrentCulture, Resources.XmlProcessor_ErrorLoadFailed, fileName), ex);
            }

            // Save loaded file
            this.xmlFileName = fileName;
        }

        /// <summary>
        /// Finds the first node that match the given XPath query.
        /// </summary>
        /// <param name="xPathExpression">The XPath expression to use to locate the first node</param>
        /// <param name="namespaces">A list of namespaces and prefixes with which to query</param>
        /// <returns></returns>
        public IXmlProcessorNode FindFirst(string xPathExpression, IDictionary<string, string> namespaces)
        {
            Guard.NotNullOrEmpty(() => xPathExpression, xPathExpression);
            Guard.NotNull(() => namespaces, namespaces);

            if (!IsDocumentLoaded())
            {
                throw new InvalidOperationException(Resources.XmlProcessor_ErrorLoadNotCalledFirst);
            }

            try
            {
                var nsManager = new XmlNamespaceManager(this.document.NameTable);
                namespaces.ForEach(ns => nsManager.AddNamespace(ns.Key, ns.Value));
                var node = this.document.SelectSingleNode(xPathExpression, nsManager);
                if (node != null)
                {
                    return new XmlProcessorNode(node);
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new XPathException(
                    string.Format(CultureInfo.CurrentCulture, Resources.XmlProcessor_ErrorXPathSearchFailed, xPathExpression), ex);
            }
        }

        /// <summary>
        /// Finds all nodes that match the given XPath query.
        /// </summary>
        /// <param name="xPathExpression">The XPath expression to use to locate the nodes</param>
        /// <param name="namespaces">A list of namespaces and prefixes with which to query</param>
        /// <returns></returns>
        public IEnumerable<IXmlProcessorNode> Find(string xPathExpression, IDictionary<string, string> namespaces)
        {
            Guard.NotNullOrEmpty(() => xPathExpression, xPathExpression);
            Guard.NotNull(() => namespaces, namespaces);

            if (!IsDocumentLoaded())
            {
                throw new InvalidOperationException(Resources.XmlProcessor_ErrorLoadNotCalledFirst);
            }

            var nodes = new List<IXmlProcessorNode>();

            try
            {
                var nsManager = new XmlNamespaceManager(this.document.NameTable);
                namespaces.ForEach(ns => nsManager.AddNamespace(ns.Key, ns.Value));
                var nodelist = this.document.SelectNodes(xPathExpression, nsManager);
                if (nodelist != null)
                {
                    foreach (XmlNode node in nodelist)
                    {
                        nodes.Add(new XmlProcessorNode(node));
                    }
                }

                return nodes;
            }
            catch (Exception ex)
            {
                throw new XPathException(
                    string.Format(CultureInfo.CurrentCulture, Resources.XmlProcessor_ErrorXPathSearchFailed, xPathExpression), ex);
            }
        }

        /// <summary>
        /// Saves the XML document.
        /// </summary>
        /// <param name="closeAfterSave">Whether to close the document after save.</param>
        public void SaveDocument(bool closeAfterSave = true)
        {
            if (!IsDocumentLoaded())
            {
                throw new InvalidOperationException(Resources.XmlProcessor_ErrorLoadNotCalledFirst);
            }

            try
            {
                this.document.PreserveWhitespace = true;
                this.document.Save(this.xmlFileName);
            }
            catch (Exception ex)
            {
                throw new XPathException(
                    string.Format(CultureInfo.CurrentCulture, Resources.XmlProcessor_ErrorSaveFailed, this.xmlFileName), ex);
            }

            if (closeAfterSave)
            {
                this.CloseDocument();
            }
        }

        /// <summary>
        /// Closes the current document.
        /// </summary>
        public void CloseDocument()
        {
            if (!IsDocumentLoaded())
            {
                throw new InvalidOperationException(Resources.XmlProcessor_ErrorLoadNotCalledFirst);
            }

            // Reinitialize the document
            this.xmlFileName = null;
            InitializeDocument();
        }

        private bool IsDocumentLoaded()
        {
            return !String.IsNullOrEmpty(this.xmlFileName);
        }

        private void InitializeDocument()
        {
            this.document = new XmlDocument();
            this.document.PreserveWhitespace = true;
        }
    }
}
