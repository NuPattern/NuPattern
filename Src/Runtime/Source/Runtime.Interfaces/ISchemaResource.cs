using System.IO;

namespace NuPattern.Runtime
{
    /// <summary>
    /// Provides access to a toolkit schema.
    /// </summary>
    public interface ISchemaResource
    {
        /// <summary>
        /// Creates a stream out of the resource so that it can be read.
        /// </summary>
        Stream CreateStream();
    }
}