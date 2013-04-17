using System;

namespace NuPattern.ComponentModel
{
    /// <summary>
    /// A standard value that is expoerted by another type.
    /// </summary>
    public class ExportedStandardValue : StandardValue
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ExportedStandardValue"/> class.
        /// </summary>
        public ExportedStandardValue(string displayText, object value, Type exportingType, string description = "", string group = "")
            : base(displayText, value, description, group)
        {
            this.ExportingType = exportingType;
        }

        /// <summary>
        /// Gets the type exporting the value.
        /// </summary>
        public Type ExportingType { get; private set; }
    }
}