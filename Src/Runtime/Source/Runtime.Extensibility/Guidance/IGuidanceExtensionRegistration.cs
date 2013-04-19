using System.Collections.Generic;
using System.Drawing;
using NuPattern.VisualStudio.Extensions;
using NuPattern.VisualStudio.Solution.Templates;

namespace NuPattern.Runtime.Guidance
{
    /// <summary>
    /// Represents a guidance extension registration.
    /// </summary>
    public interface IGuidanceExtensionRegistration : IGuidanceExtensionMetadata
    {
        /// <summary>
        /// Gets the path where the guidance extension is installed
        /// </summary>
        string InstallPath { get; }

        /// <summary>
        /// Gets the icon.
        /// </summary>
        Icon Icon { get; }

        /// <summary>
        /// Gets the preview image
        /// </summary>
        Bitmap PreviewImage { get; }

        /// <summary>
        /// Gets the ExtensionManager-provided installation 
        /// information.
        /// </summary>
        IInstalledExtension InstalledExtension { get; }

        /// <summary>
        /// Gets the ExtensionManager-provided manifest 
        /// information.
        /// </summary>
        IExtension ExtensionManifest { get; }

        /// <summary>
        /// Gets the templates exposed by the guidance extension.
        /// </summary>
        IEnumerable<IVsTemplate> Templates { get; }
    }
}