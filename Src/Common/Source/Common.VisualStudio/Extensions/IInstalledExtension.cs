
using System;

namespace NuPattern.VisualStudio.Extensions
{
    /// <summary>
    /// Defines an installed VSIX Extension
    /// </summary>
    public interface IInstalledExtension : IExtension
    {
        /// <summary>
        /// Gets the date the extension was installed
        /// </summary>
        DateTimeOffset? InstalledOn { get; }

        /// <summary>
        /// Gets a value to to indicate whether the extension is installed for all users.
        /// </summary>
        bool InstalledPerMachine { get; }

        /// <summary>
        /// Gets the path where the extension is installed
        /// </summary>
        string InstallPath { get; }

        /// <summary>
        /// 
        /// </summary>
        bool IsPackComponent { get; }

        /// <summary>
        /// The size of the extension package
        /// </summary>
        int SizeInBytes { get; }

        /// <summary>
        /// Gets the state of the extension.
        /// </summary>
        EnabledState State { get; }
    }
}
