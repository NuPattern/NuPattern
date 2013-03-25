
using NuPattern.VisualStudio.Extensions;

namespace NuPattern.Runtime
{
    /// <summary>
    /// Defines the classification of a toolkit.
    /// </summary>
    public interface IToolkitClassification
    {
        /// <summary>
        /// Gets the category of the toolkit.
        /// </summary>
        string Category { get; }

        /// <summary>
        /// Gets the visibility of the toolkit for customization.
        /// </summary>
        ExtensionVisibility CustomizeVisibility { get; }

        /// <summary>
        /// Gets the visibility of the toolkit for creation.
        /// </summary>
        ExtensionVisibility CreateVisibility { get; }
    }
}
