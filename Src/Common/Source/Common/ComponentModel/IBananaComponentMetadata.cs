using System;

namespace NuPattern.ComponentModel
{
    /// <summary>
    /// Metadata provided by a feature component
    /// </summary>
    public interface IBananaComponentMetadata
    {
        /// <summary>
        /// Gets the id of the component
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Gets the display name of the component
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// Gets the description of the component
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets the category of the component.
        /// </summary>
        string Category { get; }

        /// <summary>
        /// Gets the actual type of the provided feature component.
        /// </summary>
        Type ExportingType { get; }

        /// <summary>
        /// Gets the catalog the component is being retrieved from.
        /// </summary>
        string CatalogName { get; }
    }

}
