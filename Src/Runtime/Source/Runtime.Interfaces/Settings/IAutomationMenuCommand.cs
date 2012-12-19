using Microsoft.VisualStudio.Modeling.ExtensionEnablement;

namespace NuPattern.Runtime
{
    /// <summary>
    /// Wrapper for <see cref="T:Microsoft.VisualStudio.Modeling.ExtensionEnablement.IMenuCommand"/> that provides the ability to specify an Icon
    /// </summary>
    public interface IAutomationMenuCommand : IMenuCommand
    {
        /// <summary>
        /// Path to the icon in pack:// format
        /// </summary>
        string IconPath { get; }

        /// <summary>
        /// Order for sorting.
        /// </summary>
        long SortOrder { get; }
    }
}
