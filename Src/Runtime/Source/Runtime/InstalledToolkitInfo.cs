using System;
using System.IO;
using Microsoft.VisualStudio.ExtensionManager;

namespace Microsoft.VisualStudio.Patterning.Runtime
{
    // TODO: this class should be abstract and implemented and exported by toolkits.

    /// <summary>
    /// Default implementation of an installed toolkit 
    /// information which exposes information from a 
    /// Visual Studio extension.
    /// </summary>
    [CLSCompliant(false)]
    public class InstalledToolkitInfo : IInstalledToolkitInfo
    {
        // Lazy initialized;
        private IPatternModelInfo schema;

        private ISchemaReader reader;
        private ISchemaResource resource;

        /// <summary>
        /// Initializes a new instance of the <see cref="InstalledToolkitInfo"/> class.
        /// </summary>
        /// <param name="extension">The extension.</param>
        /// <param name="reader">The reader.</param>
        /// <param name="resource">The resource.</param>
        public InstalledToolkitInfo(IInstalledExtension extension, ISchemaReader reader, ISchemaResource resource)
        {
            Guard.NotNull(() => extension, extension);
            Guard.NotNull(() => reader, reader);
            Guard.NotNull(() => resource, resource);

            this.Extension = extension;
            this.reader = reader;
            this.resource = resource;
            this.PatternIconPath = null;
            this.ToolkitIconPath = GetIconFromExtension(extension);
        }

        /// <summary>
        /// Gets the author of the extension.
        /// </summary>
        public string Author
        {
            get
            {
                return this.Extension.Header.Author;
            }
        }

        /// <summary>
        /// Gets the version of the extension.
        /// </summary>
        /// <value></value>
        public Version Version
        {
            get
            {
                return this.Extension.Header.Version;
            }
        }

        /// <summary>
        /// Gets the description of the extension.
        /// </summary>
        public string Description
        {
            get
            {
                return this.Extension.Header.Description;
            }
        }

        /// <summary>
        /// Gets the URL to download the extension from.
        /// </summary>
        public Uri DownloadUri
        {
            get
            {
                return new Uri(this.Extension.InstallPath);
            }
        }

        /// <summary>
        /// Gets the installed extension information.
        /// </summary>
        public IInstalledExtension Extension { get; private set; }

        /// <summary>
        /// Gets the toolkit schema information.
        /// </summary>
        public IPatternModelInfo Schema
        {
            get
            {
                if (this.schema == null)
                {
                    this.schema = this.LoadSchema();
                }

                return this.schema;
            }
        }

        /// <summary>
        /// Gets the icon of the pattern.
        /// </summary>
        public string PatternIconPath { get; private set; }

        /// <summary>
        /// Gets the icon of the toolkit.
        /// </summary>
        public string ToolkitIconPath { get; private set; }

        /// <summary>
        /// Gets the identifier for the extension.
        /// </summary>
        public string Id
        {
            get
            {
                return this.Extension.Header.Identifier;
            }
        }

        /// <summary>
        /// Gets the name of the extension.
        /// </summary>
        public string Name
        {
            get
            {
                return this.Extension.Header.Name;
            }
        }

        private static string GetIconFromExtension(IInstalledExtension extension)
        {
            if (extension.Header == null || string.IsNullOrEmpty(extension.Header.Icon))
            {
                return null;
            }

            // Ensure file exists in VSIX at configured path.
            var fullPath = Path.Combine(extension.InstallPath, extension.Header.Icon);
            return File.Exists(fullPath) ? fullPath : null;
        }

        private IPatternModelInfo LoadSchema()
        {
            return this.reader.Load(this.resource);
        }
    }
}