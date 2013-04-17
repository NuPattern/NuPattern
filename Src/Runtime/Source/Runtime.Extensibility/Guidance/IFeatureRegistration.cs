using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.VisualStudio.ExtensionManager;
using NuPattern.VisualStudio.Solution.Templates;

namespace Microsoft.VisualStudio.TeamArchitect.PowerTools.Features
{
    /// <summary>
    /// Represents a feature registration.
    /// </summary>
    [CLSCompliant(false)]
    public interface IFeatureRegistration : IFeatureMetadata
    {
        /// <summary>
        /// Gets the path where the feature is installed
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
        /// Gets the templates exposed by the feature.
        /// </summary>
        IEnumerable<IVsTemplate> Templates { get; }
    }
}