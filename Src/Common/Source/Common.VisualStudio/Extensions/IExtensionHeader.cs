using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;

namespace NuPattern.VisualStudio.Extensions
{
    /// <summary>
    /// Defines the header configuration of a VSIX
    /// </summary>
    public interface IExtensionHeader
    {
        /// <summary>
        /// Gets the addtional unsupported elements in the header
        /// </summary>
        IList<XmlElement> AdditionalElements { get; }

        /// <summary>
        /// Gets a value to indicate whether the VSIX will be installed for all users. 
        /// </summary>
        bool AllUsers { get; }

        /// <summary>
        /// Gets the author of the VSIX
        /// </summary>
        string Author { get; }

        /// <summary>
        /// Gets the description of the VSIX
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets a 'Getting Started' URI for the VSIX
        /// </summary>
        Uri GettingStartedGuide { get; }

        /// <summary>
        /// Gets a value to indicate whether the the VSIX has global scope
        /// </summary>
        bool GlobalScope { get; }

        /// <summary>
        /// Gets the icon for the VSIX
        /// </summary>
        string Icon { get; }

        /// <summary>
        /// Gets the identifier of the VSIX
        /// </summary>
        string Identifier { get; }

        /// <summary>
        /// Gets a value to indicate whether the VSIX was installed by an MSI.
        /// </summary>
        bool InstalledByMsi { get; }

        /// <summary>
        /// Gets the license of the VSIX
        /// </summary>
        string License { get; }

        /// <summary>
        /// Gets a value to indicate whether to not disply the license
        /// </summary>
        bool LicenseClickThrough { get; }

        /// <summary>
        /// Gets the format of the license.
        /// </summary>
        string LicenseFormat { get; }

        /// <summary>
        /// Gets the locale of the VSIX.
        /// </summary>
        CultureInfo Locale { get; }

        /// <summary>
        /// Gets the localized version of the addtional unsupported elements in the header.
        /// </summary>
        IList<XmlElement> LocalizedAdditionalElements { get; }

        /// <summary>
        /// Gets the localized description.
        /// </summary>
        string LocalizedDescription { get; }

        /// <summary>
        /// Gets the localized name of the VSIX
        /// </summary>
        string LocalizedName { get; }

        /// <summary>
        /// Gets a 'More Information' URI
        /// </summary>
        Uri MoreInfoUrl { get; }

        /// <summary>
        /// Gets the name of the VSIX
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Get the preview image of the VSIX.
        /// </summary>
        string PreviewImage { get; }

        /// <summary>
        /// Gets the release notes for the VSIX.
        /// </summary>
        Uri ReleaseNotes { get; }

        /// <summary>
        /// Gets the raw byte data of the release notes.
        /// </summary>
        byte[] ReleaseNotesContent { get; }

        /// <summary>
        /// Gets the format of the release notes
        /// </summary>
        string ReleaseNotesFormat { get; }

        /// <summary>
        /// Gets the supported framework versions
        /// </summary>
        IVersionRange SupportedFrameworkVersionRange { get; }

        /// <summary>
        /// Gets a value to indicate whether the VSIX is a system component.
        /// </summary>
        bool SystemComponent { get; }

        /// <summary>
        /// Gets the tags for the VSIX
        /// </summary>
        IEnumerable<string> Tags { get; }

        /// <summary>
        /// Gets the version of the VSIX.
        /// </summary>
        Version Version { get; }
    }
}
