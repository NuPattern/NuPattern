using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace NuPattern.Extensibility
{
    /// <summary>
    /// Defines a context for automated schema upgrades.
    /// </summary>
    public interface ISchemaUpgradeContext
    {
        /// <summary>
        /// Gets the upgrade processors.
        /// </summary>
        IEnumerable<Lazy<IPatternModelSchemaUpgradeProcessor, ISchemaUpgradeProccesorOptions>> UpgradeProcessors { get; }

        /// <summary>
        /// Gets the schema file path
        /// </summary>
        string SchemaFilePath { get; }

        /// <summary>
        /// Loads the schema as an XML document.
        /// </summary>
        /// <returns></returns>
        XDocument OpenSchema();

        /// <summary>
        /// Creates a backup of the schema document.
        /// </summary>
        /// <remarks>The schema document must not be changed before this method is called.</remarks>
        void BackupSchema();

        /// <summary>
        /// Saves the schema document.
        /// </summary>
        void SaveSchema();

        /// <summary>
        /// Gets or sets the version of the schema
        /// </summary>
        Version SchemaVersion { get; set; }

        /// <summary>
        /// Gets the runtime version.
        /// </summary>
        Version RuntimeVersion { get; }
    }
}
