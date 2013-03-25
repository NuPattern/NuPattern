using System;
using System.ComponentModel.Composition;

namespace NuPattern.Runtime
{
    /// <summary>
    /// Specifies the options for exports of <see cref="IPatternModelSchemaUpgradeProcessor"/>.
    /// </summary>
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class SchemaUpgradeProcessorOptionsAttribute : ExportAttribute
    {
        /// <summary>
        /// The default order of the processor.
        /// </summary>
        internal const int DefaultOrder = 1000;

        /// <summary>
        /// The defautl target version.
        /// </summary>
        /// <remarks>Target Version cannot be less than this default.</remarks>
        internal const string DefaultTargetVersion = SchemaConstants.SchemaVersion;

        /// <summary>
        /// Creates a new instance of the <see cref="SchemaUpgradeProcessorOptionsAttribute"/> class.
        /// </summary>
        public SchemaUpgradeProcessorOptionsAttribute()
            : base(typeof(IPatternModelSchemaUpgradeProcessor))
        {
            this.Order = DefaultOrder;
            this.TargetVersion = DefaultTargetVersion;
        }

        /// <summary>
        /// Gets or sets the global order that the processor executes.
        /// </summary>
        /// <remarks>
        /// The default value is 1000.
        /// </remarks>
        public int Order { get; set; }

        /// <summary>
        /// Gets or sets the version of the schema that this migration targets up to.
        /// </summary>
        /// <remarks>
        /// The default (and least possible) value is 1.3.0.0
        /// </remarks>
        public string TargetVersion { get; set; }
    }
}