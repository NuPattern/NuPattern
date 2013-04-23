using NuPattern.Runtime.Bindings;

namespace NuPattern.Library.Automation
{
    partial interface ITemplateSettings
    {
        /// <summary>
        /// Gets the settings for the of the target file.
        /// </summary>
        IPropertyBindingSettings TargetFileName { get; }

        /// <summary>
        /// Gets the settings for the of the target path.
        /// </summary>
        IPropertyBindingSettings TargetPath { get; }
    }
}
