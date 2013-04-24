using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.Shell;
using NuPattern.Diagnostics;
using NuPattern.Runtime.Schema.Properties;
using NuPattern.VisualStudio.Solution;

namespace NuPattern.Runtime.Schema
{
    /// <summary>
    /// Th context for a schema upgrade.
    /// </summary>
    internal class SchemaUpgradeContext : ISchemaUpgradeContext
    {
        private static readonly ITracer tracer = Tracer.Get<SchemaUpgradeContext>();

        internal const string UpgradedFileNameFormat = "{0}.{1}.backup";
        private const string DefaultNamespace = SchemaConstants.DefaultNamespace;
        private static readonly XName PatternModelRootElementName = XName.Get("patternModel", DefaultNamespace);
        private static readonly XName PatternModelDiagramRootElementName = XName.Get("patternModelDiagram", string.Empty);
        private const string DslVersionAttributeName = "dslVersion";
        private static readonly Version RuntimeSchemaVersion = new Version(NuPattern.Runtime.Schema.Constants.ProductVersion);
        private XDocument document;
        private Version schemaVersion;
        private IEnumerable<string> diagramFilePaths;
        private bool isDirty;

        /// <summary>
        /// Creates a new instance of the <see cref="SchemaUpgradeContext"/> class.
        /// </summary>
        public SchemaUpgradeContext(string filePath, IEnumerable<string> diagramFilePaths)
        {
            Guard.NotNullOrEmpty(() => filePath, filePath);

            this.SchemaFilePath = filePath;
            this.diagramFilePaths = diagramFilePaths;
            this.isDirty = false;
        }

        /// <summary>
        /// Gets the upgrade processors.
        /// </summary>
        [ImportMany]
        public IEnumerable<Lazy<IPatternModelSchemaUpgradeProcessor, ISchemaUpgradeProccesorOptions>> UpgradeProcessors { get; set; }

        /// <summary>
        /// Returns the schema as an <see cref="XDocument"/>.
        /// </summary>
        public XDocument OpenSchema()
        {
            this.document = XDocument.Load(this.SchemaFilePath);
            this.document.Changed += document_Changed;
            return this.document;
        }

        /// <summary>
        /// Creates a backup of the schema document.
        /// </summary>
        /// <remarks>The schema document must not be saved before this method is called.</remarks>
        public void BackupSchema()
        {
            var fileName = Path.GetFileName(this.SchemaFilePath);
            var backupFilePath = string.Format(CultureInfo.InvariantCulture, UpgradedFileNameFormat, this.SchemaFilePath, this.SchemaVersion);

            try
            {
                // Backup original file
                File.Copy(this.SchemaFilePath, backupFilePath, true);

                tracer.Info(ShellResources.SchemaUpgradeContext_TraceUpgradedBackup, fileName, this.SchemaVersion, backupFilePath);
            }
            catch (Exception ex)
            {
                tracer.Error(ShellResources.SchemaUpgradeContext_ErrorUpgradedBackup, this.SchemaFilePath, ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Saves the schema document.
        /// </summary>
        public void SaveSchema()
        {
            if (this.document != null)
            {
                if (this.isDirty)
                {
                    tracer.Verbose(ShellResources.SchemaUpgradeContext_TraceSaveDocument);

                    // Save the schema document
                    SaveDocument(document, this.SchemaFilePath);

                    // Update and save diagram files
                    this.diagramFilePaths.ForEach(diag => UpdateAndSaveDiagram(diag, this.SchemaVersion));
                }
            }
        }

        /// <summary>
        /// Gets or sets the full path to the schema document.
        /// </summary>
        public string SchemaFilePath { get; private set; }

        /// <summary>
        /// Gets the version of the schema
        /// </summary>
        public Version SchemaVersion
        {
            get
            {
                if (this.schemaVersion == null)
                {
                    GetSchemaVersion();
                }

                return this.schemaVersion;
            }
            set
            {
                SetSchemaVersion(value);
                this.schemaVersion = value;
            }
        }

        /// <summary>
        /// Gets the runtime version.
        /// </summary>
        public Version RuntimeVersion { get { return RuntimeSchemaVersion; } }

        private void document_Changed(object sender, XObjectChangeEventArgs e)
        {
            this.isDirty = true;
        }

        private static void SaveDocument(XDocument document, string filePath)
        {
            var serviceProvider = ServiceProvider.GlobalProvider;

            tracer.Info(ShellResources.SchemaUpgradeContext_TraceSaveSchemaFile, filePath);

            // Save file content
            VsHelper.CheckOut(filePath);
            VsHelper.WithoutFileChangeNotification(serviceProvider, filePath, () =>
                {
                    document.Save(filePath);
                });
        }

        private void GetSchemaVersion()
        {
            var doc = this.document ?? this.OpenSchema();
            var rootElement = doc.Descendants(PatternModelRootElementName).FirstOrDefault();
            if (rootElement != null)
            {
                this.schemaVersion = new Version(rootElement.Attribute(DslVersionAttributeName).Value);
            }
        }

        private void SetSchemaVersion(Version newVersion)
        {
            var doc = this.document ?? this.OpenSchema();
            UpdateSchemaVersion(doc, PatternModelRootElementName, newVersion);
        }

        private static void UpdateSchemaVersion(XDocument document, XName rootElementName, Version newVersion)
        {
            var rootElement = document.Descendants(rootElementName).FirstOrDefault();
            if (rootElement != null)
            {
                var currentVersionString = rootElement.Attribute(DslVersionAttributeName).Value;
                var newVersionString = newVersion.ToString(4);
                if (String.IsNullOrEmpty(currentVersionString) ||
                    !currentVersionString.Equals(newVersionString, StringComparison.Ordinal))
                {
                    tracer.Info(ShellResources.SchemaUpgradeContext_TraceUpgradeSchemaVersion, currentVersionString, newVersion);

                    rootElement.Attribute(DslVersionAttributeName).Value = newVersionString;
                }
            }
        }

        private void UpdateAndSaveDiagram(string filePath, Version newVersion)
        {
            if (File.Exists(filePath))
            {
                var diagramDocument = XDocument.Load(filePath);
                UpdateSchemaVersion(diagramDocument, PatternModelDiagramRootElementName, newVersion);
                SaveDocument(diagramDocument, filePath);
            }
        }
    }
}