
namespace NuPattern.Extensibility.Serialization.Json
{
    /// <summary>
    /// Specifies reference loop handling options for the <see cref="JsonSerializer"/>.
    /// </summary>
    public enum ReferenceLoopHandling
    {
        /// <summary>
        /// Throw a <see cref="JsonSerializationException"/> when a loop is encountered.
        /// </summary>
        Error = 0,
        /// <summary>
        /// Ignore loop references and do not serialize.
        /// </summary>
        Ignore = 1,
        /// <summary>
        /// Serialize loop references.
        /// </summary>
        Serialize = 2
    }
}
