
namespace NuPattern.Runtime.Serialization
{
    /// <summary>
    /// Loop handling options
    /// </summary>
    public enum ReferenceLoopHandling
    {
        /// <summary>
        /// Throw a <see cref="JsonSerializationException"/> when a loop is encountered. 
        /// </summary>
        Error = Newtonsoft.Json.ReferenceLoopHandling.Error,
        /// <summary>
        /// Ignore loop references and do not serialize. 
        /// </summary>
        Ignore = Newtonsoft.Json.ReferenceLoopHandling.Ignore,
        /// <summary>
        /// Serialize loop references. 
        /// </summary>
        Serialize = Newtonsoft.Json.ReferenceLoopHandling.Serialize
    }
}
