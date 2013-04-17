using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NuPattern.VisualStudio.Extensions;
using NuPattern.VisualStudio.Solution.Templates;

namespace NuPattern.Runtime
{
    // TODO: this class should be abstract and implemented and exported by toolkits.

    /// <summary>
    /// Default implementation of an installed toolkit 
    /// information which exposes information from a 
    /// Visual Studio extension.
    /// </summary>
    internal class InstalledToolkitInfo : IInstalledToolkitInfo
    {
        internal const string PatternModelCustomExtensionName = "NuPattern.Toolkit.PatternModel";
        internal const string ToolkitClassificationCustomExtensionName = "NuPattern.Toolkit.Classification";
        internal const string CategoryAttributeName = "Category";
        internal const string CustomizeVisibilityAttributeName = "CustomizeVisibility";
        internal const string CreateVisibilityAttributeName = "CreateVisibility";

        // Lazy initialized;
        private IPatternModelInfo schema;

        private ISchemaReader reader;
        private ISchemaResource resource;
        private List<IVsTemplate> templates;

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

        /// <summary>
        /// Gets the VS templates deployed by this toolkit.
        /// </summary>
        public IEnumerable<IVsTemplate> Templates
        {
            get
            {
                if (this.templates == null)
                {
                    LoadTemplates();
                }

                return this.templates;
            }
        }

        /// <summary>
        /// Gets the classification of the toolkit.
        /// </summary>
        public IToolkitClassification Classification
        {
            get
            {
                return GetClassification();
            }
        }


        private void LoadTemplates()
        {
            this.templates = new List<IVsTemplate>();

            // Get list of vstemplates in installation directories
            var installationDir = Directory.Exists(this.Extension.InstallPath) ? new DirectoryInfo(this.Extension.InstallPath) : null;
            if (installationDir != null)
            {
                var templateFiles = installationDir.GetFiles("*.zip", SearchOption.AllDirectories);
                templateFiles.ToList()
                    .ForEach(tf => this.templates.Add(VsTemplateFile.Read(tf.FullName)));
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

        private IToolkitClassification GetClassification()
        {
            // Read the 'ToolkitClassification' customextension
            var customExtension = this.Extension.Content.Where(cnt => cnt.ContentTypeName.Equals(ToolkitClassificationCustomExtensionName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            if (customExtension != null
                && customExtension.Attributes != null)
            {
                return new ToolkitClassification(
                    customExtension.GetCustomAttributeValue(CategoryAttributeName),
                    customExtension.GetVisibilityAttributeValue(CreateVisibilityAttributeName, ExtensionVisibility.Expanded),
                    customExtension.GetVisibilityAttributeValue(CustomizeVisibilityAttributeName, ExtensionVisibility.Expanded));
            }
            else
            {
                return new ToolkitClassification();
            }
        }
    }
}