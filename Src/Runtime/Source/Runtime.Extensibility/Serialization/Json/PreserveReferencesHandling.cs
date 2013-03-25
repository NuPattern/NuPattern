using System;

namespace NuPattern.Extensibility.Serialization.Json
{
    /// <summary>
    /// Specifies reference handling options for the <see cref="JsonSerializer"/>.
    /// </summary>
    [Flags]
    public enum PreserveReferencesHandling
    {
        /// <summary>
        /// Do not preserve references when serializing types.
        /// </summary>
        None = 0,
        /// <summary>
        /// Preserve references when serializing into a JSON object structure.
        /// </summary>
        Objects = 1,
        /// <summary>
        /// Preserve references when serializing into a JSON array structure.
        /// </summary>
        Arrays = 2,
        /// <summary>
        /// Preserve references when serializing.
        /// </summary>
        All = Objects | Arrays
    }
}
