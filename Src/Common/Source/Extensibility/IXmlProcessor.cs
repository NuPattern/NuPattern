

using System.Collections.Generic;

namespace NuPattern.Extensibility
{
    /// <summary>
    /// An XML processor
    /// </summary>
    public interface IXmlProcessor
    {
        /// <summary>
        /// Loads the XML document
        /// </summary>
        /// <param name="fileName">The full path to the XML document to load.</param>
        void LoadDocument(string fileName);

        /// <summary>
        /// Finds the first XML node that matches the given XPath expression. 
        /// </summary>
        /// <param name="xPathExpression">The Xpath expression of the node to search for</param>
        /// <param name="namespaces">A list of namespaces and prefixes with which to query</param>
        /// <returns></returns>
        IXmlProcessorNode FindFirst(string xPathExpression, IDictionary<string, string> namespaces);

        /// <summary>
        /// Find the XML nodes that match the given the XPath expression. 
        /// </summary>
        /// <param name="xPathExpression">The Xpath expression of the node to search for</param>
        /// <param name="namespaces">A list of namespaces and prefixes with which to query</param>
        /// <returns></returns>
        IEnumerable<IXmlProcessorNode> Find(string xPathExpression, IDictionary<string, string> namespaces);

        /// <summary>
        /// Saves the XML document after modification.
        /// </summary>
        /// <param name="closeAfterSave">Whether to close the document after the save.</param>
        void SaveDocument(bool closeAfterSave = true);

        /// <summary>
        /// Closes the current XML document.
        /// </summary>
        void CloseDocument();
    }
}
