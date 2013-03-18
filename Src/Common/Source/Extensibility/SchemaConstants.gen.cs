using System;

namespace NuPattern.Extensibility
{
    /// <summary>
    /// Defines constant values for the runtime schema.
    /// </summary>
    public static class SchemaConstants
    {
        /// <summary>
        /// Current DSL version
        /// </summary>
        public const string SchemaVersion = @"1.3.0.0";
        
        /// <summary>
        /// Current DSL Version
        /// </summary>
        public static readonly Version DslVersion = new Version("1.3.0.0");

        /// <summary>
        /// Current DSL version
        /// </summary>
        public const string DefaultNamespace = @"http://schemas.microsoft.com/visualstudio/patterning/runtime/patternmodel";
    }
}