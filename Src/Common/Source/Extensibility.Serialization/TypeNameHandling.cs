
namespace NuPattern.Extensibility.Serialization
{
    /// <summary>
    /// Type handling options
    /// </summary>
    public enum TypeNameHandling
    {
        /// <summary>
        /// Do not include the .NET type name when serializing types. 
        /// </summary>
        None = Newtonsoft.Json.TypeNameHandling.None,
        /// <summary>
        /// Include the .NET type name when serializing into a JSON object structure. 
        /// </summary>
        Objects = Newtonsoft.Json.TypeNameHandling.Objects,
        /// <summary>
        /// Include the .NET type name when serializing into a JSON array structure. 
        /// </summary>
        Arrays = Newtonsoft.Json.TypeNameHandling.Arrays,
        /// <summary>
        /// Always include the .NET type name when serializing. 
        /// </summary>
        All = Newtonsoft.Json.TypeNameHandling.All,
        /// <summary>
        /// Include the .NET type name when the type of the object being serialized is not the same as its declared type. 
        /// </summary>
        Auto = Newtonsoft.Json.TypeNameHandling.Auto
    }
}
