using System.ComponentModel;

namespace NuPattern.Runtime
{
    /// <summary>
    /// Defines the options for <see cref="SchemaUpgradeProcessorOptionsAttribute"/>
    /// </summary>
    public interface ISchemaUpgradeProccesorOptions
    {
        /// <summary>
        /// Gets the global order of the processor. 
        /// </summary>
        [DefaultValue(SchemaUpgradeProcessorOptionsAttribute.DefaultOrder)]
        int Order { get; }

        /// <summary>
        /// Gets or sets the version of the schema that this migration targets up to.
        /// </summary>
        /// <remarks>
        /// The default (and least possible) value is 1.3.0.0
        /// </remarks>
        [DefaultValue(SchemaUpgradeProcessorOptionsAttribute.DefaultTargetVersion)]
        string TargetVersion { get; }
    }
}
