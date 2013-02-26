
namespace NuPattern.Extensibility
{
    /// <summary>
    /// Represents a node in an XML document.
    /// </summary>
    public interface IXmlProcessorNode
    {
        /// <summary>
        /// Gets the parent node of the node.
        /// </summary>
        IXmlProcessorNode Parent { get; }

        /// <summary>
        /// Removes the node from the document.
        /// </summary>
        void Remove();

        /// <summary>
        /// Gets or sets the value of the node.
        /// </summary>
        string Value { get; set; }
    }
}
