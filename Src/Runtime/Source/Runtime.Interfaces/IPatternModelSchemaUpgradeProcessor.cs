using System.Xml.Linq;

namespace NuPattern.Runtime
{
    /// <summary>
    /// Defines a schema upgrade processor
    /// </summary>
    public interface IPatternModelSchemaUpgradeProcessor
    {
        /// <summary>
        /// Processes the document
        /// </summary>
        /// <param name="document">The whole document to process</param>
        void ProcessSchema(XDocument document);

        /// <summary>
        /// Gets a value to indicate whether the processing changed the schema.
        /// </summary>
        bool IsModified { get; }
    }
}
