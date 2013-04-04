
namespace NuPattern.Runtime.Serialization
{
    /// <summary>
    /// Settings for serialization
    /// </summary>
    public class JsonSerializerSettings
    {
        /// <summary>
        /// Loop handling options
        /// </summary>
        public ReferenceLoopHandling ReferenceLoopHandling { get; set; }
        /// <summary>
        /// Type name handling options
        /// </summary>
        public TypeNameHandling TypeNameHandling { get; set; }
    }
}
