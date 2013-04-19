using System;
using System.Collections.Generic;
using System.Xml;
using ExtMan = Microsoft.VisualStudio.ExtensionManager;
using System.Globalization;

namespace NuPattern.VisualStudio.Extensions
{
    /// <summary>
    /// A wrapper for a VS VSIX registration header
    /// </summary>
    internal class VsExtensionHeader : IExtensionHeader
    {
        private ExtMan.IExtensionHeader vsHeader;

        /// <summary>
        /// Creates a new instance of the <see cref="VsExtensionHeader"/> class.
        /// </summary>
        public VsExtensionHeader(ExtMan.IExtensionHeader vsHeader)
        {
            this.vsHeader = vsHeader;
        }

        public bool AllUsers
        {
            get { return this.vsHeader.AllUsers; }
        }

        public string Author
        {
            get { return this.vsHeader.Author; }
        }

        public string Description
        {
            get { return this.vsHeader.Description; }
        }

        public Uri GettingStartedGuide
        {
            get { return this.vsHeader.GettingStartedGuide; }
        }

        public string Icon
        {
            get { return this.vsHeader.Icon; }
        }

        public string Identifier
        {
            get { return this.vsHeader.Identifier; }
        }

        public bool InstalledByMsi
        {
            get { return this.vsHeader.InstalledByMsi; }
        }

        public string License
        {
            get { return this.vsHeader.License; }
        }

        public bool LicenseClickThrough
        {
            get { return this.vsHeader.LicenseClickThrough; }
        }

        public string LicenseFormat
        {
            get { return this.vsHeader.LicenseFormat; }
        }

        public CultureInfo Locale
        {
            get { return this.vsHeader.Locale; }
        }

        public string LocalizedDescription
        {
            get { return this.vsHeader.LocalizedDescription; }
        }

        public string LocalizedName
        {
            get { return this.vsHeader.LocalizedName; }
        }

        public Uri MoreInfoUrl
        {
            get { return this.vsHeader.MoreInfoUrl; }
        }

        public string Name
        {
            get { return this.vsHeader.Name; }
        }

        public string PreviewImage
        {
            get { return this.vsHeader.PreviewImage; }
        }

        public bool SystemComponent
        {
            get { return this.vsHeader.SystemComponent; }
        }

        public Version Version
        {
            get { return this.vsHeader.Version; }
        }

#if VSVER10
        /// <summary>
        /// Gets the maximum supported framework version
        /// </summary>
        public Version SupportedFrameworkMaxVersion 
        { 
            get 
            {
                return this.vsHeader.SupportedFrameworkMaxVersion;
            } 
        }

        /// <summary>
        /// Gets the minimum supported framework version
        /// </summary>
        public Version SupportedFrameworkMinVersion
        {
            get
            {
                return this.vsHeader.SupportedFrameworkMinVersion;
            }
        }

        /// <summary>
        /// Gets the supported VS versions
        /// </summary>
        public IEnumerable<Version> SupportedVSVersions
        {
            get
            {
                return this.vsHeader.SupportedVSVersions;
            }
        }
#endif
#if VSVER11
        public IList<XmlElement> AdditionalElements
        {
            get { return this.vsHeader.AdditionalElements; }
        }

        public bool GlobalScope
        {
            get { return this.vsHeader.GlobalScope; }
        }
        
        public IList<XmlElement> LocalizedAdditionalElements
        {
            get { return this.vsHeader.LocalizedAdditionalElements; }
        }
        
        public Uri ReleaseNotes
        {
            get { return this.vsHeader.ReleaseNotes; }
        }

        public byte[] ReleaseNotesContent
        {
            get { return this.vsHeader.ReleaseNotesContent; }
        }

        public string ReleaseNotesFormat
        {
            get { return this.vsHeader.ReleaseNotesFormat; }
        }

        public IVersionRange SupportedFrameworkVersionRange
        {
            get { return new VsVersionRange(this.vsHeader.SupportedFrameworkVersionRange); }
        }

        public IEnumerable<string> Tags
        {
            get { return this.vsHeader.Tags; }
        }
#endif
    }
}